using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TAMS.Models;

namespace TAMS.Models
{
    public class TAMSContext : DbContext
    {
        public TAMSContext(DbContextOptions<TAMSContext> options) : base(options)
        {

        }
        public DbSet<AttendanceLog> AttendanceLogs { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Level> Levels { get; set; }
        public DbSet<EmployeeType> EmployeeTypes { get; set; }
        public DbSet<Cluster> Clusters { get; set; }
        public DbSet<ClusterUser> ClusterUsers { get; set; }
        

        public DbSet<ScreeningToolData> ScreeningToolData { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(p => new { p.Username,p.Status })
                .IsUnique();

            //modelBuilder.Entity<AttendanceLog>()
            //   .HasIndex(p => new { p.EmployeeId,p.CreatedDate, p.Status })
            //   .IsUnique();




        }



    }
}
