using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cSMSReponse
    {
        /*{"ErrorCode":"000","ErrorMessage":"Success","JobId":"6121048","MessageData":[{"Number":"919600075586","MessageParts":[{"MsgId":"919600075586-749a062f0baf4e1da9a5c48e70f4f083","PartId":1,"Text":"Dear PAJANI, Welcome to my App name. Please enter this 5576 verification code in android app."}]}]}*/

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string JobId { get; set; }
        public msgData MessageData { get; set; }

    }

    public class msgData
    {
        public string Number { get; set; }
        public msgpart MessageParts { get; set; }

    }

    public class msgpart
    {
        public string MsgId { get; set; }
        public int PartId { get; set; }
        public string Text { get; set; }
        

    }
}