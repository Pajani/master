using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class ctblMessage
    {
        [Key]
        public int MessageID { get; set; }
        public int SenderID { get; set; }
        public string userType { get; set; }
        public string msgCategory { get; set; }
        public string msgContent { get; set; }
        public string imagePath { get; set; }
        public string msgDistributedTo { get; set; }
        //public string msgResRecGroup { get; set; }
    }

    public class cmsgOverallDistributed
    {
        public string msgDistributedto { get; set; }
        public string newFlag { get; set; }
        public int cnt { get; set; }
        public List<cmsgforOthers> msgCategoryList { get; set; }
    }

    public class cmsgforOthers
    {
        public string msgDistributedto { get; set; }
        public string msgCategory { get; set; }
        public int cnt { get; set; }
        public string newFlag { get; set; }
        public List<cmsgSender> msgSenderList { get; set; }

    }
    public class cmsgSender
    {
        public string msgDistributedto { get; set; }
        public string msgCategory { get; set; }
        public string sendername { get; set; }
        public string senderusertype { get; set; }
        public int senderid { get; set; }
        public string senderSvc { get; set; }
        public string logopath { get; set; }
        public string newFlag { get; set; }
        public int cnt { get; set; }


    }
    public class cmsgPostReturn
    {
        public int retValue  { get; set; }
        public string sendername { get; set; }
        public int senderID { get; set; }
        public string POSTCODE { get; set; }
        public string broadCast { get; set; }
        public string msgImagePath { get; set; }
        public string msgContent { get; set; }
        public string msgCategory { get; set; }
        public string msgDistributedTo { get; set; }


    }
}