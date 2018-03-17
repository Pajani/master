using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ThandoraAPI.Models
{
    public class cPostCodeDistance
    {
        public string destination_addresses { get; set; }
        public string origin_addresses { get; set; }
        public string status { get; set; }
        public List<crow> rows { get; set; }


        /* {
   "destination_addresses": [
     "Tamil Nadu 603103, India"
   ],
   "origin_addresses": [
     "605008, India"
   ],
    "status": "OK"
   "rows": [
     {
       "elements": [
         {
           "distance": {
             "text": "116 km",
             "value": 115754
           },
           "duration": {
             "text": "2 hours 11 mins",
             "value": 7883
           },
           "status": "OK"
         }
       ]
     }
   ]
  
 }*/
    }

    public class crow
    {
        public List<celements> elements { get; set; }
    }

    public class celements
    {
        public string status { get; set; }
        public cdistance distance { get; set; }
        public cduration duration { get; set; }
    }
    public class cdistance
    {
        public string text { get; set; }
        public string value { get; set; }
    }
    public class cduration
    {
        public string text { get; set; }
        public string value { get; set; }
    }
}