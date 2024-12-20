﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vouchee.Data.Models.DTOs;

namespace Vouchee.Business.Models
{
    public class AuthResponse
    {
        public AuthResponse()
        {
            deviceTokens = [];
        }

        public string? id { get; set; }
        public string? role { get; set; }
        public string? uid { get; set; }
        public string? name { get; set; }
        public string? email { get; set; }
        public string? image { get; set; }
        public string? phoneNumber { get; set; }
        public string? accessToken { get; set; }
        public IList<GetDeviceTokenDTO> deviceTokens { get; set; }
    }
}
