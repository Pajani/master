using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cReadOtherMessagesforSender
    {
        public int ReceiverID { get; set; }
        public string RecUserType { get; set; }
        public int SenderId { get; set; }
        public string senderUsertype { get; set; }
        public string msgCategory { get; set; }
        public string msgDistributedTo { get; set; }
        public string PostCode { get; set; }

    }
}