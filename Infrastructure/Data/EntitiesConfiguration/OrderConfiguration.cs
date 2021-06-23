using System;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntitiesConfiguration
{
   public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShipAddress, a => a.WithOwner());
            builder.Property(s => s.OrderStatus).HasConversion(o => o.ToString(),
                o => (OrderStatus) Enum.Parse(typeof(OrderStatus),o));
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);// delete order will delete its item(s)
            builder.Property(x => x.Subtotal).HasColumnType("money");
        }
    }
}
