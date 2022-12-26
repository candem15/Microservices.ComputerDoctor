using EventBus.Base.Abstraction;
using EventBus.Base.SubscriptionManagers;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
    public abstract class BaseEventBus : IEventBus
    {
        public readonly IServiceProvider ServiceProvider;
        public readonly IEventBusSubscriptionManager SubscriptionManager;
        private EventBusConfig eventBusConfig;
        public BaseEventBus(IServiceProvider serviceProvider, EventBusConfig eventBusConfig)
        {
            ServiceProvider = serviceProvider;
            this.eventBusConfig = eventBusConfig;
            SubscriptionManager = new InMemoryEventBusSubscriptionManager(ProcessEventName);
        }
        public virtual string ProcessEventName(string eventName) //Event isimlerini kırpıp istediğimiz şekle büründürür.
        {
            if (eventBusConfig.DeleteEventPrefix)
                eventName = eventName.TrimStart(eventBusConfig.EventNamePrefix.ToArray());

            if (eventBusConfig.DeleteEventSuffix)
                eventName = eventName.TrimEnd(eventBusConfig.EventNameSuffix.ToArray());

            return eventName;
        }
        public virtual string GetSubName(string eventName)
        {
            return $"{eventBusConfig.SubscriberClientAppName}.{ProcessEventName(eventName)}";
        }
        public virtual void Dispose()
        {
            eventBusConfig = null;
        }
        public async Task<bool> ProcessEvent(string eventName, string message)
        {
            eventName = ProcessEventName(eventName);

            var process = false;

            if (SubscriptionManager.HasSubscriptionForEvent(eventName)) //Gelen event takip edilmiş mi diye kontrol ediyoruz, eğer edilmişse işleme alıyoruz.
            {
                var subscriptions = SubscriptionManager.GetHandlersForEvent(eventName); //Gelen eventin subscribe olunmuş handlerlarını alıyoruz.

                using (var scope = ServiceProvider.CreateScope()) //Altındaki işlemlerin aynı service scope u içerinde çalışmasını istiyoruz.
                {
                    foreach (var subscription in subscriptions)
                    {
                        var handler = ServiceProvider.GetService(subscription.HandlerType);
                        if (handler == null) continue; //

                        var eventType = SubscriptionManager.GetEventTypeByName($"{eventBusConfig.EventNamePrefix}{eventName}{eventBusConfig.EventNameSuffix}");
                        var integrationEvent = JsonConvert.DeserializeObject(message, eventType);

                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(eventType);
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { integrationEvent }); //Reflection ile diğer servislerdeki handle methoduna ulaşarak tetikliyoruz.
                    }
                }
                process = true;
            }
            return process;
        }
        //Aşağıdaki methodlar RabbitMQ ve AzureServiceBus tarafında kendilerine göre override edilecekler.
        public abstract void Publish(IntegrationEvent @event);

        public abstract void Subcribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;

        public abstract void UnSubcribe<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>;
    }
}