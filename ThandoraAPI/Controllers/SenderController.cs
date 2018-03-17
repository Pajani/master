using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class SenderController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/Sender
      /*  public IQueryable<ctblSender> GetRs_tblSender()
        {
            return db.Rs_tblSender;
        }

        // GET: api/Sender/5
        [ResponseType(typeof(ctblSender))]
        public IHttpActionResult GetctblSender(int id)
        {
            ctblSender ctblSender = db.Rs_tblSender.Find(id);
            if (ctblSender == null)
            {
                return NotFound();
            }

            return Ok(ctblSender);
        }*/


        //GET: api/GetReceiverOf(SenderID)
        [Route("api/Sender")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cPartialSender>))]
        public async Task <List<cPartialSender>> GetSenderOf(int ReceiverID, string RecUserType)
        {
            List<cPartialSender> SendersList = new List<cPartialSender>();
           


            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_getSendersOf", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@ReceiverUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    con.Open();
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        cPartialSender c = new cPartialSender();
                        c.SenderID = (int)reader["SenderID"];
                        c.SenderName= reader["SenderName"].ToString().Trim();
                        c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                     
                        c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                        c.ContactHide = (Int16) reader["ContactHide"];
                        c.cServiceType = reader["cServiceType"].ToString().Trim();
                        c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();
                        c.ReviewReceived = (int)reader["ReviewReceived"];
                        c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                        c.msgLikeReceived = (int)reader["msgLikeReceived"];
                        c.msgTotpublished = (int)reader["msgTotpublished"];
                        c.msgReadBy = (int)reader["msgReadBy"];
                        c.logopath = reader["logopath"].ToString().Trim();

                        //c.logopath= reader["logopath"].ToString().Trim();

                        SendersList.Add(c);

                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            //return Ok(retvalue);*/


            return SendersList;
        }


        [Route("api/TellerUpdate")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> TellerUpdate([FromBody] ctblSender ctblSender)
        {
            cStatus status = new cStatus();
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_update_sender", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    paramSenderID.Value = ctblSender.SenderID;
                    cmd.Parameters.Add(paramSenderID);

                    SqlParameter paramSendName = new SqlParameter();
                    paramSendName.ParameterName = "@SenderName";
                    paramSendName.Value = ctblSender.SenderName;
                    cmd.Parameters.Add(paramSendName);

              
                    SqlParameter paramSenderContactNo_1 = new SqlParameter();
                    paramSenderContactNo_1.ParameterName = "@SenderContactNo_1";
                    paramSenderContactNo_1.Value = ctblSender.SenderContactNo_1;
                    cmd.Parameters.Add(paramSenderContactNo_1);

                
                    SqlParameter paramContactHide = new SqlParameter();
                    paramContactHide.ParameterName = "@ContactHide";
                    paramContactHide.Value = ctblSender.ContactHide;
                    cmd.Parameters.Add(paramContactHide);

                    SqlParameter paramcServiceType = new SqlParameter();
                    paramcServiceType.ParameterName = "@cServiceType";
                    paramcServiceType.Value = ctblSender.cServiceType;
                    cmd.Parameters.Add(paramcServiceType);

                    SqlParameter paramServiceDesc = new SqlParameter();
                    paramServiceDesc.ParameterName = "@ServiceDesc";
                    paramServiceDesc.Value = ctblSender.ServiceDesc;
                    cmd.Parameters.Add(paramServiceDesc);
                  
                    

                    SqlParameter paramAddress = new SqlParameter();
                    paramAddress.ParameterName = "@Address";
                    paramAddress.Value = ctblSender.Address;
                    cmd.Parameters.Add(paramAddress);

                    cmd.Parameters.Add("@LastUpdatedID", SqlDbType.Int);
                    cmd.Parameters["@LastUpdatedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    status.returnID = (int) cmd.Parameters["@LastUpdatedID"].Value;
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "You are not active user";
                    }
                    if (status.returnID  == -3)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Bad word in Service description";
                    }
                    if (status.returnID == -4)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Bad word in  Name ";
                    }
                    if (status.returnID == -5)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Name Not Allowed ";
                    }

                    if (status.returnID == -6)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Name Not Available ";
                    }

                    if (status.returnID >1)
                    {
                        status.StatusID = 0;
                        status.StatusMsg = "Profile updated successfully ";
                    }


                }
            }
            catch (Exception ex)
            {
                status.StatusMsg = ex.Message.ToString();
            }
            return status;

        }

        // POST: api/Sender
        [Route("api/Sender")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<IHttpActionResult> PostctblSender([FromBody]ctblSender ctblSender)
        {
            cStatus status = new cStatus();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_new_sender", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramSendName = new SqlParameter();
                    paramSendName.ParameterName = "@SenderName";
                    paramSendName.Value = ctblSender.SenderName;
                    cmd.Parameters.Add(paramSendName);

                    SqlParameter paramSenderPhone = new SqlParameter();
                    paramSenderPhone.ParameterName = "@SenderPhone";
                    paramSenderPhone.Value = ctblSender.SenderPhone;
                    cmd.Parameters.Add(paramSenderPhone);

                    if (ctblSender.deviceTokenID == null )
                    {
                        ctblSender.deviceTokenID = "Assigned by API";
                       /* status.StatusID = 1;
                        status.DesctoDev = "FCM Device id null ";
                        status.StatusMsg = " FCM Device id null ";
                        return Ok(status);*/
                    }
                   
                        SqlParameter paramdeviceTokenID = new SqlParameter();
                        paramdeviceTokenID.ParameterName = "@IME";
                        paramdeviceTokenID.Value = ctblSender.deviceTokenID;
                        cmd.Parameters.Add(paramdeviceTokenID);
                 

                    SqlParameter paramRecSIMNO = new SqlParameter();
                    paramRecSIMNO.ParameterName = "@SIMNO";
                    paramRecSIMNO.Value = ctblSender.SIMNO;
                    cmd.Parameters.Add(paramRecSIMNO);

                    SqlParameter paramSenderContactNo_1 = new SqlParameter();
                    paramSenderContactNo_1.ParameterName = "@SenderContactNo_1";
                    paramSenderContactNo_1.Value = ctblSender.SenderContactNo_1;
                    cmd.Parameters.Add(paramSenderContactNo_1);

                   /* SqlParameter paramSenderContactNo_2 = new SqlParameter();
                    paramSenderContactNo_2.ParameterName = "@SenderContactNo_2";
                    paramSenderContactNo_2.Value = ctblSender.SenderContactNo_2;
                    cmd.Parameters.Add(paramSenderContactNo_2);
                    */

                    SqlParameter paramContactHide = new SqlParameter();
                    paramContactHide.ParameterName = "@ContactHide";
                    paramContactHide.Value = ctblSender.ContactHide;
                    cmd.Parameters.Add(paramContactHide);

                    SqlParameter paramcServiceType = new SqlParameter();
                    paramcServiceType.ParameterName = "@cServiceType";
                    paramcServiceType.Value = ctblSender.cServiceType;
                    cmd.Parameters.Add(paramcServiceType);

                    SqlParameter paramServiceDesc = new SqlParameter();
                    paramServiceDesc.ParameterName = "@ServiceDesc";
                    if (ctblSender.ServiceDesc == null)
                    { ctblSender.ServiceDesc = " "; }
                    paramServiceDesc.Value = ctblSender.ServiceDesc;
                    cmd.Parameters.Add(paramServiceDesc);

                    SqlParameter paramPOSTCODE = new SqlParameter();
                    paramPOSTCODE.ParameterName = "@POSTCODE";
                    paramPOSTCODE.Value = ctblSender.POSTCODE;
                    cmd.Parameters.Add(paramPOSTCODE);

                    SqlParameter paramAddress = new SqlParameter();
                    paramAddress.ParameterName = "@Address";
                    if (ctblSender.Address==null)
                    { ctblSender.Address = " "; }
                    paramAddress.Value = ctblSender.Address;
                    cmd.Parameters.Add(paramAddress);

                    SqlParameter paramlogopath = new SqlParameter();
                    paramlogopath.ParameterName = "@logopath";
                    paramlogopath.Value = ctblSender.logopath;
                    cmd.Parameters.Add(paramlogopath);

                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID == -5)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = " Name Restricted ";
                        status.StatusMsg = " Name Restricted ";
                    }

                    if (status.returnID == -4)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Restricted word in Name ";
                        status.StatusMsg = " Restricted word in Name ";
                    }
                    if (status.returnID == -3)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Restricted word in Description ";
                        status.StatusMsg = " Restricted word in Description ";
                    }
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "-2 is the error code for invalid POSTAL code. ";
                        status.StatusMsg = " Invalid POSTAL Code. ";
                    }
                    if (status.returnID >= 100000)
                    {
                        // Whenever a new TELLER is added, a new TOPIC will be created in FCM on the TELLER ID. So that the notification
                        //will be published on the TELLER topic name, it will reach automatically all the subcribers of the TELLER. 
                        //Every LISTENER & TELLER should be subscribed to the below TOPIC mandatory according to their POSTCODE
                        //FirebaseMessaging.getInstance().subscribeToTopic("POSTCODE);
                        //FirebaseMessaging.getInstance().subscribeToTopic("POSTCODE);


                        //Whenever a receiver subcribing a TELLER from Services menu that time the below line should be exeucted from the client. 
                        //FirebaseMessaging.getInstance().subscribeToTopic("SenderID");

                        //DeviceGroupController dgc = new DeviceGroupController();
                        //Task<string> s=  dgc.CreateTopicGroup(ctblSender.deviceTokenID, status.returnID.ToString());

                        status.StatusID = 0;
                        //status.DesctoDev = s.ToString();
                     

                        status.StatusMsg = " User Successfully Created/Updated ";
                        status.userType = "S";
                        SendNotificationController s = new SendNotificationController();
                       
                        string msg;
                        FCMController fcm = new FCMController();
                        cStatus st = new cStatus();
                        if (ctblSender.cServiceType != "Personal Use" && ctblSender.cServiceType != "Home Maker")
                        {
                            st = await s.NewTellerNotificationAsync(ctblSender.POSTCODE, ctblSender.SenderName, ctblSender.cServiceType, status.returnID);
                            msg = "@POSTCODE:" + ctblSender.POSTCODE + "@SenderName:" + ctblSender.SenderName + "New user joined @ServiceType:" + ctblSender.cServiceType;

                            if (st.DesctoDev.Contains("message_id"))
                            {
                                status.StatusMsg = "Broadcast success";
                                await fcm.PostFCMResponse(st.DesctoDev, msg, "Y");
                            }
                            else
                            {
                                status.StatusMsg = "Broadcast failed";
                                await fcm.PostFCMResponse(st.DesctoDev, msg, "N");
                            }
                        }
                        ctblEnquiry enq = new ctblEnquiry();
                        string str;
                        enq.EnquiryID = 0;
                        str = @"Welcome to Thandora a Business/Social media app to promote your business and services.

All business users requested to update your Address, Profile picture, business descriptions clearly with commonly searched key words specific to your business.

Any search/enquiry on Thandora related to your business area or business name, would be notified to you through SMS and Thandora notification.

If you are NOT posting for 30 calendar days, your name and services will be made dormant and made unavailable to public. 

You can invite your friends/neighbors/business partners/workers/staffs/contractors/owners to promote their business and services.

WEB: http://www.thandora.co

FB: https://www.facebook.com/search/top/?q=thandora%20-%20a%20business%20app

Regards
Thandora Admin";
                        enq.EnquiryMessage = str;
                        enq.FromUserID = 100089;
                        enq.FromUserType = "S";
                        enq.ToUserID = status.returnID;
                        enq.ToUserType = "S";
                        enq.AnswerID = 0;
                        MessageController mc = new MessageController();
                        mc.PostEnquiry(enq);
                        
                        AuthorizationController ac = new AuthorizationController();
                        ac.RefreshUsers();
                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }

            return Ok(status);

        }

        // POST: api/Sender
        [Route("api/GuestMode")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cguestModeStatus))]
        public async Task<cguestModeStatus> PostctblSender([FromBody]cGuestMode cgmode)
        {
            cStatus status = new cStatus();
            cguestModeStatus cguesttopics = new cguestModeStatus();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_guest_OnOff", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                  
                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userID";
                    paramuserID.Value = cgmode.userID;
                    cmd.Parameters.Add(paramuserID);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = cgmode.userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlParameter paramgPOSTCODE = new SqlParameter();
                    paramgPOSTCODE.ParameterName = "@guest_postcode";
                    paramgPOSTCODE.Value = cgmode.gPOSTCODE;
                    cmd.Parameters.Add(paramgPOSTCODE);


                    SqlParameter paramis_guest_mode = new SqlParameter();
                    paramis_guest_mode.ParameterName = "@is_guest_mode";
                    paramis_guest_mode.Value = cgmode.is_guest_mode;
                    cmd.Parameters.Add(paramis_guest_mode);


                    cmd.Parameters.Add("@result", SqlDbType.Int);
                    cmd.Parameters["@result"].Direction = ParameterDirection.Output;

                                    
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();


                    retvalue = cmd.Parameters["@result"].Value.ToString();
                    status.DesctoDev = cmd.Parameters["@guest_postcode"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID == 0)
                    {
                        status.StatusID = 0;
                        //status.DesctoDev = "Execute FCM subscribe method for the new POSTCODE ";
                        status.StatusMsg = " GUEST MODE ON ";
                    }
                    if (status.returnID == 1)
                    {
                        status.StatusID = 0;
                        //status.DesctoDev = "Execute FCM UNsubscribe method for the guest POSTCODE ";
                        status.StatusMsg = " GUEST MODE OFF ";
                    }

                    if (status.returnID == -1)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "NOT ACTIVE USER ";
                        status.StatusMsg = " NOT ACTIVE USER";
                    }
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = " POST CODE NOT AVAILABLE ";
                        status.StatusMsg = " GUEST POSTCODE NOT AVAILABLE";
                    }
                    if (status.returnID == -3)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "SAME POST CODE ";
                        status.StatusMsg = "GUEST POSTCODE SHOULD BE DIFFERENT FROM YOUR ORIGINAL POSTCODE ";
                    }
                   
                    if (status.StatusID==0)
                    {
                       
                        AuthorizationController ca = new AuthorizationController();
                        cguesttopics.AreaNames= await ca.GetGuestModeTopics(cgmode.userID, cgmode.userType,status.DesctoDev);
                        cguesttopics.status = status;
                    }
                    else
                    {
                        cguesttopics.AreaNames = new List<string>();
                        cguesttopics.status = status;
                       

                    }
                  
                }

            }
            catch (Exception ex)
            {
                status.StatusMsg = ex.Message.ToString();
                status.StatusID = 1;
                cguesttopics.status = status;
                cguesttopics.AreaNames = new List<string>();
            }

            return cguesttopics;

        }



        // DELETE: api/Sender/5
        /* [ResponseType(typeof(ctblSender))]
         public IHttpActionResult DeletectblSender(int id)
         {
             ctblSender ctblSender = db.Rs_tblSender.Find(id);
             if (ctblSender == null)
             {
                 return NotFound();
             }

             db.Rs_tblSender.Remove(ctblSender);
             db.SaveChanges();

             return Ok(ctblSender);
         }

         protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 db.Dispose();
             }
             base.Dispose(disposing);
         }*/

        private bool ctblSenderExists(int id)
        {
            return db.Rs_tblSender.Count(e => e.SenderID == id) > 0;
        }
    }
}