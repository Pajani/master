using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class SendNotificationController : ApiController
    {

        [Route("api/SendNotification")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        private string SendNotification(string title, string message)
        {
            List<string> deviceids = new List<string>();

            deviceids.Add("cF5Ehug_fn4:APA91bFl5d3XN-XKzm_gpQsZeJYAErdbD7DuiylvujNvxR_oodbQN-dFOoW52S1S50E8WFioU9YuPreTyI5quc9lOGjWt0vHiHbkIAKRr9s6ewWfTwNemhin_6eEJ_wzSSTz0fBjz4H9");
            deviceids.Add("dj3-Wf1RfkE:APA91bGFXuAl4wwNA410y1GjPRrefiMnBuyM6AFG_9-qhLY9JQnYKD0Q4bdDMtP0hpHZy3rszADXJlPvWZrLDVP5uUuBs_uFzmW6ko0W1NwtWHdlpvGg_B4yKyI_87uS1pcpIQiEE-Nk");
            deviceids.Add("d6Gm0FGVQhs:APA91bE9CGOUs3QWW7N7-oV-58pnXxbqm0MqWLJRydaxlTMJl2_zKs8j5My8qU2jBpesDDmYC8RiNictqRB_i3Iklk6tLml5vUGxDeufbaw3YxndRm5RLMT_LW392qC0djfXqOKJELPr");
            deviceids.Add("ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd");



            string regIds = string.Join("\",\"", deviceids);
            string SERVER_API_KEY = "AAAAPYj679o:APA91bEKN3E2csYZVghk3DpyHbVWn43gzTbYZ_z8rjIoDuVxWpALbHL_v6OEx8RQTfP5pQ7KD_JNeSJwJmFIx3Aq-OYcq5fid1mYhrF4fgPV3CjEeNFTttIhZxs7158hkUr_WzC_NHqtByTQho0_2wUoiWZt6rY-4Q";
            string devicegroup = "APA91bFctukpTd-svShpXaXBfcpNCMY7uvc7EDKHqwyMkBC8U_o7svMkcdmMTJYhfTpZNInpXB2Hopjrlu8IPEp6JjJ36IVlbinqdqcJuC0_NyhCZZKqSJo";

            var SENDER_ID = "264291151834";
            var value = message;
            WebRequest tRequest;
            String sResponseFromServer;
            //tRequest = WebRequest.Create("https://android.googleapis.com/gcm/send");
            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

            //tRequest.UseDefaultCredentials = true;

            //tRequest.PreAuthenticate = true;

            // tRequest.Credentials = new NetworkCredential("pajani.arjunan@gmail.com", "Iniyan@4104");

            //tRequest.Credentials = CredentialCache.DefaultCredentials;

            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", SERVER_API_KEY));

            tRequest.Headers.Add(string.Format("Sender: id={0}", SENDER_ID));


            // string postData = "{\"collapse_key\":\"score_update\",\"time_to_live\":108,\"delay_while_idle\":true,\"data\": { \"message\" : " + message + ",\"time\": " + "\"" + System.DateTime.Now.ToString() + "\"},\"registration_ids\":[\"" + regIds + "\"]}";


            //string postData = "collapse_key=score_update&time_to_live=108&delay_while_idle=1&data.message=" + message + "&data.time=" + System.DateTime.Now.ToString() + "&to=cF5Ehug_fn4:APA91bFl5d3XN-XKzm_gpQsZeJYAErdbD7DuiylvujNvxR_oodbQN-dFOoW52S1S50E8WFioU9YuPreTyI5quc9lOGjWt0vHiHbkIAKRr9s6ewWfTwNemhin_6eEJ_wzSSTz0fBjz4H9";
            // Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            //to = "dj3-Wf1RfkE:APA91bGFXuAl4wwNA410y1GjPRrefiMnBuyM6AFG_9-qhLY9JQnYKD0Q4bdDMtP0hpHZy3rszADXJlPvWZrLDVP5uUuBs_uFzmW6ko0W1NwtWHdlpvGg_B4yKyI_87uS1pcpIQiEE-Nk",
            tRequest.ContentType = "application/json;charset=UTF-8";
            /* var postData = new
             {
                 registration_ids = new string[] {
                     "dj3-Wf1RfkE:APA91bGFXuAl4wwNA410y1GjPRrefiMnBuyM6AFG_9-qhLY9JQnYKD0Q4bdDMtP0hpHZy3rszADXJlPvWZrLDVP5uUuBs_uFzmW6ko0W1NwtWHdlpvGg_B4yKyI_87uS1pcpIQiEE-Nk",
                     "cF5Ehug_fn4:APA91bFl5d3XN-XKzm_gpQsZeJYAErdbD7DuiylvujNvxR_oodbQN-dFOoW52S1S50E8WFioU9YuPreTyI5quc9lOGjWt0vHiHbkIAKRr9s6ewWfTwNemhin_6eEJ_wzSSTz0fBjz4H9"
                 },

                 notification = new
                 {
                     body = message,
                     title = title,
                     icon = "myicon"
                 }
             };*/

            var postData = new
            {
                //to=devicegroup,

                //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",

                to = "/topics/COSMOCITY",
                //registration_ids = deviceids,
                //notification_key_name = "Thandora-603103",
                notification = new
                {
                    body = message,
                    title = title,
                    icon = "myicon"
                }
            };


            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(postData);

            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            //Console.WriteLine(data);
            // Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;


            try
            {
                Stream dataStream;
                //Stream dataStream = tRequest.GetRequestStream();

                using (dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse tResponse = tRequest.GetResponse();
                // dataStream.Write(byteArray, 0, byteArray.Length);
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

            // return sResponseFromServer;
            return (sResponseFromServer);
        }

        [Route("api/PostNotification")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        private async Task<IHttpActionResult> PostNotification(string GroupName, string SenderName, string msgCategory, string message, int MsgID)
        {
            string toGroup;
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            var value = message;
            WebRequest tRequest;
            String sResponseFromServer;

            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmDetails.SERVER_API_KEY));

            tRequest.Headers.Add(string.Format("Sender: id={0}", fcmDetails.PROJECT_KEY));
            tRequest.ContentType = "application/json;charset=UTF-8";
            if (SenderName.Equals("THANDORA ADMIN") == true)
            {
                toGroup = "/topics/THANDORA_ADMIN";
            }
            else
            {
                toGroup = "/topics/" + GroupName;
            }
            var postData = new
            {
                //registration_ids = deviceids,
                //to=devicegroup,
                //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                to = toGroup,
                data = new
                {
                    MessageID = MsgID
                },
                priority = "high",
                notification = new
                {
                    body = message,
                    title = msgCategory + " from " + SenderName,
                    icon = "myicon"
                }
            };


            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(postData);

            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            //Console.WriteLine(data);
            // Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;


            try
            {
                Stream dataStream;
                //Stream dataStream = tRequest.GetRequestStream();

                using (dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse tResponse = tRequest.GetResponse();
                // dataStream.Write(byteArray, 0, byteArray.Length);
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

            // return sResponseFromServer;
            return Ok(sResponseFromServer);
        }

        [Route("api/PostNotificationAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostNotificationAsync(string titlebroadcast, string GroupName, string SenderName, string msgCategory, string message, int MsgID)
        {
            string toGroup, uri;
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);
            string value;
            if (message.Length > 50)
                value = message.Substring(0,40)+"...";
            else
                value = message;


            uri = "https://fcm.googleapis.com/fcm/send";


            if (SenderName.Equals("THANDORA ADMIN") == true)
            {
                toGroup = "/topics/THANDORA_ADMIN";
            }
            else
            {
                toGroup = "/topics/" + GroupName;
            }

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));
                var postData = new
                {
                    //registration_ids = deviceids,
                    //to=devicegroup,
                    //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                    to = toGroup,
                    priority = "high",
                    data = new
                    {
                        //title = titlebroadcast + "/" + msgCategory ,
                        title = titlebroadcast ,
                        message = value,
                        SenderName= SenderName,
                        imageUrl= "",
                         messageId = MsgID,
                         notificationType= "1"  //(1->PostMessage, 2-- > EnquiryReply and so on)
                       
                    },

                    /*notification = new
                    {
                        body = value,
                        title = titlebroadcast + "/" + msgCategory + "/" + SenderName,
                        icon = "myicon"
                    }*/
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(postData);

                //request.AddParameter("application/json", "{\r\n                \"to\" : \""+DeviceID+"\"\r\n                \"data\":{\r\n                \t \"enquiryID\": "+enquiryid.ToString()+"\r\n                }\r\n                \r\n                \"notification\":\r\n                {\r\n                    \"body\": \""+enquirymessage+"\",\r\n                    \"title\":\""+FromUser +"\",\r\n                    \"icon\": \"myicon\"\r\n                }\r\n            }", ParameterType.RequestBody);

                //request.AddParameter("application/json", request.JsonSerializer.Serialize( new {to = DeviceID, data = new {MessageID = enquiryid}, notification = new
                // { body = enquirymessage, title = FromUser,icon = "myicon"  } }) , ParameterType.RequestBody);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                /*var rs = client.Execute(request);
                 sResponseFromServer = rs.Content.ToString();
                 return sResponseFromServer;*/
                //var asyncHandle = await client.ExecuteAsync(request, NotificationOnComplete);

                IRestResponse<cFCMReturn> asyncHandle = await client.ExecutePostTaskAsync<cFCMReturn>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                st.returnID = asyncHandle.Data.success; // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                st.StatusID = asyncHandle.Data.failure; // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                //return asyncHandle.Data.success.ToString();
                return st;

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
                return st;

            }

        }

        [Route("api/PostMsgImageNotificationAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostMsgImageNotificationAsync(string titlebroadcast, string GroupName, string SenderName, string msgCategory, string message, string imgPath, int MsgID,int senderID)
        {
            string toGroup, uri;
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);
            string value;
            if (message.Length > 50)
                value = message.Substring(0, 40) + "...";
            else
                value = message;


            uri = "https://fcm.googleapis.com/fcm/send";


            if (SenderName.Equals("THANDORA ADMIN") == true)
            {
                toGroup = "/topics/THANDORA_ADMIN";
            }
            else
            {
                toGroup = "/topics/" + GroupName;
            }

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));
                var postData = new
                {
                    //registration_ids = deviceids,
                    //to=devicegroup,
                    //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                    to = toGroup,
                    data = new
                    {
                        //title = titlebroadcast + "/" + msgCategory,
                        title = titlebroadcast,
                        message = value,
                        SenderName = SenderName,
                        imageUrl = imgPath,
                        messageId = MsgID,
                        SenderID= senderID,
                        notificationType = "1"  //(1->PostMessage, 2-- > EnquiryReply and so on)

                    },
                    priority= "high"

                    /*notification = new
                    {
                        body = value,
                        title = titlebroadcast + "/" + msgCategory + "/" + SenderName,
                        icon = "myicon"
                    }*/
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(postData);

                //request.AddParameter("application/json", "{\r\n                \"to\" : \""+DeviceID+"\"\r\n                \"data\":{\r\n                \t \"enquiryID\": "+enquiryid.ToString()+"\r\n                }\r\n                \r\n                \"notification\":\r\n                {\r\n                    \"body\": \""+enquirymessage+"\",\r\n                    \"title\":\""+FromUser +"\",\r\n                    \"icon\": \"myicon\"\r\n                }\r\n            }", ParameterType.RequestBody);

                //request.AddParameter("application/json", request.JsonSerializer.Serialize( new {to = DeviceID, data = new {MessageID = enquiryid}, notification = new
                // { body = enquirymessage, title = FromUser,icon = "myicon"  } }) , ParameterType.RequestBody);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                /*var rs = client.Execute(request);
                 sResponseFromServer = rs.Content.ToString();
                 return sResponseFromServer;*/
                //var asyncHandle = await client.ExecuteAsync(request, NotificationOnComplete);

                IRestResponse<cFCMReturn> asyncHandle = await client.ExecutePostTaskAsync<cFCMReturn>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                st.returnID = asyncHandle.Data.success; // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                st.StatusID = asyncHandle.Data.failure; // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                //return asyncHandle.Data.success.ToString();
                return st;

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
                return st;

            }

        }



        [Route("api/PostMessageforEachDeviceAsync")]
        [AllowAnonymous]
        public async Task PostMessageforEachDeviceAsync(string titlebroadcast, string DeviceID, string SenderName, string msgCategory, string message, string imgPath, int MsgID,int senderID)
        {
            string  uri;
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);
            string value;
            if (message.Length > 50)
                value = message.Substring(0, 40) + "...";
            else
                value = message;


            uri = "https://fcm.googleapis.com/fcm/send";


           /* if (SenderName.Equals("THANDORA ADMIN") == true)
            {
                toGroup = "/topics/THANDORA_ADMIN";
            }
            else
            {
                toGroup = "/topics/" + GroupName;
            }
            */
            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));
                var postData = new
                {
                    //registration_ids = deviceids,
                    //to=devicegroup,
                    //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                    //to = toGroup,
                    to = DeviceID,
                    data = new
                    {
                        title = titlebroadcast + "/" + msgCategory,
                        message = value,
                        SenderName = SenderName,
                        imageUrl = imgPath,
                        messageId = MsgID,
                        SenderID = senderID,
                        notificationType = "1"  //(1->PostMessage, 2-- > EnquiryReply and so on)

                    },
                    priority = "high"

                    /*notification = new
                    {
                        body = value,
                        title = titlebroadcast + "/" + msgCategory + "/" + SenderName,
                        icon = "myicon"
                    }*/
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(postData);

                //request.AddParameter("application/json", "{\r\n                \"to\" : \""+DeviceID+"\"\r\n                \"data\":{\r\n                \t \"enquiryID\": "+enquiryid.ToString()+"\r\n                }\r\n                \r\n                \"notification\":\r\n                {\r\n                    \"body\": \""+enquirymessage+"\",\r\n                    \"title\":\""+FromUser +"\",\r\n                    \"icon\": \"myicon\"\r\n                }\r\n            }", ParameterType.RequestBody);

                //request.AddParameter("application/json", request.JsonSerializer.Serialize( new {to = DeviceID, data = new {MessageID = enquiryid}, notification = new
                // { body = enquirymessage, title = FromUser,icon = "myicon"  } }) , ParameterType.RequestBody);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                /*var rs = client.Execute(request);
                 sResponseFromServer = rs.Content.ToString();
                 return sResponseFromServer;*/
                //var asyncHandle = await client.ExecuteAsync(request, NotificationOnComplete);

                IRestResponse<cFCMReturn> asyncHandle = await client.ExecutePostTaskAsync<cFCMReturn>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                st.returnID = asyncHandle.Data.success; // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                st.StatusID = asyncHandle.Data.failure; // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                //return asyncHandle.Data.success.ToString();
             

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
            }

        }




        [Route("api/NewTellerNotification")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        private async Task<IHttpActionResult> NewTellerNotification(string GroupName, string TellerName, string TellerService)
        {
            string toGroup;
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            var value = "New TELLER:" + TellerService;
            WebRequest tRequest;
            String sResponseFromServer;

            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmDetails.SERVER_API_KEY));

            tRequest.Headers.Add(string.Format("Sender: id={0}", fcmDetails.PROJECT_KEY));
            tRequest.ContentType = "application/json;charset=UTF-8";


            toGroup = "/topics/" + GroupName;

            var postData = new
            {
                //registration_ids = deviceids,
                //to=devicegroup,
                //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                to = toGroup,
                data = new
                {
                    MessageID = 1
                },

                notification = new
                {
                    body = TellerName + " joined in Thandora for the services " + TellerService,
                    title = TellerName + " using THANDORA",
                    icon = "myicon"
                }
            };


            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(postData);

            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            //Console.WriteLine(data);
            // Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;


            try
            {
                Stream dataStream;
                //Stream dataStream = tRequest.GetRequestStream();

                using (dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse tResponse = tRequest.GetResponse();
                // dataStream.Write(byteArray, 0, byteArray.Length);
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

            // return sResponseFromServer;
            return Ok(sResponseFromServer);
        }

        [Route("api/NewTellerNotificationAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> NewTellerNotificationAsync(string GroupName, string TellerName, string TellerService,int senderID)
        {
            string toGroup, uri;
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            var value = "New TELLER:" + TellerService;
            uri = "https://fcm.googleapis.com/fcm/send";
            toGroup = "/topics/" + GroupName;

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));

                var postData = new
                {
                    //registration_ids = deviceids,
                    //to=devicegroup,
                    //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                    to = toGroup,
                    data = new
                    {
                        title =  " available in THANDORA",
                        message = TellerName + " joined in Thandora as " + TellerService,
                        SenderName = TellerName,
                        imageUrl = "",
                        messageId = senderID,
                        SenderID = senderID,
                        notificationType = 4  //(1->PostMessage, 2-- > Comment, 3 --> Enquiry, 4 -> New Teller and so on)

                    },
                    priority = "high",
                    /* data = new
                     {
                         MessageID = 1
                     },*/

                    /*notification = new
                    {
                        body = TellerName + " joined in Thandora as " + TellerService,
                        title = TellerName + " using THANDORA",
                        icon = "myicon"
                    }*/
                };


                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(postData);

                //request.AddParameter("application/json", "{\r\n                \"to\" : \""+DeviceID+"\"\r\n                \"data\":{\r\n                \t \"enquiryID\": "+enquiryid.ToString()+"\r\n                }\r\n                \r\n                \"notification\":\r\n                {\r\n                    \"body\": \""+enquirymessage+"\",\r\n                    \"title\":\""+FromUser +"\",\r\n                    \"icon\": \"myicon\"\r\n                }\r\n            }", ParameterType.RequestBody);

                //request.AddParameter("application/json", request.JsonSerializer.Serialize( new {to = DeviceID, data = new {MessageID = enquiryid}, notification = new
                // { body = enquirymessage, title = FromUser,icon = "myicon"  } }) , ParameterType.RequestBody);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                /*var rs = client.Execute(request);
                 sResponseFromServer = rs.Content.ToString();
                 return sResponseFromServer;*/
                //var asyncHandle = await client.ExecuteAsync(request, NotificationOnComplete);

                IRestResponse<cFCMReturn> asyncHandle = await client.ExecutePostTaskAsync<cFCMReturn>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                st.returnID = asyncHandle.Data.success;  // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                st.StatusID = asyncHandle.Data.failure;  // TOPIC DISTIBUTION WONT RETURN SUCCESS OR FAILURE
                //return asyncHandle.Data.success.ToString();
                return st;

            }
            catch (Exception ex)
            {
                st.StatusID = 1;
                st.StatusMsg = ex.Message.ToString();
                return st;

            }

        }



        [Route("api/EnquiryNotification")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        private async Task<IHttpActionResult> EnquiryNotification(string DeviceID, string FromUser, string enquirymessage, int enquiryid)
        {
            string toGroup;
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            //var value = "New TELLER:" + TellerService;
            WebRequest tRequest;
            String sResponseFromServer;

            tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

            tRequest.Method = "post";
            //tRequest.ContentType = " application/x-www-form-urlencoded;charset=UTF-8";
            tRequest.Headers.Add(string.Format("Authorization: key={0}", fcmDetails.SERVER_API_KEY));

            tRequest.Headers.Add(string.Format("Sender: id={0}", fcmDetails.PROJECT_KEY));
            tRequest.ContentType = "application/json;charset=UTF-8";


            //toGroup = "/topics/" + GroupName;

            var postData = new
            {
                //registration_ids = deviceids,
                //to=devicegroup,
                //to= "ckZ8rtZ6r1I:APA91bHvdVlS7p7-XdQMWAVI1zpG6LfU40Mrllj9u2wRtGhnQ-GUPk43BjtfsDadoJmYviCsn1mMluXaJfWrldLWLccddgAE1cUVIbVP1a7JSo3IIlraZoi4qipt8v-1owIavTniwahd",
                to = DeviceID,

                //to = toGroup,
                data = new
                {
                    MessageID = enquiryid
                },

                notification = new
                {
                    body = enquirymessage,
                    title = FromUser,
                    icon = "myicon"
                }
            };


            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(postData);

            Byte[] byteArray = Encoding.UTF8.GetBytes(json);

            //Console.WriteLine(data);
            // Byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            tRequest.ContentLength = byteArray.Length;


            try
            {
                Stream dataStream;
                //Stream dataStream = tRequest.GetRequestStream();

                using (dataStream = tRequest.GetRequestStream())
                {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }

                WebResponse tResponse = tRequest.GetResponse();
                // dataStream.Write(byteArray, 0, byteArray.Length);
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

            // return sResponseFromServer;
            return Ok(sResponseFromServer);
        }

        [Route("api/EnquiryNotificationAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> EnquiryNotificationAsync(string DeviceID, string FromUser, string Message, int id,string NotifyType,int senderID)
        {
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            string uri;
            uri = "https://fcm.googleapis.com/fcm/send";

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));
                /*var postData = new
                {
                    to = DeviceID,
                    data = new
                    {
                        MessageID = enquiryid
                    },

                    notification = new
                    {
                        body = enquirymessage,
                        title = FromUser,
                        icon = "myicon"
                    }
                };*/
                int typeid=0;
                if (NotifyType== "Comments")
                {
                    typeid = 2;
                }
                if (NotifyType == "Enquiry")
                {
                    typeid = 3;
                }

                if (NotifyType == "Feedback")
                {
                    typeid = 4;
                }

                var postData = new
                {
                    to = DeviceID,
                    data = new
                    {
                        title = NotifyType,
                        message = Message,
                        SenderName = FromUser,
                        imageUrl = "",
                        messageId = id,
                        SenderID= senderID,
                        notificationType = typeid  //(1->PostMessage, 2-- > Comment, 3 --> Enquiry, and so on)

                    },
                    priority = "high"


                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(postData);

                //request.AddParameter("application/json", "{\r\n                \"to\" : \""+DeviceID+"\"\r\n                \"data\":{\r\n                \t \"enquiryID\": "+enquiryid.ToString()+"\r\n                }\r\n                \r\n                \"notification\":\r\n                {\r\n                    \"body\": \""+enquirymessage+"\",\r\n                    \"title\":\""+FromUser +"\",\r\n                    \"icon\": \"myicon\"\r\n                }\r\n            }", ParameterType.RequestBody);

                //request.AddParameter("application/json", request.JsonSerializer.Serialize( new {to = DeviceID, data = new {MessageID = enquiryid}, notification = new
                // { body = enquirymessage, title = FromUser,icon = "myicon"  } }) , ParameterType.RequestBody);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                /*var rs = client.Execute(request);
                 sResponseFromServer = rs.Content.ToString();
                 return sResponseFromServer;*/
                //var asyncHandle = await client.ExecuteAsync(request, NotificationOnComplete);

                IRestResponse<cFCMReturn> asyncHandle = await client.ExecutePostTaskAsync<cFCMReturn>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                st.returnID = asyncHandle.Data.success;
                st.StatusID = asyncHandle.Data.failure;
                //return asyncHandle.Data.success.ToString();
                return st;

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
                return st;

            }

        }


        [Route("api/SubscriberNotificationAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> SubscriberNotificationAsync(string DeviceID, string subscriber, string msg)
        {
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            string uri;
            uri = "https://fcm.googleapis.com/fcm/send";

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("content-type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));
                var postData = new
                {
                    to = DeviceID,
                    data = new
                    {
                        MessageID = 1
                    },
                    priority = "high",
                    notification = new
                    {
                        body = subscriber + msg,
                        title = "Subscription update",
                        icon = "myicon"
                    }
                };

                var serializer = new JavaScriptSerializer();
                var json = serializer.Serialize(postData);

                //request.AddParameter("application/json", "{\r\n                \"to\" : \""+DeviceID+"\"\r\n                \"data\":{\r\n                \t \"enquiryID\": "+enquiryid.ToString()+"\r\n                }\r\n                \r\n                \"notification\":\r\n                {\r\n                    \"body\": \""+enquirymessage+"\",\r\n                    \"title\":\""+FromUser +"\",\r\n                    \"icon\": \"myicon\"\r\n                }\r\n            }", ParameterType.RequestBody);

                //request.AddParameter("application/json", request.JsonSerializer.Serialize( new {to = DeviceID, data = new {MessageID = enquiryid}, notification = new
                // { body = enquirymessage, title = FromUser,icon = "myicon"  } }) , ParameterType.RequestBody);

                request.AddParameter("application/json", json, ParameterType.RequestBody);
                /*var rs = client.Execute(request);
                 sResponseFromServer = rs.Content.ToString();
                 return sResponseFromServer;*/
                //var asyncHandle = await client.ExecuteAsync(request, NotificationOnComplete);

                IRestResponse<cFCMReturn> asyncHandle = await client.ExecutePostTaskAsync<cFCMReturn>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                st.returnID = asyncHandle.Data.success;
                st.StatusID = asyncHandle.Data.failure;
                //return asyncHandle.Data.success.ToString();
                return st;

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
                return st;

            }

        }


        [Route("api/PostMessageNotification")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostMessageNotification(cmsgPostReturn msgPostReturn)
        {
            cStatus st = new cStatus();
            MessageController mc = new MessageController();
            string retvalue;
            if (msgPostReturn.broadCast.Equals("Y"))
            {
                ////SEND NOTIFICATION FOR BROADCAST MESSAGECATEGORY 

                if (msgPostReturn.msgDistributedTo.Equals("NATIONAL"))
                {
                    st = await PostMsgImageNotificationAsync("INDIA", "INDIA", msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent,msgPostReturn.msgImagePath, msgPostReturn.retValue,msgPostReturn.senderID);

                }

                if (msgPostReturn.msgDistributedTo.Equals("STATE"))
                {

                    try
                    {
                        List<string> topics;
                        topics = mc.getTopicsforpostcode(msgPostReturn.POSTCODE, "STATE");
                        foreach (string topic in topics)
                        {
                            st = await PostMsgImageNotificationAsync(topic, topic, msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent, msgPostReturn.msgImagePath, msgPostReturn.retValue, msgPostReturn.senderID);
                        }
                    }
                    catch (Exception ex)
                    {
                        retvalue = ex.Message.ToString();
                    }
                }

                if (msgPostReturn.msgDistributedTo.Equals("DISTRICT"))
                {
                    try
                    {
                        List<string> topics;
                        //postcode = "110006";
                        topics = mc.getTopicsforpostcode(msgPostReturn.POSTCODE, "DISTRICT");
                        foreach (string topic in topics)
                        {
                            st = await PostMsgImageNotificationAsync(topic, topic, msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent, msgPostReturn.msgImagePath, msgPostReturn.retValue, msgPostReturn.senderID);
                        }
                    }
                    catch (Exception ex)
                    {
                        retvalue = ex.Message.ToString();
                    }
                }

                if (msgPostReturn.msgDistributedTo.Equals("POSTCODE"))
                {
                    st = await PostMsgImageNotificationAsync("LOCAL ", msgPostReturn.POSTCODE, msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent, msgPostReturn.msgImagePath, msgPostReturn.retValue, msgPostReturn.senderID);
                }

                if (msgPostReturn.msgDistributedTo.Equals("SUBSCRIBER"))
                {
                    st = await PostMsgImageNotificationAsync("SUBSCRIBER", msgPostReturn.senderID.ToString(), msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent, msgPostReturn.msgImagePath, msgPostReturn.retValue, msgPostReturn.senderID);
                }
            }
            else
            {
                st = await PostMsgImageNotificationAsync("SUBSCRIBER", msgPostReturn.senderID.ToString(), msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent, msgPostReturn.msgImagePath, msgPostReturn.retValue, msgPostReturn.senderID);
            }

            return st;
        }

        [Route("api/fcmsubcriptioncheckAsync")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> GetfcmsubcriptioncheckAsync(string DeviceID)
        {
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            string uri;
            //uri = "https://iid.googleapis.com/iid/info/f0l3VKFi_bs:APA91bEvLlgSHIwB3zJv3YEL_jkjxXRVzWY85jNe4_6tSC-IE7oh-FqJBajA9nuDNI7pZCZ1eqa2aDjCCpK9bS7jW0ONezffGvRMs7q4lbyVGEYkKLv4tH9wE26b80aSLM5sdowG349G?details=true";
            uri = "https://iid.googleapis.com/iid/info/" + DeviceID + "?details=true";

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);

                request.AddHeader("sender", string.Format("id={0}", fcmDetails.PROJECT_KEY));
                request.AddHeader("Content-Type", "application/json");
                request.AddHeader("authorization", string.Format("key={0}", fcmDetails.SERVER_API_KEY));


                IRestResponse<cFCMSubscribed> asyncHandle = await client.ExecuteGetTaskAsync<cFCMSubscribed>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
                if (st.DesctoDev.Contains("THANDORA_ADMIN") && st.DesctoDev.Contains("INDIA"))
                {
                    st.StatusMsg = "THANDORA Installed successfully";
                    st.StatusID = 0;
                }
                else
                {
                    st.StatusMsg = "THANDORA Installation failed. Please install it again. ";
                    st.StatusID = 1;
                }
                return st;

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
                return st;

            }

        }


        [Route("api/CalculateDistanceAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cPostCodeDistance))]
        public async Task<cPostCodeDistance> GetCalculatedistanceAsync(string origin, string destination)
        {
            //string origin, destination;
            //origin = "600113";
            //destination = "605008";
            cPostCodeDistance distance = new cPostCodeDistance();
           
            string uri;
            uri="http://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + ",india&destinations=" + destination + ",india&mode=driving&language=en-EN&sensor=false";
            try
            {
                var client = new RestClient(uri);
                var request = new RestRequest(Method.GET);
                IRestResponse<cPostCodeDistance> asyncHandle = await client.ExecutePostTaskAsync<cPostCodeDistance>(request);
                distance = (cPostCodeDistance) asyncHandle.Data;
                return distance;

            }
            catch (Exception ex)
            {
                distance.status = "failed";
                return distance;

            }

        }


    }
}
