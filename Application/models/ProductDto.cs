﻿namespace Application.Models;

public sealed record ProductDto(
    Guid Id,
    string Title,
    int InventoryCount,
    decimal Price,
    decimal Discount)
{
    public decimal FinalPrice => Price - (Price * Discount / 100);
}
