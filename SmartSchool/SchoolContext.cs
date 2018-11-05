using System.Data.Entity;
using System.Windows.Media;
using SmartSchool.Model;

namespace SmartSchool {
    public class SchoolContext : DbContext {
        public SchoolContext() : base("name:DefaultConnection") {
            Configuration.AutoDetectChangesEnabled = false;
        }

        public DbSet<AccessLog> AccessLogs { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<PurchaseLog> PurchaseLogs { get; set; }
        public DbSet<User> Users { get; set; }
    }
}