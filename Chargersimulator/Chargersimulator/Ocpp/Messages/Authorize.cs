namespace Chargersimulator.Ocpp.Messages;

public static class Authorize
{
    public static object Create(string idToken)
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
                    idToken = idToken,
                    type = "Central"
                }
            }
        };
    }
}
