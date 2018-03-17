using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cSearch
    {
        public int userID { get; set; }
        public string userType { get; set; }
        public string what { get; set; }
        public string where { get; set; }
        public decimal lat { get; set; }
        public decimal lng { get; set; }
        public string latlngstatus { get; set; }

    }
}