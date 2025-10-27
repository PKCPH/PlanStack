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
            //modelBuilder.Entity<IdentityRole>().HasData(
            //    new IdentityRole
            //    {
            //        Id = "b2e8c3e2-8db9-4c4c-b4f2-879b2f41f001",
            //        Name = "User",
            //        NormalizedName = "USER"
            //    },
            //    new IdentityRole
            //    {
            //        Id = "2b5fbc8a-1b5c-4e09-bf5e-9e0b0b41e002",
            //        Name = "Admin",
            //        NormalizedName = "ADMIN"
            //    }
            //);

            base.OnModelCreating(modelBuilder);
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
        public DbSet<StandardRuleSet> StandardRuleSets { get; set; }
        #endregion
    }
}
