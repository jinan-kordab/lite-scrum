using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Models
{
    public class AuthResponse
    {
        public string InvalidLogin { get; set; }

        public string NickName { get; set; }
        public string Email { get; set; }
    }
}