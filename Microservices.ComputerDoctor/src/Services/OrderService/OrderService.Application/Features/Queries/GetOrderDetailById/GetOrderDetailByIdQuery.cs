using MediatR;
using OrderService.Application.Features.Queries.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Features.Queries.GetOrderDetailById
{
    public class GetOrderDetailByIdQuery : IRequest<OrderDetailViewModel>
    {
        public Guid OrderId { get; set; }

        public GetOrderDetailByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
