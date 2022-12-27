using System.Net.Sockets;
using Polly;
using Polly.Contrib.WaitAndRetry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class RabbitMQPersistenceConnection : IDisposable
    {
        private IConnection connection;
        private readonly IConnectionFactory _connectionFactory;
        private object lock_object = new object();
        private readonly int retryCount;
        private bool _diposed;

        public RabbitMQPersistenceConnection(IConnectionFactory connectionFactory, int retryCount = 5)
        {
            this.retryCount = retryCount;
            _connectionFactory = connectionFactory;
        }

        public bool IsConnected => connection != null && connection.IsOpen;

        public IModel CreateModel()
        {
            return connection.CreateModel();
        }

        public void Dispose()
        {
            _diposed = true;
            connection.Dispose();
        }

        public bool TryConnect()
        {
            lock (lock_object)
            {
                var policy = Policy.Handle<SocketException>()
                    .Or<BrokerUnreachableException>()
                    .WaitAndRetry(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));
                policy.Execute(() =>
                {
                    connection = _connectionFactory.CreateConnection();
                });

                if (IsConnected)
                {
                    connection.ConnectionShutdown += Connection_ConnectionShutdown;
                    connection.ConnectionBlocked += Connection_ConnectionBlocked;
                    connection.CallbackException += Connection_CallbackException;
                    return true;
                }

                return false;
            }
        }

        private void Connection_CallbackException(object? sender, CallbackExceptionEventArgs e)
        {
            if (_diposed) return;
            TryConnect();
        }

        private void Connection_ConnectionBlocked(object? sender, ConnectionBlockedEventArgs e)
        {
            if (_diposed) return;
            TryConnect();
        }

        private void Connection_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            if (_diposed) return;
            TryConnect();
        }
    }
}