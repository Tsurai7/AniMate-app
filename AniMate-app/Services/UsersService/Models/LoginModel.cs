﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniMate_app.Services.UsersService.Models
{
    internal class LoginModel
    {
        public string? Email { get; set; }
        public string? Password { get; set; }
    }
}