﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace A2.Models
{
    public class Order
    {
        public string UserName { get; set; }
        public long ProductId { get; set; }
    }
}