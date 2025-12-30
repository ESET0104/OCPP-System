using Chargersimulator.Ocpp.Messages;

namespace Chargersimulator.Ocpp;

public static class OcppMessageBuilder
{

    public static object BootNotification()
          => Chargersimulator.Ocpp.Messages.BootNotification.Create();

    public static object Heartbeat()
        => new object[]
        {
            2,
            Guid.NewGuid().ToString(),
            "Heartbeat",
            new { }
        };

    public static object Authorize(string idToken)
        => Chargersimulator.Ocpp.Messages.Authorize.Create(idToken);

    public static object TransactionStarted(
        string sessionId,
        string chargerId,
        string userId)
        => TransactionEvent.Create(
            eventType: "Started",
            sessionId: sessionId,
            chargerId: chargerId,
            userId: userId,
            triggerReason: "Authorized",
            seqNo: 1
        );

    public static object TransactionEnded(
        string sessionId,
        string chargerId,
        string userId,
        string triggerReason)
        => TransactionEvent.Create(
            eventType: "Ended",
            sessionId: sessionId,
            chargerId: chargerId,
            userId: userId,
            triggerReason: triggerReason,
            seqNo: 2
        );


    public static object StatusAvailable()
        => StatusNotification.Create("Available");

    public static object StatusCharging()
        => StatusNotification.Create("Charging");

    public static object MeterValues(decimal energyKwh)
    => Chargersimulator.Ocpp.Messages.MeterValues.Create(energyKwh);

    public static object StatusFaulted(string faultCode)
    => StatusNotification.Create("Faulted", faultCode);


}
