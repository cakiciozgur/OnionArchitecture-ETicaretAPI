﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.DTOs.User
{
    public class CreateUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}