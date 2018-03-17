
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThandoraAPI.Models
{
    [Table("dbo.tbl_Receiver")]
    public class ctblReceiver
    {
        [Key]
        public int ReceiverID { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverPhone { get; set; }
        
        public string Address {get; set; }
        //public string doj {get; set; }
        public string deviceTokenID { get; set; }
        public string SIMNO { get; set; }
        public string POSTCODE { get; set; }
        public string ActiveUser { get; set; }
    }
}