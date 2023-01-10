using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Exceptions
{
    public class PaymentMethodDomainException : Exception
    {
        public PaymentMethodDomainException()
        {
        }

        public PaymentMethodDomainException(string? message) : base($"Invalid card informations are given: " + message)
        {
        }

        public PaymentMethodDomainException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
