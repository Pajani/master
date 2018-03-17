using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    [Table("[dbo].[tbl_MessageCategory]")]
    public class ctblMessageCategory
    {
        [Key]
        public int ID  { get; set; }
        public string cMessageCategory { get; set; }
        public Double cDisplayOrder { get; set; }
        public string userType { get; set; }
    }
}