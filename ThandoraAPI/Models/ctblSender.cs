
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ThandoraAPI.Models
{
    [Table("dbo.tbl_Sender")]
    public class ctblSender
    {
        [Key]
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderPhone { get; set; }
        public string Address { get; set; }
        public string SenderContactNo_1 { get; set; }
        public string SenderContactNo_2 { get; set; }
        public Int16 ContactHide { get; set; }
        public string deviceTokenID { get; set; }
        public string SIMNO { get; set; }
        public string POSTCODE { get; set; }
        public string cServiceType { get; set; }
        public string ServiceDesc { get; set; }
        public string logopath { get; set; }
        public string ActiveUser { get; set; }
    }
}