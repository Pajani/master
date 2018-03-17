using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    [Table("dbo.tbl_FCM")]
    public class ctblFCM
    {
        [Key]
        public int KEY_ID { get; set; }
        public string SERVER_API_KEY { get; set;}
        public string PROJECT_KEY { get; set; }
        public string PACKAGE_NAME { get; set; }

    }
}