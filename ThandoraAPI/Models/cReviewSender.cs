using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cReviewSender
    {
        [Key]
        public int reviewerID { get; set; }
        public string RevUserType { get; set; }
        public int SenderID { get; set; }
        public string ReviewComments { get; set; }

    }
}