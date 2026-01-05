using Chargersimulator.Ocpp;
using Chargersimulator.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;



public class WebSocketClient
{
    private TaskCompletionSource<bool>? _pendingVinAuthorization;
    private readonly ClientWebSocket _socket = new();
    private readonly ChargerState _state;
    private readonly Uri _uri;

    public ChargerState State => _state;


    public WebSocketClient(ChargerState state, string url)
    {
        _state = state;
        _uri = new Uri(url);
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Connecting to OCPP microservice...");
        await _socket.ConnectAsync(_uri, CancellationToken.None);
        Console.WriteLine("WebSocket connected");

        await SendAsync(OcppMessageBuilder.BootNotification());
        await SendAsync(OcppMessageBuilder.StatusAvailable());

        _ = HeartbeatLoop();
        _ = ReceiveLoop();
      
    }

    private async Task HeartbeatLoop()
    {
        while (_socket.State == WebSocketState.Open)
        {
            await Task.Delay(TimeSpan.FromSeconds(30));
            await SendAsync(OcppMessageBuilder.Heartbeat());
        }
    }

    private async Task ReceiveLoop()
    {
        var buffer = new byte[8192];
        try
        {
            while (_socket.State == WebSocketState.Open)
            {
                var result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                Console.WriteLine("--" + json);
                HandleIncoming(json);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
        }
        finally
        {
            Console.WriteLine("WebSocket disconnected unexpectedly");

            if (_state.Status == ChargerStatus.Charging &&
                _state.ActiveSessionId != null)
            {
                await SendAsync(
                    OcppMessageBuilder.TransactionEnded(
                        _state.ActiveSessionId!,
                        _state.ChargerId,
                        _state.ActiveUserId!,
                        "PowerLoss"
                    )
                );
            }
            _state.Status = ChargerStatus.Faulted;
        }
    }


    private void HandleIncoming(string json)
    {
        var doc = JsonDocument.Parse(json);
        var messageType = doc.RootElement[0].GetInt32();

        //
        if (messageType == 3)
        {
            var messageId = doc.RootElement[1].GetString();
            var payloadmsg = doc.RootElement[2];

            // Only VIN auth uses CallResult right now
            if (_pendingVinAuthorization != null)
            {
                var status = payloadmsg
                    .GetProperty("idTokenInfo")
                    .GetProperty("status")
                    .GetString();

                var accepted = status == "Accepted";

                _pendingVinAuthorization.TrySetResult(accepted);
                _pendingVinAuthorization = null;

                if (accepted)
                {
                    Console.WriteLine(" VIN accepted — waiting for RemoteStart");
                    _state.Status = ChargerStatus.Preparing;
                    //
                    _= SendAsync(OcppMessageBuilder.StatusPreparing());
                    //
                }
                else
                {
                    Console.WriteLine(" VIN rejected");
                    _state.Status = ChargerStatus.Available;
                }
            }

            return;
        }
        //

        if (messageType != 2)
            return;

        var action = doc.RootElement[2].GetString();
        var payload = doc.RootElement[3];

        switch (action)
        {
            case "RequestStartTransaction":
                _ = HandleRemoteStart(payload);
                break;

            case "RequestStopTransaction":
                _ = HandleRemoteStop(payload);
                break;
            //case "Authorize":
            //    HandleAuthorizeResponse(payload);
            //    break;
        }
    }


    private async Task HandleRemoteStart(JsonElement payload)
    {
        Console.WriteLine("Remote Start Request received");

        //if (_state.Status != ChargerStatus.Available)
        //{
        //    Console.WriteLine("Charger not available");
        //    return;
        //}
        if (
    _state.Status != ChargerStatus.Preparing || _state.ActiveVin == null)
        {
            Console.WriteLine("Cannot start — no authorized vehicle connected");
            return;
        }

        _state.ActiveSessionId = payload.GetProperty("sessionId").GetString();
        _state.ActiveUserId = payload.GetProperty("userId").GetString();
        _state.Status = ChargerStatus.Charging;
        _state.TotalEnergyKwh = 0;

        //await SendAsync(OcppMessageBuilder.Authorize(_state.ActiveUserId!));
       
        await SendAsync(
     OcppMessageBuilder.TransactionStarted(
         _state.ActiveSessionId!,
         _state.ChargerId,
         _state.ActiveUserId!
     )
 );

        await SendAsync(OcppMessageBuilder.StatusCharging());

      
        _ = StartMeteringLoop();

        Console.WriteLine("Charging started");
    }



    private async Task HandleRemoteStop(JsonElement payload)
    {
        Console.WriteLine("Remote Stop Request received");

        var sessionId = payload.GetProperty("sessionId").GetString();

        if (_state.Status != ChargerStatus.Charging ||
            _state.ActiveSessionId != sessionId)
        {
            Console.WriteLine("Session mismatch");
            return;
        }

     
        _state.MeteringCts?.Cancel();

        
        await SendAsync(
         OcppMessageBuilder.TransactionEnded(
             _state.ActiveSessionId!,
             _state.ChargerId,
             _state.ActiveUserId!,
             "Authorized"
            
         )
     );

        await SendAsync(OcppMessageBuilder.StatusAvailable());

        _state.ActiveSessionId = null;
        _state.ActiveUserId = null;
        _state.Status = ChargerStatus.Available;
        //_state.TotalEnergyKwh = 0;

        Console.WriteLine("Charging stopped");
    }

    
    private async Task StartMeteringLoop()
    {
        _state.MeteringCts = new CancellationTokenSource();
        var token = _state.MeteringCts.Token;

        while (!token.IsCancellationRequested &&
               _state.Status == ChargerStatus.Charging)
        {
            await Task.Delay(TimeSpan.FromSeconds(10), token);

            
            _state.TotalEnergyKwh += 0.5m;

            await SendAsync(
                OcppMessageBuilder.MeterValues(_state.TotalEnergyKwh,_state.ActiveSoc)
            );

            Console.WriteLine($"Energy = {_state.TotalEnergyKwh:F2} kWh");
        }
    }



    private async Task SendAsync(object payload)
    {
        var json = JsonSerializer.Serialize(payload);
        var bytes = Encoding.UTF8.GetBytes(json);

        await _socket.SendAsync(
            bytes,
            WebSocketMessageType.Text,
            true,
            CancellationToken.None);

        Console.WriteLine("-- " + json);
    }


    //
    public async Task<bool> AuthorizeVinAsync(string vin)
    {
        if (_pendingVinAuthorization != null)
            throw new InvalidOperationException("VIN authorization already in progress");

        _pendingVinAuthorization = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        _state.ActiveVin = vin;

        await SendAsync(OcppMessageBuilder.Authorize(vin));

        return await _pendingVinAuthorization.Task;
    }



    public async Task SimulateFault(string faultCode)
    {
        if (_state.Status == ChargerStatus.Faulted)
            return;

        Console.WriteLine($"Simulating fault: {faultCode}");

        _state.MeteringCts?.Cancel();

        if (_state.ActiveSessionId != null)
        {
            await SendAsync(
                OcppMessageBuilder.TransactionEnded(
                    _state.ActiveSessionId!,
                    _state.ChargerId,
                    _state.ActiveUserId!,
                    "Fault"
                )
            );
        }

        await SendAsync(
            OcppMessageBuilder.StatusFaulted(faultCode)
        );

        _state.Status = ChargerStatus.Faulted;
    }


    public async Task SimulatePowerLoss()
    {
        Console.WriteLine("Simulating POWER LOSS");

        _state.MeteringCts?.Cancel();

        Environment.FailFast("Power loss simulated");
    }


    public bool CanAcceptSoc =>
    _state.Status == ChargerStatus.Charging;

    public void SetCar(string vin, double soc)
    {
        _state.ActiveVin = vin;
        _state.ActiveSoc = soc;

        Console.WriteLine($"CAR CONNECTED: {vin}, SOC={soc}%");
    }

    public void SetSoc(double soc)
    {
        Console.WriteLine($"CAR SOC UPDATE: {soc}%");
    }

    public void RemoveCar()
    {
        Console.WriteLine("CAR DISCONNECTED");
    }




}
