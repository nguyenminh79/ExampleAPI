﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleAPI.Models;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public DateTime? OrderDate { get; set; }

    public decimal? TotalAmount { get; set; }
    [JsonIgnore]
    public virtual Customer Customer { get; set; } = null!;
    [JsonIgnore]
    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
