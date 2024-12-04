﻿using InventoryService.Models.Product;
using Microsoft.EntityFrameworkCore;


namespace InventoryService.Data;

public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }


}
