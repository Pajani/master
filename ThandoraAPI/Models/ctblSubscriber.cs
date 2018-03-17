using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class ctblSubscriber
    {
        [Key]
        public int SubscribeID { get; set; }
        public int ReceiverID { get; set; }
        public string ReceiverUserType { get; set; }
        public List<cSender> Senders;
    }

    public class cSender
    {
      
        public int SenderID { get; set; }
        //public DateTime dos{get;set;} 
        public string subscribeonoff { get; set; }
     
    }
}