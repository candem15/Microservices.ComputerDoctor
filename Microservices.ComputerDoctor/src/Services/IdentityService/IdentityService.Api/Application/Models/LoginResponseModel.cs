using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityService.Api.Application.Models
{
    public class LoginResponseModel
    {
        public string Username { get; set; }
        public Token Token { get; set; }
    }
}