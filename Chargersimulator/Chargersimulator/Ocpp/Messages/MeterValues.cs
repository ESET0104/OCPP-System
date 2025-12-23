namespace Chargersimulator.Ocpp.Messages;

public static class MeterValues
{
    public static object Create(decimal energyKwh)
    {
        return new object[]
        {
            2, // CALL
            Guid.NewGuid().ToString(),
            "MeterValues",
            new
            {
                evseId = 1,
                meterValue = new[]
                {
                    new
                    {
                        timestamp = DateTime.UtcNow,
                        sampledValue = new[]
                        {
                            new
                            {
                                value = energyKwh.ToString("F2"),
                                measurand = "Energy.Active.Import.Register",
                                unitOfMeasure = new
                                {
                                    unit = "kWh"
                                }
                            }
                        }
                    }
                }
            }
        };
    }
}
