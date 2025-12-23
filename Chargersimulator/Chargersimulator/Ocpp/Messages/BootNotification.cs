namespace Chargersimulator.Ocpp.Messages;

public static class BootNotification
{
    public static object Create()
    {
        return new object[]
        {
            2, // CALL
            Guid.NewGuid().ToString(),
            "BootNotification",
            new
            {
                chargingStation = new
                {
                    model = "Demo-Charger-Model",
                    vendorName = "Demo-Vendor"
                },
                reason = "PowerUp"
            }
        };
    }
}

