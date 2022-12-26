using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventBus.Base.Events;

namespace EventBus.Base.Abstraction
{
    public interface IEventBusSubscriptionManager
    {
        bool IsEmpty { get; }
        event EventHandler<string> OnEventRemoved; // Subscription silindiğinde yani 'unsubscribe' tetiklendiğinde bu metot da tetiklenir.
        void AddSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        void RemoveSubscription<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>;
        bool HasSubscriptionForEvent<T>() where T : IntegrationEvent; // Dışarıdan gönderilen eventin hali hazırda subscribe edilip edilmediğinin bilgisini tutar.
        bool HasSubscriptionForEvent(string eventName);
        Type? GetEventTypeByName(string eventName); // İsmi girilen eventin handler tipini geriye döndürür.
        void Clear(); //Tüm subscriptionları temizler.
        IEnumerable<SubcriptionInfo> GetHandlersForEvent<T>() where T : IntegrationEvent; // Verilen eventin tüm handlerlarını geriye döndürür.
        IEnumerable<SubcriptionInfo> GetHandlersForEvent(string eventName);
        string GetEventKey<T>(); // Eventler için 'routing key' değerini geriye döndürür.
    }
}