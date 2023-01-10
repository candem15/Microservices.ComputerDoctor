using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
    public class OrderStatus : Enumeration
    {
        public static OrderStatus Submitted = new(1, nameof(Submitted).ToLowerInvariant());
        public static OrderStatus AwaitingValidation = new(2, nameof(AwaitingValidation).ToLowerInvariant());
        public static OrderStatus StockConfirmed = new(1, nameof(StockConfirmed).ToLowerInvariant());
        public static OrderStatus Paid = new(1, nameof(Paid).ToLowerInvariant());
        public static OrderStatus Shipped = new(1, nameof(Shipped).ToLowerInvariant());
        public static OrderStatus Cancelled = new(1, nameof(Cancelled).ToLowerInvariant());

        public OrderStatus(int id, string name) : base(id, name)
        {

        }

        public static IEnumerable<OrderStatus> List()
        {
            return new[] { Submitted, AwaitingValidation, StockConfirmed, Paid, Shipped, Cancelled };
        }

        public static OrderStatus FromName(string name)
        {
            var state = List()
                            .SingleOrDefault(x => String.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));

            return state ?? throw new OrderingDomainException(String.Join(",", List().Select(x => x.Name)));
        }

        public static OrderStatus FromId(int id)
        {
            var state = List()
                            .SingleOrDefault(x => x.Id == id);

            return state ?? throw new OrderingDomainException(String.Join(",", List().Select(x => x.Name)));
        }
    }
}
