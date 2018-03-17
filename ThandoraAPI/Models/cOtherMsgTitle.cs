using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cOtherMsgTitle
    {

        public string msgCategory { get; set; }
        public int count { get; set; }
        public string  newmsgFlag { get; set; }
        public Double cDisplayorder { get; set; }
        public List<ctblReadMessage> lstmessages { get; set; }
    }
}