using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
    public class PaymentMethod : BaseEntity
    {
        public string Alias { get; set; }
        public string CardNumber { get; set; }
        public string SecurityNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime Expiration { get; set; }

        public int CardTypeId { get; set; }
        public CardType CardType { get; set; }

        public PaymentMethod(string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration, int cardTypeId)
        {

            CardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new PaymentMethodDomainException(nameof(cardNumber));
            SecurityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new PaymentMethodDomainException(nameof(securityNumber));
            CardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new PaymentMethodDomainException(nameof(cardHolderName));
            if (expiration < DateTime.UtcNow)
                throw new OrderingDomainException(nameof(expiration));

            Expiration = expiration;
            CardTypeId = cardTypeId;
            Alias = alias;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        {
            return CardTypeId == cardTypeId && CardNumber == cardNumber && Expiration == expiration;
        }
    }
}
