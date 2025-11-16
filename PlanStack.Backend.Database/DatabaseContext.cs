using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PlanStack.Backend.Database.DataModels;

namespace PlanStack.Backend.Database
{
    public class DatabaseContext : IdentityDbContext<User>
    {
        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = "b2e8c3e2-8db9-4c4c-b4f2-879b2f41f001", Name = "User", NormalizedName = "USER" },
                new IdentityRole { Id = "3da92c92-794e-4624-809e-77198ed828b9", Name = "Admin", NormalizedName = "ADMIN" }
            );

            base.OnModelCreating(modelBuilder);

            // Setup Cascade Deletions
            #region Blueprint Relations
            modelBuilder.Entity<BlueprintBuildingStructure>()
                .HasOne(bbs => bbs.Blueprint)
                .WithMany(b => b.BuildingStructures)
                .HasForeignKey(bbs => bbs.BlueprintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlueprintBuildingStructure>()
                .HasOne(bbs => bbs.BuildingStructure)
                .WithMany(bs => bs.BlueprintBuildingStructures)
                .HasForeignKey(bbs => bbs.BuildingStructureId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlueprintStandard>()
                .HasOne(bs => bs.Blueprint)
                .WithMany(b => b.Standards)
                .HasForeignKey(bs => bs.BlueprintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlueprintStandard>()
                .HasOne(bs => bs.Standard)
                .WithMany(s => s.BlueprintStandards)
                .HasForeignKey(bs => bs.StandardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlueprintComponent>()
                .HasOne(bc => bc.Blueprint)
                .WithMany(b => b.Components)
                .HasForeignKey(bc => bc.BlueprintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BlueprintComponent>()
                .HasOne(bc => bc.Component)
                .WithMany(c => c.BlueprintComponents)
                .HasForeignKey(bc => bc.ComponentId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion

            #region Standard Relations
            modelBuilder.Entity<StandardRuleSet>()
                .HasOne(sr => sr.Standard)
                .WithMany(r => r.RuleSets)
                .HasForeignKey(sr => sr.StandardId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StandardRuleSet>()
                .HasOne(sr => sr.RuleSet)
                .WithMany(r => r.Standards)
                .HasForeignKey(sr => sr.RuleSetId)
                .OnDelete(DeleteBehavior.Cascade);
            #endregion
        }

        #region DbSets
        public DbSet<Blueprint> Blueprints { get; set; }
        public DbSet<BlueprintBuildingStructure> BlueprintBuildingStructures { get; set; }
        public DbSet<BlueprintComponent> BlueprintComponents { get; set; }
        public DbSet<BlueprintStandard> BlueprintStandards { get; set; }

        public DbSet<BuildingStructure> BuildingStructures { get; set; }
        public DbSet<Component> Components { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<RuleSet> RuleSets { get; set; }
        public DbSet<Standard> Standards { get; set; }
        public DbSet<Room> Rooms { get; set; }
        #endregion
    }
}
