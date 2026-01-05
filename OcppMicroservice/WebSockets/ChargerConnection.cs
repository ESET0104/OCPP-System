using OcppMicroservice.Messaging;
using OcppMicroservice.Ocpp;
using OcppMicroservice.State;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace OcppMicroservice.WebSockets
{
    public class ChargerConnection
    {
        private readonly WebSocket _socket;
        private readonly string _chargePointId;
        private readonly string _tenantId;

        public ChargerConnection(string chargePointId, string tenantId, WebSocket socket)
        {
            _chargePointId = chargePointId;
            _tenantId = tenantId;
            _socket = socket;
        }

        public async Task ListenAsync()
        {
            ChargerConnectionManager.Add(_chargePointId, _socket);
            bool gracefulClose = false;

            try
            {
                var buffer = new byte[8192];

                
                while (_socket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    try
                    {
                        result = await _socket.ReceiveAsync(buffer, CancellationToken.None);
                    }
                    catch (WebSocketException ex)
                    {
                        Console.WriteLine(
                            $"WebSocket abruptly closed for charger {_chargePointId}: {ex.Message}"
                        );
                        break;
                    }
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        gracefulClose = true;
                        break;
                    }
                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    var doc = JsonDocument.Parse(json);
                    //var payload = doc.RootElement[3];
                    var payload = doc.RootElement.GetArrayLength() > 3? doc.RootElement[3]: default;
                    await OcppRouter.RouteAsync(
                        json,
                        _chargePointId,
                        _tenantId,
                        //payload,
                        _socket);
                }
            }
           
            finally
            {
                ChargerConnectionManager.Remove(_chargePointId);
                if (!gracefulClose)
                {
                    Console.WriteLine($"Charger {_chargePointId} disconnected unexpectedly");

                    var state = ChargerStateStore.Get(_chargePointId);
                    await RabbitMqEventPublisher.PublishAsync(
                        "event.charger.faulted",
                        new
                        {
                            ChargerId = _chargePointId,
                            FaultCode = "PowerLoss",
                            Timestamp = DateTime.UtcNow
                        }
                    );
                    if (state.ActiveSessionId != null)
                    {
                        
                        state.ActiveSessionId = null;
                    }
                }

                try
                {
                    if (_socket.State != WebSocketState.Closed)
                    {
                        await _socket.CloseAsync(
                            WebSocketCloseStatus.NormalClosure,
                            "Connection closed",
                            CancellationToken.None);
                    }
                }
                catch { }
            }

        }
    }

}
