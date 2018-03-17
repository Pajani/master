using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    [Table("[dbo].[tbl_Services]")]
    public class ctblServices
    {
        [Key]
        public int ID { get; set; }
        public string cServiceType { get; set; }

        public string cSvcDesc {get;set; }
        public Double cDisplayOrder {get;set; }
        public string cLanguage {get;set; }

    }
}