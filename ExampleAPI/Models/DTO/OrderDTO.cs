﻿namespace ExampleAPI.Models.DTO
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int CustomerId { get; set; }

        public DateTime? OrderDate { get; set; }

        public decimal? TotalAmount { get; set; }

    }
}
