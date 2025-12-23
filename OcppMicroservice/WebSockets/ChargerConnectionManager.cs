using System.Net.WebSockets;
using System.Collections.Concurrent;
namespace OcppMicroservice.WebSockets
{
    public static class ChargerConnectionManager
    {
        private static readonly ConcurrentDictionary<string, WebSocket> _connections
            = new();

        public static void Add(string chargePointId, WebSocket socket)
        {
            _connections[chargePointId] = socket;
        }

        public static void Remove(string chargePointId)
        {
            _connections.TryRemove(chargePointId, out _);
        }

        public static WebSocket? GetSocket(string chargePointId)
        {
            if (_connections.TryGetValue(chargePointId, out var socket))
            {
                if (socket.State == WebSocketState.Open)
                    return socket;
            }

            return null;
        }
    }
}
