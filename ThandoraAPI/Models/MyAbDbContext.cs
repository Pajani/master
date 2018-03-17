using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class MyAbDbContext : DbContext
    {
        public DbSet<ctblReceiver> Rs_tblReceiver { get; set; }
        public DbSet<ctblSender> Rs_tblSender { get; set; }
        public DbSet<ctblServices> Rs_tblServices { get; set; }
     
        public DbSet<ctblSubscriber> ctblSubscribers { get; set; }

        public DbSet<ctblMessage> ctblMessages { get; set; }
      
        public DbSet<cAuthorization> cAuthorization { get; set; }

        public DbSet<ctblMessageCategory> ctblMessageCategories { get; set; }

        public DbSet<cReviewSender> cReviewSenders { get; set; }

        public System.Data.Entity.DbSet<ThandoraAPI.Models.ctblFCM> ctblFCMs { get; set; }
    }
}