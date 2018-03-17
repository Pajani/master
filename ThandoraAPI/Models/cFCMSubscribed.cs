using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cFCMSubscribed
    {
        /* {
             "applicationVersion": "1",
             "connectDate": "2017-02-09",
             "attestStatus": "NOT_ROOTED",
             "application": "com.thandora.android",
             "scope": "*",
             "authorizedEntity": "391118300920",
              "connectionType": "WIFI",
              "appSigner": "ddaf5061e328bcb080877fd40b5124f8b218c464",
             "platform": "ANDROID",
             "rel": 
                {
                     "topics": {
                                 "100008": {"addDate": "2017-02-06" },
                                 "100089": { "addDate": "2017-02-06"}
                                 "103349": { "addDate": "2017-02-06"}
                                 "102349": { "addDate": "2017-02-06"}
                             }
                 }

         }*/

        /*
        {
"applicationVersion": "sample string 1",
"connectDate": "sample string 2",
"attestStatus": "sample string 3",
"application": "sample string 4",
"scope": "sample string 5",
"authorizedEntity": "sample string 6",
"connectionType": "sample string 7",
"appSigner": "sample string 8",
"platform": "sample string 9",
"rel": {
"topics": [
  {
    "tdate": {
      "addDate": "sample string 1"
    }
  },
  {
    "tdate": {
      "addDate": "sample string 1"
    }
  }
]
}
} 

         * */


        /*
        {
"applicationVersion": "sample string 1",
"connectDate": "sample string 2",
"attestStatus": "sample string 3",
"application": "sample string 4",
"scope": "sample string 5",
"authorizedEntity": "sample string 6",
"connectionType": "sample string 7",
"appSigner": "sample string 8",
"platform": "sample string 9",
"rel": {
"topics": {
  "tdate": [
    {
      "addDate": "sample string 1"
    },
    {
      "addDate": "sample string 1"
    }
  ]
}
}
} 

         */

        /* {
"applicationVersion": "sample string 1",
"connectDate": "sample string 2",
"attestStatus": "sample string 3",
"application": "sample string 4",
"scope": "sample string 5",
"authorizedEntity": "sample string 6",
"connectionType": "sample string 7",
"appSigner": "sample string 8",
"platform": "sample string 9",
"rel": {
 "topics": 
 {
   "tdate": {"addDate": "sample string 1"   }
 }
}
}*/

        public string applicationVersion { get; set; }
        public string connectDate { get; set; }
        public string attestStatus { get; set; }
        public string application { get; set; }
        public string scope { get; set; }
        public string authorizedEntity { get; set; }
        public string connectionType { get; set; }
        public string appSigner { get; set; }
        public string platform { get; set; }

        public topicsname rel { get; set; }
    }
    public class topicsname
    {
        // public List<topicname> topics { get; set; }
        //List<Tuple<Section, SubSection, Slide>>();
        public List<Tuple<topicname>> topics { get; set; }
    }
    
    public class topicdate
    {
        public string addDate { get; set; }

    }

    public class topicname
    {

        public topicdate tdate { get; set; }

    }
}