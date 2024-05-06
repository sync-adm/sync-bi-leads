using Microsoft.EntityFrameworkCore;
using Sync.Bi.Leads.Companies;
using Sync.Bi.Leads.Leads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sync.Bi.Leads
{
    public class LeadDbContext : DbContext
    {

        public DbSet<Lead> Leads { get; set; }

        public DbSet<Company> Companies { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlServer("Server=sync-corp.database.windows.net; Database=SyncBiLeads;Trusted_Connection=False;TrustServerCertificate=True; User Id=syncsa;Password=zudkKCv%T7*;");

        }
      
    }
}
