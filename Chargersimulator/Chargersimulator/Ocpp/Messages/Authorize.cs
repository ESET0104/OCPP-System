namespace Chargersimulator.Ocpp.Messages;

public static class Authorize
{
    public static object Create(string vin)
    {
        return new object[]
        {
            2, // CALL
            Guid.NewGuid().ToString(),
            "Authorize",
            new
            {
                idToken = new
                {
                    value = vin,
                    type = "VIN"
                }
            }
        };
    }
}
