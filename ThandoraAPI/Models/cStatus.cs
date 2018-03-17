using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cStatus
    {
        public int StatusID { get; set; }
        public string StatusMsg { get; set; }
        public int returnID { get; set; }
        public string DesctoDev { get; set; }
        public string userType { get; set; }
        public int OTP { get; set; }

    }
}