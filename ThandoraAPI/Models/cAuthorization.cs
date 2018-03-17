using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cAuthorization
    {
        [Key]
        public string deviceID { get; set; }
        public string purpose { get; set; }
        public int AuthorizeKey { get; set; }
        public string verified { get; set; }
        public string phoneNo { get; set; }
    }

    public class cAllAdvertisements
    {
      
        public string SenderName { get; set; }
        public string PhoneNo { get; set; }
        public string imagePath { get; set; }
        public DateTime datePublised { get; set; }
  
    }


}