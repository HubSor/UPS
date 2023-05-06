﻿using System;

namespace Models.Entities
{
    public class Product : Entity
    {
        public string Name { get; set; } = default!;
        public DateTime CreatedAt { get; set; }
    }
}
