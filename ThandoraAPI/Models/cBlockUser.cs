using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cBlockUser
    {
        public int ReceiverID { get; set; }
        public string RecUserType { get; set; }
        public int BlockedID { get; set; }
        public string BlockedUserType { get; set; }
        public int MessageID { get; set; }


    }
}