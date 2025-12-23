namespace OcppMicroservice.WebSockets
{
    public class WebSocketMiddleware
    {
        private readonly RequestDelegate _next;

        public WebSocketMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
            {
                await _next(context);
                return;
            }

            var chargePointId = context.Request.Query["chargePointId"].ToString();
            var tenantId = context.Request.Query["tenantId"].ToString();

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            var connection = new ChargerConnection(
                chargePointId,
                tenantId,
                socket
            );

            await connection.ListenAsync();
        }
    }

}
