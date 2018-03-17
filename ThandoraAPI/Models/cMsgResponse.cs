using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cMsgResponse
    {
        public int userID { get; set; }
        public string userType { get; set; }
        public int MessageID { get; set; }
        public int like { get; set; }
        public string favflag { get; set; }
        public string delFlag { get; set; }
        public string comments { get; set; }
       
    }

    public class cFeedback
    {
        public int userID { get; set; }
        public string userType { get; set; }
        public string Message { get; set; }

    }


}