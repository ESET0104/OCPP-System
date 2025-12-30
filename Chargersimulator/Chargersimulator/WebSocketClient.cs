using Chargersimulator.Ocpp;
using Chargersimulator.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class WebSocketClient
{
    private readonly ClientWebSocket _socket = new();
    private readonly ChargerState _state;
    private readonly Uri _uri;

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
        }
    }


    private async Task HandleRemoteStart(JsonElement payload)
    {
        Console.WriteLine("Remote Start Request received");

        if (_state.Status != ChargerStatus.Available)
        {
            Console.WriteLine("Charger not available");
            return;
        }

        _state.ActiveSessionId = payload.GetProperty("sessionId").GetString();
        _state.ActiveUserId = payload.GetProperty("userId").GetString();
        _state.Status = ChargerStatus.Charging;
        _state.TotalEnergyKwh = 0;

        await SendAsync(OcppMessageBuilder.Authorize(_state.ActiveUserId!));
       
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
                OcppMessageBuilder.MeterValues(_state.TotalEnergyKwh)
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




}
