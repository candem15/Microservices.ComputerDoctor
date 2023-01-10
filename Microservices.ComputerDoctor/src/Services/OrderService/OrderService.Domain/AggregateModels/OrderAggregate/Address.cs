using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class Address
    {
        public string Street { get; private set; }
        public string City { get; private set; }
        public string Country { get; private set; }
        public string State { get; private set; }
        public string ZipCode { get; private set; }

        public Address(string zipCode, string state, string country, string city, string street)
        {
            ZipCode = zipCode;
            State = state;
            Country = country;
            City = city;
            Street = street;
        }
        public Address()
        {

        }
    }
}
