﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleAPI.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FullName { get; set; } = null!;
    
    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }
    //[JsonIgnore]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
