using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cPartialSender
    {
            public int SenderID { get; set; }
            public string SenderName { get; set; }
            public string SenderContactNo_1 { get; set; }
            public string SenderContactNo_2 { get; set; }
            public Int16 ContactHide { get; set; }
            public string cServiceType { get; set; }
            public string ServiceDesc { get; set; }
            public string logopath { get; set; }
            public string subscribeonoff { get; set; }
            public int ReviewReceived { get; set; }
            public int msgCommentsReceived { get; set; }
            public int msgLikeReceived { get; set; }
            public int msgTotpublished { get; set; }
            public int msgReadBy { get; set; }
            public string  isFCMActive { get; set; }
            public string address { get; set; }
            public string postcode { get; set; }
           



    }

    public class cUserProfile
    {
        public int userID { get; set; }
        public string userName { get; set; }
        public string address { get; set; }
        public string SenderContactNo_1 { get; set; }
        public string userType { get; set; }
        public string ContactHide { get; set; }
        public string doj { get; set; }
        public string postcode { get; set; }
        public string cServiceType { get; set; }
        public string ServiceDesc { get; set; }
        public string isActive { get; set; }
        public string logopath { get; set; }
        public int ReviewReceived { get; set; }
        public int msgCommentsReceived { get; set; }
        public int msgLikeReceived { get; set; }
        public int msgTotpublished { get; set; }
        public int subscribersCnt { get; set; }
        public int msgReadBy { get; set; }
        public string user_level { get; set; }
        public string first_page_announcement { get; set; }

        public List<cUpdatecount> newCountsbyTitle { get; set; }
        public List<cSuggestUsers> SuggUsers { get; set; }
    }

    public class cUpdatecount
    {
        public string title { get; set; }
        public int count { get; set; }
        public List<allids> ids { get; set; }
    }

    public class cSuggestUsers
    {
        public string userID { get; set; }
        public string userName { get; set; }
        public string userType { get; set; }
        public string ServiceDesc { get; set; }
        public string logopath { get; set; }
        public int followerscnt { get; set; }

    }


    public class allids
    {
        public int key { get; set; }
        public string id { get; set; }
        public string type { get; set; }
    }

}