using Core.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.EntitiesConfiguration
{
    class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        /// <inheritdoc />
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasOne(x => x.AppUser)
                .WithOne(x => x.Address)
                .HasForeignKey<Address>(a => a.AppUserId);
        }
    }
}
