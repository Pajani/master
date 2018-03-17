using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cAreaName
    {
       public List<string> AreaNames;
    }

    public class cguestModeStatus
    {
        public List<string> AreaNames { get; set; }
        public cStatus status { get; set; }
    }

    public class cPermanentPostcode
    {
        public string AreaName { get; set; }
        public string  csubscribe { get; set; }
    }

}