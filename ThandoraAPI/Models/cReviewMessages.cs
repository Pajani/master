using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cReviewMessages
    {
        public int reviewerID { get; set; }
        public string ReviewerName { get; set; }
        public string RevUserType { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public string ReviewComments { get; set; }
        public DateTime ReviewDate { get; set; }

    }
}