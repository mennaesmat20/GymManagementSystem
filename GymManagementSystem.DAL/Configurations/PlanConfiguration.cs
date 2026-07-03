using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations
{
    public class PlanConfiguration : IEntityTypeConfiguration<Plan>
    {
        public void Configure(EntityTypeBuilder<Plan> builder)
        {
            builder.Property(p => p.Name)
                    .HasColumnType("varchar")
                    .HasMaxLength(50);

            builder.Property(p => p.Description)
                    .HasMaxLength(200);

            builder.Property(p => p.Price)
                    .HasPrecision(10, 2);

            builder.Property(p => p.CreatedAt)
                   .HasDefaultValueSql("GetDate()");

            builder.ToTable(tb =>
            {
                tb.HasCheckConstraint("PlanDurationCheck", "DurationDays Between 1 and 365");
            });

            builder.HasData(
                new Plan
                {
                    Id = 1,
                    Name = "Basic Plan",
                    DurationDays = 30,
                    Price = 300,
                    Description = "Access to gym equipment during staffed hours",
                    IsActive = false
                },

                new Plan
                {
                    Id = 2,
                    Name = "Standard Plan",
                    Description = "Includes gym equipment and 2 group classes per week",
                    DurationDays = 60,
                    Price = 500,
                    IsActive = true
                },

                new Plan
                {
                    Id = 3,
                    Name = "Premium Plan",
                    Description = "Unlimited access to equipment, classes, and sauna",
                    DurationDays = 90,
                    Price = 900,
                    IsActive = true
                },

                new Plan
                {
                    Id = 4,
                    Name = "Annual Plan",
                    Description = "Full year access with personal trainer sessions",
                    DurationDays = 365,
                    Price = 3000,
                    IsActive = false
                }
            );

        }
    }
}