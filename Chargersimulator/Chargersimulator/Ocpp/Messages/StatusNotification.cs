namespace Chargersimulator.Ocpp.Messages;

public static class StatusNotification
{
    public static object Create(string status)
    {
        return new object[]
        {
            2, // CALL
            Guid.NewGuid().ToString(),
            "StatusNotification",
            new
            {
                timestamp = DateTime.UtcNow,
                evseId = 1,
                connectorId = 1,
                connectorStatus = status
                // Available | Charging | Faulted 
            }
        };
    }
}

