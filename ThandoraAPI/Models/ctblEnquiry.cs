using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class ctblEnquiry
    {
        [Key]
        public int EnquiryID { get; set; }
        public string EnquiryMessage { get; set; }

        public int AnswerID { get; set; }
        public int ToUserID { get; set; }
        public string ToUserType { get; set; }
        public string ToUserName { get; set; }
        public string touserImage { get; set; }
        public int FromUserID { get; set; }
        public string FromUserType { get; set; }
        public string FromUserName { get; set; }
        public string FromUserImage { get; set; }
        public string newflag { get; set; }
        public string doc { get; set; }

    }
}