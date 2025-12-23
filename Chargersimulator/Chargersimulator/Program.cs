using Chargersimulator.State;

class Program
{
    static async Task Main()
    {
        var chargerState = new ChargerState
        {
            ChargerId = "NKX9RZJ0y1",
            TenantId = "tenant1",
            Status = ChargerStatus.Available
        };

        var wsUrl =
            $"ws://localhost:5102/ws?tenantId={chargerState.TenantId}&chargePointId={chargerState.ChargerId}";

        var client = new WebSocketClient(chargerState, wsUrl);
        await client.StartAsync();

        Console.ReadLine();
    }
}
