using OcppMicroservice.Ocpp;
using System.Net.WebSockets;
using System.Text;

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

            try
            {
                var buffer = new byte[8192];

                while (_socket.State == WebSocketState.Open)
                {
                    var result = await _socket.ReceiveAsync(buffer, CancellationToken.None);

                    if (result.MessageType == WebSocketMessageType.Close)
                        break;

                    var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    await OcppRouter.RouteAsync(
                        json,
                        _chargePointId,
                        _tenantId,
                        _socket);
                }
            }
            finally
            {
                ChargerConnectionManager.Remove(_chargePointId);
                await _socket.CloseAsync(
                    WebSocketCloseStatus.NormalClosure,
                    "Connection closed",
                    CancellationToken.None);
            }
        }
    }

}
