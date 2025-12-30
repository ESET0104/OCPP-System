using Chargersimulator.State;

class Program
{
    static async Task Main()
    {
        var chargerState = new ChargerState
        {
            ChargerId = "q1eEGhWtH9",
            TenantId = "tenant1",
            Status = ChargerStatus.Available
        };

        var wsUrl =
            $"ws://localhost:5102/ws?tenantId={chargerState.TenantId}&chargePointId={chargerState.ChargerId}";

        var client = new WebSocketClient(chargerState, wsUrl);

        await client.StartAsync();

        Console.WriteLine("F = Fault | P = PowerLoss | E = EmergencyStop");

        while (true)
        {
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.F:
                    await client.SimulateFault("InternalError");
                    break;

                case ConsoleKey.E:
                    await client.SimulateFault("EmergencyStop");
                    break;

                case ConsoleKey.P:
                    Console.WriteLine("Simulating Power Loss...");
                    await client.SimulatePowerLoss();
                    return;
            }
        }
    }
}
