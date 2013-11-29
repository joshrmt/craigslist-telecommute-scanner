using System;
using System.Data.Entity;

namespace CraigslistScanner.Data {
    public class CraigslistDatabase : DbContext {

        public static String ConnectionString { get; set; }

        public CraigslistDatabase()
            : base(GetConnectionString())
        {
        }

        public static String GetConnectionString() {
            return ConnectionString ?? "[Your connection string]";
        }

        public DbSet<Job> Jobs { get; set; }
    }
}
