using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cAllMessages
    {
        public string msgBroadcast { get; set; }
        public int count { get; set; }
        public List<ctblReadMessage> listMessage { get; set; }
    }
    public class cMessageSepration
    {

        public int count { get; set; }
        public List<ctblReadMessage> listMessages { get; set; }
    }


    public class ctblReadMessage
    {
        public int MessageID { get; set; }
        public int SenderID { get; set; }
        public string SenderName { get; set; }
        public string SenderuserType { get; set; }
        public Int16 contactHide { get; set; }
        public string SenderContactNo_1 { get; set; }
        public string logopath { get; set; }
        public string msgCategory { get; set; }
        public string msgPublished { get; set; }
        public DateTime msgUpdated { get; set; }
        public string msgContent { get; set; }
        public string imagePath { get; set; }
        public int likeCnt { get; set; }
        public int readCnt { get; set; }
        public int cmntCnt { get; set; }
        public string Likeflag { get; set; }
        public string Readflag { get; set; }
        public string Favflag { get; set; }
        public string expiredFlag { get; set; }

        public string msgDistributedto { get; set; }
        public List<cmsgLikedBy> PeopleLikedby { get; set; }
        public List<cmsgCommentedBy> PeopleCommentedby { get; set; }
    }

    public class cmsgLikedBy
    {
        public string likerName { get; set; }
        public int likerID { get; set; }
        public string logopath { get; set; }
        public string likedTime { get; set; }
     }

    public class cmsgCommentedBy
    {
        public string commenterName { get; set; }
        public int commenterID { get; set; }
        public string comments { get; set; }
        public string logopath { get; set; }
        public string commentTime { get; set; }
        public int commentID { get; set; }
        public string is_delete_but_visible { get; set; }


    }
}