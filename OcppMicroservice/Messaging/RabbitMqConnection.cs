using RabbitMQ.Client;
using System.Threading.Tasks;

namespace OcppMicroservice.Messaging
{
    public static class RabbitMqConnection
    {
        public static IConnection Connection { get; }
        public static IChannel Channel { get; }

        static RabbitMqConnection()
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                VirtualHost = "ev-charging",
                UserName = "guest",
                Password = "guest"
            };
          
            Connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
          
            Channel = Connection.CreateChannelAsync().GetAwaiter().GetResult();
        }
    }
}
