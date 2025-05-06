// using Domain.Entities;
// using Microsoft.EntityFrameworkCore;

// namespace Infrastructure.Persistence.Data.Context;
// public class DeploymentManagerContext : DbContext
// {
//     public DbSet<AppEntity> Apps { get; set; }

//     public DbSet<CustomerEntity> Customers { get; set; }

//     public DbSet<DeploymentEntity> Deployments { get; set; }

//     public DbSet<ProjectEntity> Projects { get; set; }

//     public DeploymentManagerContext(DbContextOptions<DeploymentManagerContext> options) : base(options) { }

//     protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//     {
//         optionsBuilder.UseSqlServer("Server=.;Database=DeploymentManager;Trusted_Connection=True;");
//     }

//     protected override void OnModelCreating(ModelBuilder modelBuilder)
//     {
//         modelBuilder.Entity<AppEntity>().ToTable("Apps");
//         modelBuilder.Entity<CustomerEntity>().ToTable("Customers");
//         modelBuilder.Entity<DeploymentEntity>().ToTable("Deployments");
//         modelBuilder.Entity<ProjectEntity>().ToTable("Projects");
//     }
// }


// //public DbSet<Database> Databases { get; set; }
// //public DbSet<DatabaseServer> DatabaseServers { get; set; }
// //public DbSet<DatabaseType> DatabaseTypes { get; set; }
// //public DbSet<Environment> Environments { get; set; }
// //public DbSet<EnvironmentType> EnvironmentTypes { get; set; }
// //public DbSet<Server> Servers { get; set; }
// //public DbSet<ServerType> ServerTypes { get; set; }
// //public DbSet<Deployment> Deployments { get; set; }
// //public DbSet<DeploymentStatus> DeploymentStatuses { get; set; }
// //public DbSet<DeploymentType> DeploymentTypes { get; set; }
// //public DbSet<DeploymentEnvironment> DeploymentEnvironments { get; set; }
// //public DbSet<DeploymentServer> DeploymentServers { get; set; }
// //public DbSet<DeploymentServerType> DeploymentServerTypes { get; set; }
// //public DbSet<DeploymentServerStatus> DeploymentServerStatuses { get; set; }
// //public DbSet<DeploymentServerEnvironment> DeploymentServerEnvironments { get; set; }
// //public DbSet<DeploymentServerEnvironmentType> DeploymentServerEnvironmentTypes { get; set; }
// //public DbSet<DeploymentServerEnvironmentStatus> DeploymentServerEnvironmentStatuses { get; set; }
// //public DbSet<DeploymentServerEnvironmentType> DeploymentServerEnvironmentTypes { get; set; }
// //public DbSet<DeploymentServerEnvironment> DeploymentServerEnvironments { get; set; }
// //public DbSet<DeploymentServerEnvironmentStatus> DeploymentServerEnvironmentStatuses { get; set; }
// //public DbSet<DeploymentServerEnvironmentType> DeploymentServerEnvironmentTypes { get; set; }
// //public DbSet<DeploymentServerEnvironment> DeploymentServerEnvironments { get; set; }
// //public DbSet<DeploymentServerEnvironmentStatus> DeploymentServerEnvironmentStatuses { get; set; }
// //public DbSet<DeploymentServerEnvironmentType> DeploymentServerEnvironmentTypes { get; set; }
// //public DbSet<DeploymentServerEnvironment> DeploymentServerEnvironments { get; set; }
// //public DbSet<DeploymentServerEnvironmentStatus> DeploymentServerEnvironmentStatuses { get; set; }
// //public DbSet<DeploymentServerEnvironmentType> DeploymentServerEnvironmentTypes { get; set; }
// //public DbSet<DeploymentServerEnvironment> DeploymentServerEnvironments { get; set;}



