namespace Chargersimulator.Ocpp.Messages;

public static class TransactionEvent
{
    public static object Create(
        string eventType,
        string sessionId,
        string chargerId,
        string userId,
      
        int seqNo = 1)
    {
        return new object[]
        {
            2, // CALL
            Guid.NewGuid().ToString(),
            "TransactionEvent",
            new
            {
                eventType, // Started | Ended
                timestamp = DateTime.UtcNow,
                triggerReason = "Authorized",
                seqNo,

                evse = new
                {
                    id = 1,
                    connectorId = 1
                },

                transactionInfo = new
                {
                    transactionId = sessionId,
                    sessionId,
                    chargerId,
                    userId,
                }
            }
            
        };
    }
}
