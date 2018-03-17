using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cGuestMode
    {
        public int userID { get; set; }
        public string userType { get; set; }
        public string gPOSTCODE { get; set; }
        public string is_guest_mode { get; set; }
    }
}