using BasketService.Api.Core.Domain.Models;
using EventBus.Base.Events;

namespace BasketService.Api.IntegrationEvents.Events
{
    public class OrderCreatedIntegrationEvent : IntegrationEvent
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public int OrderNumber { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string CardNumber { get; set; }
        public string CardHolderName { get; set; }
        public DateTime CardExpiration { get; set; }
        public string CardSecurityNumber { get; set; }
        public int CardTypeId { get; set; }
        public string Buyer { get; set; }
        //public Guid RequestId { get; set; }
        public CustomerBasket Basket { get; }

        public OrderCreatedIntegrationEvent(string userId, string username/*, int orderNumber*/, string city, string street, string country, string state, string zipCode, string cardNumber, string cardHolderName, DateTime cardExpiration, string cardSecurityNumber, int cardTypeId, string buyer/*, Guid requestId*/, CustomerBasket basket)
        {
            UserId = userId;
            Username = username;
            //OrderNumber = orderNumber;
            City = city;
            Street = street;
            Country = country;
            State = state;
            ZipCode = zipCode;
            CardNumber = cardNumber;
            CardHolderName = cardHolderName;
            CardExpiration = cardExpiration;
            CardSecurityNumber = cardSecurityNumber;
            CardTypeId = cardTypeId;
            Buyer = buyer;
            //RequestId = requestId;
            Basket = basket;
        }
    }
}
