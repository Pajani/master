using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cFCMReturn
    {
       /* {"StatusID":0,"StatusMsg":"Message Saved and Sent. ","returnID":108,"DesctoDev":
        "{\"multicast_id\":6773344932670437281,\"success\":1,\"failure\":0,\"canonical_ids\":0,\"results\":[{\"message_id\":\"0:1486282818623754%b7fa2969b7fa2969\"}]}","userType":null,"OTP":0}
        */
        public Int64 multicast_id { get; set; }
        public int success { get; set; }

        public int failure { get; set; }
        public int canonical_ids { get; set; }
        public List<msgid> results { get; set; }


    }
    public class msgid
    {
        public string message_id { get; set; }
    }
}