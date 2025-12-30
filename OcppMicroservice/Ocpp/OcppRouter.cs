using OcppMicroservice.Ocpp.Handlers;
using System.Net.WebSockets;
using System.Text.Json;

namespace OcppMicroservice.Ocpp
{
  
    public static class OcppRouter
    {
        public static async Task RouteAsync(
            string json,
            string chargePointId,
            string tenantId,
            WebSocket socket)
        {
            var message = OcppMessage.Parse(json);

            switch (message.Action)
            {
                case "BootNotification":
                    await BootNotificationHandler.Handle(
                        chargePointId,
                        message.MessageId,
                        socket);
                    break;

                case "Authorize":
                    await AuthorizeHandler.Handle(
                        message.MessageId,
                        socket);
                    break;

                case "Heartbeat":
                    await HeartbeatHandler.Handle(
                        message.Payload,
                        chargePointId);
                    break;

                case "TransactionEvent":
                    await TransactionEventHandler.Handle(
                        message.Payload,
                        chargePointId);
                    break;

                case "MeterValues":
                    await MeterValuesHandler.Handle(
                        message.Payload,
                        chargePointId);
                    break;

                case "StatusNotification":
                    StatusNotificationHandler.Handle(
                        message.Payload,
                        chargePointId);
                    break;
            }
        }

    }

    }
