using MediatR;
using OrderService.Domain.AggregateModels.OrderAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Events
{
    public class OrderStartedDomainEvent : INotification
    {
        public string Username { get; }
        public int CardTypeId { get; }
        public string CardNumber { get; }
        public string CardSecurityNumber { get; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public Order Order { get; set; }
        public OrderStartedDomainEvent(string username, int cardTypeId, string cardNumber, string cardSecurityNumber, string cardHolderName, DateTime cardExpiration, Order order)
        {
            Username = username;
            CardTypeId = cardTypeId;
            CardNumber = cardNumber;
            CardSecurityNumber = cardSecurityNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            Order = order;
        }
    }
}
