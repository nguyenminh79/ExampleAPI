using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ExampleAPI.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public decimal Price { get; set; }
    //[JsonIgnore]

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
