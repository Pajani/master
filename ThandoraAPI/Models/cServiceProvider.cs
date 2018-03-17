using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cServiceProvider
    {
        public string cServiceCategory { get; set; }
        public string cSvcDesc { get; set; }
        public Double cDisplayOrder { get; set; }
        public int cCount { get; set; }
        public int cServicetypeid { get; set; }
        public List<cPartialSender> providers;


    }
    
}