using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventBus.Base
{
    public class SubcriptionInfo
    {
        // Gönderilen integrationEvent tipi burada tutularak yine bu tip üzerinden handler metoduna 'reflection' kullanarak ulaşılacak.
        public Type HandlerType { get; }

        public SubcriptionInfo(Type handlerType)
        {
            HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
        }

        public static SubcriptionInfo Typed(Type handlerType)
        {
            return new SubcriptionInfo(handlerType);
        }
    }
}