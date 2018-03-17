using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cRestrictedReceiver
    {
        public int RestricteduserID { get; set; }
        public int SenderID { get; set; }

        public String ReceiverName { get; set; }
        public String ReceiverPhoneno { get; set; }
        public String RecUserType { get; set; }
        public String RecGroup { get; set; }
        public String RecStatus { get; set; }
        public String RecActive { get; set; }


    }
}