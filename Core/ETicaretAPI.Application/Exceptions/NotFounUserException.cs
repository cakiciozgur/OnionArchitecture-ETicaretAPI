﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Exceptions
{
    public class NotFounUserException : Exception
    {
        public NotFounUserException() : base("Kullanıcı Adı veya şifre hatalı")
        {
        }

        public NotFounUserException(string? message) : base(message)
        {
        }

        public NotFounUserException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
