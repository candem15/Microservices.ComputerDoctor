using System.Net.Sockets;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using Polly.Contrib.WaitAndRetry;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        RabbitMQPersistenceConnection persistenceConnection;
        private readonly IConnectionFactory connectionFactory;
        private readonly EventBusConfig eventBusConfig;
        private IModel consumerChannel;

        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig eventBusConfig) : base(serviceProvider, eventBusConfig)
        {
            this.eventBusConfig = eventBusConfig;
            if (eventBusConfig.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(eventBusConfig.Connection, new JsonSerializerSettings()
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
            }
            else
                connectionFactory = new ConnectionFactory();

            persistenceConnection = new RabbitMQPersistenceConnection(connectionFactory, eventBusConfig.ConnectionRetryCount);

            consumerChannel = CreateConsumerChannel();
            SubscriptionManager.OnEventRemoved += SubscriptionManager_OnEventRemoved;
        }

        private void SubscriptionManager_OnEventRemoved(object? sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!persistenceConnection.IsConnected)
            {
                persistenceConnection.TryConnect();
            }

            consumerChannel.QueueUnbind(queue: eventName,
                                        exchange: eventBusConfig.DefaultTopicName,
                                        routingKey: eventName);

            if (SubscriptionManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!persistenceConnection.IsConnected)
            {
                persistenceConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>()
                    .Or<SocketException>()
                    .WaitAndRetry(Backoff.DecorrelatedJitterBackoffV2(TimeSpan.FromSeconds(1), 5));

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: eventName,
                                            type: "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2; // Persistent modu

                // consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                //                             durable: true,
                //                             exclusive: false,
                //                             autoDelete: false,
                //                             arguments: null);

                consumerChannel.BasicPublish(exchange: eventBusConfig.DefaultTopicName,
                                            routingKey: eventName,
                                            mandatory: true,
                                            basicProperties: properties,
                                            body: body);
            });
        }

        public override void Subcribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubscriptionManager.HasSubscriptionForEvent(eventName))
            {
                if (!persistenceConnection.IsConnected)
                {
                    persistenceConnection.TryConnect();
                }

                consumerChannel.QueueDeclare(queue: GetSubName(eventName),
                                             durable: true,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                consumerChannel.QueueBind(queue: GetSubName(eventName),
                                          exchange: eventBusConfig.DefaultTopicName,
                                          routingKey: eventName);
            }

            SubscriptionManager.AddSubscription<T, TH>();
            StartBasicConsume(eventName);
        }

        public override void UnSubcribe<T, TH>()
        {
            SubscriptionManager.RemoveSubscription<T, TH>();
        }

        private IModel CreateConsumerChannel()
        {
            if (!persistenceConnection.IsConnected)
            {
                persistenceConnection.TryConnect();
            }

            var channel = persistenceConnection.CreateModel();

            channel.ExchangeDeclare(exchange: eventBusConfig.DefaultTopicName, type: "direct");

            return channel;
        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(queue: GetSubName(eventName),
                                             autoAck: false,
                                             consumer: consumer);
            }
        }

        private async void Consumer_Received(object? sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(e.Body.Span);

            try
            {
                await ProcessEvent(eventName, message);
            }
            catch (Exception ex)
            {
                //Logging mekanizması kullanılabilir.
            }

            consumerChannel.BasicAck(e.DeliveryTag, multiple: false);
        }
    }
}