using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class DeviceGroupController : ApiController
    {
        [Route("api/CreatedeviceGroup")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        public string CreateGroup()
        {
            List<string> deviceids = new List<string>();
            deviceids.Add("cF5Ehug_fn4:APA91bFl5d3XN-XKzm_gpQsZeJYAErdbD7DuiylvujNvxR_oodbQN-dFOoW52S1S50E8WFioU9YuPreTyI5quc9lOGjWt0vHiHbkIAKRr9s6ewWfTwNemhin_6eEJ_wzSSTz0fBjz4H9");
            deviceids.Add("dj3-Wf1RfkE:APA91bGFXuAl4wwNA410y1GjPRrefiMnBuyM6AFG_9-qhLY9JQnYKD0Q4bdDMtP0hpHZy3rszADXJlPvWZrLDVP5uUuBs_uFzmW6ko0W1NwtWHdlpvGg_B4yKyI_87uS1pcpIQiEE-Nk");       
            string SERVER_API_KEY = "AAAAPYj679o:APA91bEKN3E2csYZVghk3DpyHbVWn43gzTbYZ_z8rjIoDuVxWpALbHL_v6OEx8RQTfP5pQ7KD_JNeSJwJmFIx3Aq-OYcq5fid1mYhrF4fgPV3CjEeNFTttIhZxs7158hkUr_WzC_NHqtByTQho0_2wUoiWZt6rY-4Q";
            var SENDER_ID = "264291151834";

           string devicegroup= "APA91bFctukpTd-svShpXaXBfcpNCMY7uvc7EDKHqwyMkBC8U_o7svMkcdmMTJYhfTpZNInpXB2Hopjrlu8IPEp6JjJ36IVlbinqdqcJuC0_NyhCZZKqSJo";
            WebRequest tRequest;
            String sResponseFromServer;
            tRequest = WebRequest.Create("https://android.googleapis.com/gcm/notification");
            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));
            tRequest.Headers.Add(string.Format("project_id:{0}", SENDER_ID));
            tRequest.ContentType = "application/json";

            var postData = new
            {
                operation = "create",
                notification_key_name = "Thandora-603103",
                registration_ids = deviceids

            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(postData);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            tRequest.ContentLength = byteArray.Length;

            try
            {
                Stream dataStream;
                using (dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
                WebResponse tResponse = tRequest.GetResponse();
                dataStream.Close();
                dataStream = tResponse.GetResponseStream();
                StreamReader tReader = new StreamReader(dataStream);
                sResponseFromServer = tReader.ReadToEnd();
                tReader.Close();
                dataStream.Close();
                tResponse.Close();
            }
            catch (Exception ex)
            {
                sResponseFromServer = ex.Message.ToString();
            }
            return (sResponseFromServer);
        }


        [Route("api/CreateTopicGroup")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        public async Task<string> CreateTopicGroup(string SenderTokenID,string SenderTopicName)
        {
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

           // string token = "euGW0vssj4c:APA91bGAL3ks7gOTG6c8NIMfw698cwT2YQLO9iXEM_OCLgv7EdtNVg2m_xFye7WL056tPFtV_S8NqMT-rv_zDh8sg0YZXJMqhzo4UaUu553gXdcRy5XssoB63z81VmXrmDPyN36raY3g";
           // SenderTokenID = token;
            // string SERVER_API_KEY = "AAAAPYj679o:APA91bEKN3E2csYZVghk3DpyHbVWn43gzTbYZ_z8rjIoDuVxWpALbHL_v6OEx8RQTfP5pQ7KD_JNeSJwJmFIx3Aq-OYcq5fid1mYhrF4fgPV3CjEeNFTttIhZxs7158hkUr_WzC_NHqtByTQho0_2wUoiWZt6rY-4Q";
            //var SENDER_ID = "264291151834";
           // string SERVER_API_KEY = fcmDetails.SERVER_API_KEY;
            //var SENDER_ID = fcmDetails.PROJECT_KEY;
            WebRequest tRequest;
            String sResponseFromServer;
            tRequest = WebRequest.Create("https://iid.googleapis.com/iid/v1/"+ SenderTokenID + "/rel/topics/"+ SenderTopicName);
            tRequest.Method = "post";
          
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmDetails.SERVER_API_KEY));
            tRequest.Headers.Add(string.Format("project_id:{0}", fcmDetails.PROJECT_KEY));
            tRequest.ContentType = "application/json";

        
            tRequest.ContentLength = 0;

            try
            {
                tRequest.GetRequestStream();
               /* Stream dataStream;

                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream stream = tResponse.GetResponseStream())
                    {
                        // XmlTextReader reader = new XmlTextReader(stream);
                        // var json = serializer.Serialize(stream);

                        dataStream = tResponse.GetResponseStream();
                        StreamReader tReader = new StreamReader(dataStream);
                        sResponseFromServer = tReader.ReadToEnd();
                        tReader.Close();
                        dataStream.Close();
                        tResponse.Close();
                    }
                }*/


            }
            catch (Exception ex)
            {
                sResponseFromServer = ex.Message.ToString();
            }
            sResponseFromServer =await CheckTopicGroup(SenderTokenID, SenderTopicName);

            return sResponseFromServer;
        }

        [Route("api/CheckTopicGroup")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        public async Task<string> CheckTopicGroup(string SenderTokenID, string SenderTopicName)
        {
           //  string token = "euGW0vssj4c:APA91bGAL3ks7gOTG6c8NIMfw698cwT2YQLO9iXEM_OCLgv7EdtNVg2m_xFye7WL056tPFtV_S8NqMT-rv_zDh8sg0YZXJMqhzo4UaUu553gXdcRy5XssoB63z81VmXrmDPyN36raY3g";
           // SenderTokenID = token;

            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);


           // string SERVER_API_KEY = "AAAAPYj679o:APA91bEKN3E2csYZVghk3DpyHbVWn43gzTbYZ_z8rjIoDuVxWpALbHL_v6OEx8RQTfP5pQ7KD_JNeSJwJmFIx3Aq-OYcq5fid1mYhrF4fgPV3CjEeNFTttIhZxs7158hkUr_WzC_NHqtByTQho0_2wUoiWZt6rY-4Q";
           // var SENDER_ID = "264291151834";
            //string SERVER_API_KEY = fcmDetails.SERVER_API_KEY;
            //var SENDER_ID = fcmDetails.PROJECT_KEY;

            WebRequest tRequest;
            String sResponseFromServer;
            tRequest = WebRequest.Create("https://iid.googleapis.com/iid/info/"+SenderTokenID+"?details=true");
            tRequest.Method = "get";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmDetails.SERVER_API_KEY));
            tRequest.Headers.Add(string.Format("project_id:{0}", fcmDetails.PROJECT_KEY));
            tRequest.ContentType = "application/json";
            //tRequest.UseDefaultCredentials = true;

            var postData = new
            {
                operation = "create",
                notification_key_name = "Thandora-603103",
               

            };
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(postData);
            Byte[] byteArray = Encoding.UTF8.GetBytes(json);
            tRequest.ContentLength = 0;

            try
            {
                Stream dataStream;

                using (WebResponse tResponse = tRequest.GetResponse())
                {
                    using (Stream stream = tResponse.GetResponseStream())
                    {
                       // XmlTextReader reader = new XmlTextReader(stream);
                       // var json = serializer.Serialize(stream);

                        dataStream = tResponse.GetResponseStream();
                        StreamReader tReader = new StreamReader(dataStream);
                        sResponseFromServer = tReader.ReadToEnd();
                        tReader.Close();
                        dataStream.Close();
                        tResponse.Close();
                        if (sResponseFromServer.Contains(SenderTopicName))
                        {
                            sResponseFromServer = "Topic Created/Subscribed in the Name of " + SenderTopicName;
                        }
                    }
                }
               // tRequest.GetRequestStream();
                
              //  WebResponse tResponse = tRequest.GetResponse();
              
            }
            catch (Exception ex)
            {
                sResponseFromServer = ex.Message.ToString();
            }
            return (sResponseFromServer);
        }


    }

    



}
