using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class AuthorizationController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

      

        // POST: api/Authorization
        [ResponseType(typeof(cAuthorization))]
        public cAuthorization PostcAuthorization([FromBody]cAuthorization cAuthorization)
        {

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {

                    if (cAuthorization.purpose.Equals("CREATE"))
                    {

                        SqlCommand cmd = new SqlCommand("sp_create_Key", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter paramphoneNo = new SqlParameter();
                        paramphoneNo.ParameterName = "@PhoneNo";
                        paramphoneNo.Value = cAuthorization.phoneNo;
                        cmd.Parameters.Add(paramphoneNo);

                        cmd.Parameters.Add("@key", SqlDbType.Int);
                        cmd.Parameters["@key"].Direction = ParameterDirection.Output;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        retvalue = cmd.Parameters["@key"].Value.ToString();
                        cAuthorization.AuthorizeKey = int.Parse(retvalue);

                    }
                    else
                    {

                        SqlCommand cmd = new SqlCommand("sp_validate_Key", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter paramdeviceID = new SqlParameter();
                        paramdeviceID.ParameterName = "@deviceID";
                        paramdeviceID.Value = cAuthorization.deviceID;
                        cmd.Parameters.Add(paramdeviceID);

                        SqlParameter paramkey = new SqlParameter();
                        paramkey.ParameterName = "@key";
                        paramkey.Value = cAuthorization.AuthorizeKey;
                        cmd.Parameters.Add(paramkey);

                        cmd.Parameters.Add("@Result", SqlDbType.Int);
                        cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                        con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        retvalue = cmd.Parameters["@Result"].Value.ToString();

                        if (int.Parse(retvalue)> 1) { cAuthorization.verified = "S"; }
                        if (int.Parse(retvalue) < 1) { cAuthorization.verified = "F"; }
                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return cAuthorization;
          
        }

        [Route("api/GetAreaName")]
        [AllowAnonymous]
        [ResponseType(typeof(cAreaName))]
        public cAreaName GetAreaName(string POSTCODE)
        {
         
            cAreaName listAreaName = new cAreaName();
            listAreaName.AreaNames = new List<string>();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_getAreaName", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramPOSTCODE = new SqlParameter();
                    paramPOSTCODE.ParameterName = "@postcode";
                    paramPOSTCODE.Value = POSTCODE;
                    cmd.Parameters.Add(paramPOSTCODE);


                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        listAreaName.AreaNames.Add(reader["AreaName"].ToString().Trim());
                           
                       
                    }
                    
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listAreaName;

        }

        [Route("api/CheckAreaandName")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> CheckAreaandName(string POSTCODE,string sendername)
        {

             cStatus status = new cStatus();
        
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_getAreaandName", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramPOSTCODE = new SqlParameter();
                    paramPOSTCODE.ParameterName = "@postcode";
                    paramPOSTCODE.Value = POSTCODE;
                    cmd.Parameters.Add(paramPOSTCODE);

                    SqlParameter paramsenderName = new SqlParameter();
                    paramsenderName.ParameterName = "@senderName";
                    paramsenderName.Value = sendername;
                    cmd.Parameters.Add(paramsenderName);


                    cmd.Parameters.Add("@areaName", SqlDbType.NVarChar, 400);
                    cmd.Parameters["@areaName"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@isSenderexist", SqlDbType.NVarChar, 1);
                    cmd.Parameters["@isSenderexist"].Direction = ParameterDirection.Output;



                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    status.StatusMsg = cmd.Parameters["@areaName"].Value.ToString();
                    status.DesctoDev= cmd.Parameters["@isSenderexist"].Value.ToString();
                    status.StatusID = 0;
                    if (status.DesctoDev.Contains("Y"))
                    { status.StatusID = 1;
                        status.StatusMsg = "Name already exists in this POSTCODE";

                    }
                    if (status.StatusMsg.Contains("Check your POSTCODE"))
                    { status.StatusID = 2;
                        
                    }


                }
            }
            catch (Exception ex)
            {
                status.StatusMsg = ex.Message.ToString();
                status.StatusID = 1;
            }

            return status;

        }


        [Route("api/PreRequestToPost")]
        [AllowAnonymous]
        [ResponseType(typeof(List<cDetailedArea>))]
        public async Task<List<cDetailedArea>> GetPreRequestToPost(string userID, string userType)
        {

            List<cDetailedArea> listAreaName = new List<cDetailedArea>();
            cDetailedArea da;

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_getDetailedArea", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);


                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        da = new cDetailedArea();
                        da.value = reader["AreaName"].ToString().Trim();
                        da.key = reader["AreaType"].ToString().Trim();
                        da.GROUPBY = reader["groupby"].ToString().Trim();

                        listAreaName.Add(da);
                     }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listAreaName;

        }

        [Route("api/PreRequestToPostv1_3")]
        [AllowAnonymous]
        [ResponseType(typeof(List<cDetailedArea>))]
        public async Task<List<cDetailedArea>> PreRequestToPostv1_3(string userID, string userType)
        {

            List<cDetailedArea> listAreaName = new List<cDetailedArea>();
            cDetailedArea da;

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_Post_message_prerequisite", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);


                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        da = new cDetailedArea();
                        da.value = reader["AreaName"].ToString().Trim();
                        da.key = reader["AreaType"].ToString().Trim();
                        da.GROUPBY = reader["groupBy"].ToString().Trim();

                        listAreaName.Add(da);
                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listAreaName;

        }


        [Route("api/GetTopics")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(cAreaName))]
        public async Task<cAreaName> GetTopics(string userID, string userType)
        {
            cAreaName listTopicName = new cAreaName();
            listTopicName.AreaNames = new List<string>();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_userTopic", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        listTopicName.AreaNames.Add(reader["TopicName"].ToString().Trim());

                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listTopicName;


        }


        [Route("api/GetGuestModeTopics")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<string>))]
        public async Task<List<string>> GetGuestModeTopics(int userID, string userType,string guest_postcode)
        {
            //cAreaName listTopicName = new cAreaName();
            List<string> AreaNames = new List<string>();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_guestuserTopic", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlParameter paramgpostcode = new SqlParameter();
                    paramgpostcode.ParameterName = "@guest_postcode";
                    paramgpostcode.Value = guest_postcode;
                    cmd.Parameters.Add(paramgpostcode);

                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        AreaNames.Add(reader["TopicName"].ToString().Trim());

                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                AreaNames.Add(retvalue);
            }

            return AreaNames;

        }



        [Route("api/GetPostCodePermanentUpdate")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cPermanentPostcode>))]
        public async Task<List<cPermanentPostcode>> GetPostCodePermanentUpdate(int userID, string userType, string curr_postcode,string new_postcode)
        {
            //cAreaName listTopicName = new cAreaName();
            List<cPermanentPostcode> AreaNames = new List<cPermanentPostcode>();
            cPermanentPostcode cp;
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_userPermanentUpdate", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlParameter paramcurpostcode = new SqlParameter();
                    paramcurpostcode.ParameterName = "@curr_postcode";
                    paramcurpostcode.Value = curr_postcode;
                    cmd.Parameters.Add(paramcurpostcode);

                    SqlParameter paramNewpostcode = new SqlParameter();
                    paramNewpostcode.ParameterName = "@new_postcode";
                    paramNewpostcode.Value = new_postcode;
                    cmd.Parameters.Add(paramNewpostcode);


                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        cp = new cPermanentPostcode();
                        cp.AreaName = reader["TopicName"].ToString().Trim();
                        cp.csubscribe = reader["subscribe"].ToString().Trim();

                        AreaNames.Add(cp);

                    }
                }
            }
            catch (Exception ex)
            {
                cp = new cPermanentPostcode();
                cp.AreaName=ex.Message.ToString();
                AreaNames.Add(cp);
            }

            return AreaNames;

        }




        [Route("api/GetUserProfile")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(cUserProfile))]
        public async Task<cUserProfile> GetUserProfile(string userID, string userType)
        {
            cUserProfile userProfile = new cUserProfile();
           
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_getUserProfile", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        userProfile.userID= (int)reader["userid"];
                        userProfile.userType = reader["usertype"].ToString().Trim();
                        userProfile.userName = reader["username"].ToString().Trim();
                        userProfile.postcode = reader["postcode"].ToString().Trim();
                        userProfile.address = reader["Address"].ToString().Trim();
                        userProfile.doj = reader["doj"].ToString().Trim();
                        userProfile.cServiceType = reader["cserviceType"].ToString().Trim();
                        userProfile.ServiceDesc = reader["serviceDesc"].ToString().Trim();
                        userProfile.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                        userProfile.ContactHide = reader["ContactHide"].ToString().Trim(); 
                        //userProfile.user_level = reader["user_level"].ToString().Trim();
                        userProfile.logopath = reader["logopath"].ToString().Trim();
                        //userProfile.isActive = reader["ActiveUser"].ToString().Trim();
                        //userProfile.ReviewReceived= (int) reader["ReviewReceived"];
                        //userProfile.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                        userProfile.msgLikeReceived = (int)reader["msgLikeReceived"];
                        //userProfile.msgReadBy = (int)reader["msgReadBy"];
                        userProfile.msgTotpublished = (int)reader["msgTotpublished"];
                        userProfile.subscribersCnt = (int)reader["Subscribercnt"];


                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return userProfile;


        }


        [Route("api/GetOtherUserProfile")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(cUserProfile))]
        public async Task<cUserProfile> GetOtherUserProfile(string Loggeduserid, string LoggeduserType, string wanteduserID, string wanteduserType)
        {
            cUserProfile userProfile = new cUserProfile();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_getOtherUserProfile", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramLoggeduserID = new SqlParameter();
                    paramLoggeduserID.ParameterName = "@LoginUserid";
                    paramLoggeduserID.Value = Loggeduserid;
                    cmd.Parameters.Add(paramLoggeduserID);


                    SqlParameter paramLoggeduserType = new SqlParameter();
                    paramLoggeduserType.ParameterName = "@LoginUsertype";
                    paramLoggeduserType.Value = LoggeduserType;
                    cmd.Parameters.Add(paramLoggeduserType);

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@Requesteduserid";
                    paramuserID.Value = wanteduserID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@Requestedusertype";
                    paramuserType.Value = wanteduserType;
                    cmd.Parameters.Add(paramuserType);

                    SqlDataReader reader;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        userProfile.userID = (int)reader["userid"];
                        userProfile.userType = reader["usertype"].ToString().Trim();
                        userProfile.userName = reader["username"].ToString().Trim();
                        userProfile.postcode = reader["postcode"].ToString().Trim();
                        userProfile.address = reader["Address"].ToString().Trim();
                        userProfile.doj = reader["doj"].ToString().Trim();
                        userProfile.cServiceType = reader["cserviceType"].ToString().Trim();
                        userProfile.ServiceDesc = reader["serviceDesc"].ToString().Trim();
                        userProfile.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                        userProfile.ContactHide = reader["ContactHide"].ToString().Trim();
                        //userProfile.user_level = reader["user_level"].ToString().Trim();
                        userProfile.logopath = reader["logopath"].ToString().Trim();
                        //userProfile.isActive = reader["ActiveUser"].ToString().Trim();
                        //userProfile.ReviewReceived= (int) reader["ReviewReceived"];
                        //userProfile.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                        userProfile.msgLikeReceived = (int)reader["msgLikeReceived"];
                        //userProfile.msgReadBy = (int)reader["msgReadBy"];
                        userProfile.msgTotpublished = (int)reader["msgTotpublished"];
                        userProfile.subscribersCnt = (int)reader["Subscribercnt"];


                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return userProfile;


        }



        [Route("api/GetUserProfile_V1_3")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(cUserProfile))]
        public async Task<cUserProfile> GetUserProfile_V1_3(string userID, string userType)
        {
            cUserProfile userProfile = new cUserProfile();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    SqlCommand cmd = new SqlCommand("sp_getUserProfile", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdin = new SqlCommand("sp_Inbox_Msg_count", con);
                    cmdin.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdall = new SqlCommand("sp_Others_All_Msg_count", con);
                    cmdall.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdusercount = new SqlCommand("sp_user_count", con);
                    cmdusercount.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdSercount = new SqlCommand("sp_Service_user_count", con);
                    cmdSercount.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdCmntcount = new SqlCommand("sp_new_comments_count", con);
                    cmdCmntcount.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdUserAlert = new SqlCommand("sp_user_alert", con);
                    cmdUserAlert.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdenqcount = new SqlCommand("sp_enquiry_New_count", con);
                    cmdenqcount.CommandType = CommandType.StoredProcedure;

                    SqlCommand cmdSuggUsers = new SqlCommand("sp_get_suggested_service_providers", con);
                    cmdSuggUsers.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdAdvert = new SqlCommand("sp_get_firstPage_Advertisement", con);
                    cmdAdvert.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdVersion = new SqlCommand("sp_get_version", con);
                    cmdVersion.CommandType = CommandType.StoredProcedure;

                    SqlCommand cmdAdcount = new SqlCommand("sp_Advertisement_New_count", con);
                    cmdAdcount.CommandType = CommandType.StoredProcedure;


                    cmdVersion.Parameters.Add("@version", SqlDbType.NVarChar, 5);
                    cmdVersion.Parameters["@version"].Direction = ParameterDirection.Output;


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = userID;
                    cmdin.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverID1 = new SqlParameter();
                    paramReceiverID1.ParameterName = "@ReceiverID";
                    paramReceiverID1.Value = userID;
                    cmdall.Parameters.Add(paramReceiverID1);

                    SqlParameter paramRecUserType = new SqlParameter();
                    paramRecUserType.ParameterName = "@RecUserType";
                    paramRecUserType.Value = userType;
                    cmdin.Parameters.Add(paramRecUserType);

                    SqlParameter paramRecUserType1 = new SqlParameter();
                    paramRecUserType1.ParameterName = "@RecUserType";
                    paramRecUserType1.Value = userType;
                    cmdall.Parameters.Add(paramRecUserType1);



                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlParameter paramReceiverID3 = new SqlParameter();
                    paramReceiverID3.ParameterName = "@ReceiverID";
                    paramReceiverID3.Value = userID;
                    cmdSercount.Parameters.Add(paramReceiverID3);

                    SqlParameter paramRecUserType3 = new SqlParameter();
                    paramRecUserType3.ParameterName = "@RecUserType";
                    paramRecUserType3.Value = userType;
                    cmdSercount.Parameters.Add(paramRecUserType3);

                    SqlParameter paramReceiverID4 = new SqlParameter();
                    paramReceiverID4.ParameterName = "@ReceiverID";
                    paramReceiverID4.Value = userID;
                    cmdenqcount.Parameters.Add(paramReceiverID4);

                    SqlParameter paramRecUserType4 = new SqlParameter();
                    paramRecUserType4.ParameterName = "@RecUserType";
                    paramRecUserType4.Value = userType;
                    cmdenqcount.Parameters.Add(paramRecUserType4);

                    SqlParameter paramReceiverID5 = new SqlParameter();
                    paramReceiverID5.ParameterName = "@userid";
                    paramReceiverID5.Value = userID;
                    cmdCmntcount.Parameters.Add(paramReceiverID5);


                    SqlParameter paramRecUserType5 = new SqlParameter();
                    paramRecUserType5.ParameterName = "@UserType";
                    paramRecUserType5.Value = userType;
                    cmdCmntcount.Parameters.Add(paramRecUserType5);


                    SqlParameter paramReceiverID6 = new SqlParameter();
                    paramReceiverID6.ParameterName = "@userid";
                    paramReceiverID6.Value = userID;
                    cmdUserAlert.Parameters.Add(paramReceiverID6);


                    SqlParameter paramRecUserType6 = new SqlParameter();
                    paramRecUserType6.ParameterName = "@UserType";
                    paramRecUserType6.Value = userType;
                    cmdUserAlert.Parameters.Add(paramRecUserType6);



                    SqlParameter paramReceiverID7 = new SqlParameter();
                    paramReceiverID7.ParameterName = "@userid";
                    paramReceiverID7.Value = userID;
                    cmdSuggUsers.Parameters.Add(paramReceiverID7);


                    SqlParameter paramRecUserType7 = new SqlParameter();
                    paramRecUserType7.ParameterName = "@UserType";
                    paramRecUserType7.Value = userType;
                    cmdSuggUsers.Parameters.Add(paramRecUserType7);


                    SqlParameter paramReceiverID8 = new SqlParameter();
                    paramReceiverID8.ParameterName = "@userid";
                    paramReceiverID8.Value = userID;
                    cmdAdvert.Parameters.Add(paramReceiverID8);


                    SqlParameter paramRecUserType8 = new SqlParameter();
                    paramRecUserType8.ParameterName = "@UserType";
                    paramRecUserType8.Value = userType;
                    cmdAdvert.Parameters.Add(paramRecUserType8);

                    SqlParameter paramReceiverID9 = new SqlParameter();
                    paramReceiverID9.ParameterName = "@userid";
                    paramReceiverID9.Value = userID;
                    cmdAdcount.Parameters.Add(paramReceiverID9);


                    SqlParameter paramRecUserType9 = new SqlParameter();
                    paramRecUserType9.ParameterName = "@UserType";
                    paramRecUserType9.Value = userType;
                    cmdAdcount.Parameters.Add(paramRecUserType9);



                    SqlDataReader reader, readerin, readerall, readerusercnt, readerSercnt, readerEnqcnt,readercmntcnt,readerUserAlert, readerSuggUser,readerAdvert,readerAdcnt;
                    int collcnt = 0;
                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        userProfile.userID = (int)reader["userid"];
                        userProfile.userType = reader["usertype"].ToString().Trim();
                        userProfile.userName = reader["username"].ToString().Trim();
                        userProfile.postcode = reader["postcode"].ToString().Trim();
                        userProfile.address = reader["Address"].ToString().Trim();
                        userProfile.doj = reader["doj"].ToString().Trim();
                        userProfile.cServiceType = reader["cserviceType"].ToString().Trim();
                        userProfile.ServiceDesc = reader["serviceDesc"].ToString().Trim();
                        userProfile.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                        userProfile.ContactHide = reader["ContactHide"].ToString().Trim();
                        userProfile.user_level = reader["user_level"].ToString().Trim();
                        userProfile.logopath = reader["logopath"].ToString().Trim();
                        userProfile.isActive = reader["ActiveUser"].ToString().Trim();
                        userProfile.ReviewReceived = (int)reader["ReviewReceived"];
                        userProfile.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                        userProfile.msgLikeReceived = (int)reader["msgLikeReceived"];
                        userProfile.msgReadBy = (int)reader["msgReadBy"];
                        userProfile.msgTotpublished = (int)reader["msgTotpublished"];
                        userProfile.subscribersCnt = (int)reader["Subscribercnt"];
                        userProfile.first_page_announcement = reader["first_page_advertisement"].ToString();


                    }
                    reader.Close();
                    /*********GET INBOX MSG COUNTS **************************/
                    readerin = cmdin.ExecuteReader();
                    userProfile.newCountsbyTitle = new List<cUpdatecount>();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "Subscribers Post";
                    List<allids> ids = new List<allids>();
                    //ids.Add("0");
                    allids all;
                    while (readerin.Read())
                    {
                        all  = new allids();
                        all.id = readerin["messageid"].ToString();
                        all.key = 0;
                        all.type = "";
                        ids.Add(all);
                        
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = ids;
                    userProfile.newCountsbyTitle[collcnt].count = ids.Count;
                    readerin.Close();

                    /*********GET OTHER MSG COUNTS **************************/
                    collcnt++;
                    readerall = cmdall.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "Others Post";
                    List<allids> idsO = new List<allids>();
                    //idsO.Add("0");
                    while (readerall.Read())
                    {
                        all = new allids();
                        all.id = readerall["messageid"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsO.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsO;
                    userProfile.newCountsbyTitle[collcnt].count = idsO.Count;
                    readerall.Close();



                    /*********GET NEW SERVICE PROVIDER  COUNTS **************************/
                    collcnt++;
                    readerSercnt = cmdSercount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "SERVICE COUNT";
                    List<allids> idsS = new List<allids>();
                    //idsS.Add("0");
                    while (readerSercnt.Read())
                    {
                        all = new allids();
                        all.id = readerSercnt["senderid"].ToString();
                        all.key = (int) readerSercnt["ID"];
                        all.type = "";
                        idsS.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsS;
                    userProfile.newCountsbyTitle[collcnt].count = idsS.Count;
                    readerSercnt.Close();


                    /*********GET NEW ENQUIRY COUNTS **************************/
                    collcnt++;
                    readerEnqcnt = cmdenqcount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "ENQUIRY COUNT";
                    List<allids> idsE = new List<allids>();
                    //idsE.Add("0");
                    while (readerEnqcnt.Read())
                    {
                        all = new allids();
                        all.id = readerEnqcnt["enquiryid"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsE.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsE;
                    userProfile.newCountsbyTitle[collcnt].count = idsE.Count;
                    readerEnqcnt.Close();


                    /*********GET TOTAL USERS COUNTS **************************/
                   /* collcnt++;
                    readerusercnt = cmdusercount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "TOTAL COUNT";
                    List<allids> idsU = new List<allids>();
                    //idsU.Add("0");
                    while (readerusercnt.Read())
                    {
                        all = new allids();
                        all.id = readerusercnt["TOT_USERS"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsU.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsU;
                    userProfile.newCountsbyTitle[collcnt].count = 1;
                    readerusercnt.Close();
                    */
                    /*********GET New Comments  COUNTS **************************/
                    collcnt++;
                    readercmntcnt = cmdCmntcount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "COMMENT COUNT";
                    List<allids> idsCm = new List<allids>();
                    //idsU.Add("0");
                    while (readercmntcnt.Read())
                    {
                        all = new allids();
                        all.id = readercmntcnt["messageid"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsCm.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsCm;
                    userProfile.newCountsbyTitle[collcnt].count = idsCm.Count;
                    readercmntcnt.Close();


                    /*********GET USER ALERT  **************************/
                    collcnt++;
                    readerUserAlert = cmdUserAlert.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "ALERT MESSAGE";
                    List<allids> idsUA = new List<allids>();
                    while (readerUserAlert.Read())
                    {
                        all = new allids();
                        all.id = readerUserAlert["userAlert"].ToString();
                        all.key = 0;
                        all.type = readerUserAlert["userAlertAction"].ToString();
                        idsUA.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsUA;
                    userProfile.newCountsbyTitle[collcnt].count = idsUA.Count;
                    readerUserAlert.Close();

                    /*********GET VERSION DETAIL **************************/
                    collcnt++;
                    cmdVersion.ExecuteNonQuery();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "VERSION";
                    List<allids> idsV = new List<allids>();
                    all = new allids();
                    all.id = cmdVersion.Parameters["@version"].Value.ToString();
                    all.key = 0;
                    all.type = "";
                    idsV.Add(all);

                    userProfile.newCountsbyTitle[collcnt].ids = idsV;
                    userProfile.newCountsbyTitle[collcnt].count = idsV.Count;

                    /*********GET NEW Advertisement count   **************************/
                    collcnt++;
                    readerAdcnt = cmdAdcount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "ADS COUNT";
                    List<allids> idsAdcnt = new List<allids>();
                    while (readerAdcnt.Read())
                    {
                        all = new allids();
                        all.id = readerAdcnt["cnt"].ToString();
                        userProfile.newCountsbyTitle[collcnt].count = (int) readerAdcnt["cnt"];
                        all.key = 0;
                        all.type = "ADS COUNT";
                        idsAdcnt.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsAdcnt;
                   
                    readerAdcnt.Close();


                    /*********GET SUGGESTIONED USERS   **************************/
                    readerSuggUser = cmdSuggUsers.ExecuteReader();
                    userProfile.SuggUsers = new List<cSuggestUsers>();
                   // userProfile.SuggUsers.Add(new cSuggestUsers());
                    List<cSuggestUsers> idsSU = new List<cSuggestUsers>();
                    cSuggestUsers csugg;
                    while (readerSuggUser.Read())
                    {
                        csugg = new cSuggestUsers();
                        csugg.userID = readerSuggUser["SuggUserID"].ToString();
                        csugg.userName = readerSuggUser["SuggUserName"].ToString();
                        csugg.logopath = readerSuggUser["userlogopath"].ToString();
                        csugg.ServiceDesc = readerSuggUser["BusinessDesc"].ToString();
                        csugg.followerscnt = (int)readerSuggUser["followers"];
                        csugg.userType = readerSuggUser["suggusertype"].ToString();
                        idsSU.Add(csugg);
                    }
                    userProfile.SuggUsers = idsSU;
                    readerSuggUser.Close();
                    /*if suggestions users there, then no need to show advertisement */
                    if (userProfile.SuggUsers.Count !=  0 )
                    {
                        userProfile.first_page_announcement = "";
                    }
                    else
                    {
                        /*********GET FIRSTPAGE ADVERTISEMENT   **************************/
                       /* if (userProfile.first_page_announcement.Length == 0)
                        {*/
                            readerAdvert = cmdAdvert.ExecuteReader();

                            while (readerAdvert.Read())
                            {
                                userProfile.first_page_announcement = readerAdvert["imagePath"].ToString();
                            }

                            readerAdvert.Close();
                        //}
                    }
                    



                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return userProfile;


        }



        [Route("api/GetCountsAlert")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(cUserProfile))]
        public async Task<cUserProfile> GetCountsAlert(string userID, string userType)
        {
            cUserProfile userProfile = new cUserProfile();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {


                    //SqlCommand cmd = new SqlCommand("sp_getUserProfile", con);
                    //cmd.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdin = new SqlCommand("sp_Inbox_Msg_count", con);
                    cmdin.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdall = new SqlCommand("sp_Others_All_Msg_count", con);
                    cmdall.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdusercount = new SqlCommand("sp_user_count", con);
                    cmdusercount.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdSercount = new SqlCommand("sp_Service_user_count", con);
                    cmdSercount.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdCmntcount = new SqlCommand("sp_new_comments_count", con);
                    cmdCmntcount.CommandType = CommandType.StoredProcedure;


                   // SqlCommand cmdUserAlert = new SqlCommand("sp_user_alert", con);
                   // cmdUserAlert.CommandType = CommandType.StoredProcedure;


                    SqlCommand cmdenqcount = new SqlCommand("sp_enquiry_New_count", con);
                    cmdenqcount.CommandType = CommandType.StoredProcedure;

                    //SqlCommand cmdSuggUsers = new SqlCommand("sp_get_suggested_service_providers", con);
                    //cmdSuggUsers.CommandType = CommandType.StoredProcedure;


                    //SqlCommand cmdAdvert = new SqlCommand("sp_get_firstPage_Advertisement", con);
                    //cmdAdvert.CommandType = CommandType.StoredProcedure;


                    //SqlCommand cmdVersion = new SqlCommand("sp_get_version", con);
                    //cmdVersion.CommandType = CommandType.StoredProcedure;

                    SqlCommand cmdAdcount = new SqlCommand("sp_Advertisement_New_count", con);
                    cmdAdcount.CommandType = CommandType.StoredProcedure;


                    //cmdVersion.Parameters.Add("@version", SqlDbType.NVarChar, 5);
                    //cmdVersion.Parameters["@version"].Direction = ParameterDirection.Output;


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = userID;
                    cmdin.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverID1 = new SqlParameter();
                    paramReceiverID1.ParameterName = "@ReceiverID";
                    paramReceiverID1.Value = userID;
                    cmdall.Parameters.Add(paramReceiverID1);

                    SqlParameter paramRecUserType = new SqlParameter();
                    paramRecUserType.ParameterName = "@RecUserType";
                    paramRecUserType.Value = userType;
                    cmdin.Parameters.Add(paramRecUserType);

                    SqlParameter paramRecUserType1 = new SqlParameter();
                    paramRecUserType1.ParameterName = "@RecUserType";
                    paramRecUserType1.Value = userType;
                    cmdall.Parameters.Add(paramRecUserType1);



                    /*SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userid";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);
                    */

                    SqlParameter paramReceiverID3 = new SqlParameter();
                    paramReceiverID3.ParameterName = "@ReceiverID";
                    paramReceiverID3.Value = userID;
                    cmdSercount.Parameters.Add(paramReceiverID3);

                    SqlParameter paramRecUserType3 = new SqlParameter();
                    paramRecUserType3.ParameterName = "@RecUserType";
                    paramRecUserType3.Value = userType;
                    cmdSercount.Parameters.Add(paramRecUserType3);

                    SqlParameter paramReceiverID4 = new SqlParameter();
                    paramReceiverID4.ParameterName = "@ReceiverID";
                    paramReceiverID4.Value = userID;
                    cmdenqcount.Parameters.Add(paramReceiverID4);

                    SqlParameter paramRecUserType4 = new SqlParameter();
                    paramRecUserType4.ParameterName = "@RecUserType";
                    paramRecUserType4.Value = userType;
                    cmdenqcount.Parameters.Add(paramRecUserType4);

                    SqlParameter paramReceiverID5 = new SqlParameter();
                    paramReceiverID5.ParameterName = "@userid";
                    paramReceiverID5.Value = userID;
                    cmdCmntcount.Parameters.Add(paramReceiverID5);


                    SqlParameter paramRecUserType5 = new SqlParameter();
                    paramRecUserType5.ParameterName = "@UserType";
                    paramRecUserType5.Value = userType;
                    cmdCmntcount.Parameters.Add(paramRecUserType5);

                    /*
                    SqlParameter paramReceiverID6 = new SqlParameter();
                    paramReceiverID6.ParameterName = "@userid";
                    paramReceiverID6.Value = userID;
                    cmdUserAlert.Parameters.Add(paramReceiverID6);


                    SqlParameter paramRecUserType6 = new SqlParameter();
                    paramRecUserType6.ParameterName = "@UserType";
                    paramRecUserType6.Value = userType;
                    cmdUserAlert.Parameters.Add(paramRecUserType6);
                    */

                    /*
                    SqlParameter paramReceiverID7 = new SqlParameter();
                    paramReceiverID7.ParameterName = "@userid";
                    paramReceiverID7.Value = userID;
                    cmdSuggUsers.Parameters.Add(paramReceiverID7);


                    SqlParameter paramRecUserType7 = new SqlParameter();
                    paramRecUserType7.ParameterName = "@UserType";
                    paramRecUserType7.Value = userType;
                    cmdSuggUsers.Parameters.Add(paramRecUserType7);
                    */
                    /*
                    SqlParameter paramReceiverID8 = new SqlParameter();
                    paramReceiverID8.ParameterName = "@userid";
                    paramReceiverID8.Value = userID;
                    cmdAdvert.Parameters.Add(paramReceiverID8);


                    SqlParameter paramRecUserType8 = new SqlParameter();
                    paramRecUserType8.ParameterName = "@UserType";
                    paramRecUserType8.Value = userType;
                    cmdAdvert.Parameters.Add(paramRecUserType8);
                    */
                    SqlParameter paramReceiverID9 = new SqlParameter();
                    paramReceiverID9.ParameterName = "@userid";
                    paramReceiverID9.Value = userID;
                    cmdAdcount.Parameters.Add(paramReceiverID9);


                    SqlParameter paramRecUserType9 = new SqlParameter();
                    paramRecUserType9.ParameterName = "@UserType";
                    paramRecUserType9.Value = userType;
                    cmdAdcount.Parameters.Add(paramRecUserType9);



                    SqlDataReader reader, readerin, readerall, readerusercnt, readerSercnt, readerEnqcnt, readercmntcnt, readerUserAlert, readerSuggUser, readerAdvert, readerAdcnt;
                    int collcnt = 0;
                    con.Open();
                    //reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();

                    /*while (reader.Read())
                    {
                        userProfile.userID = (int)reader["userid"];
                        userProfile.userType = reader["usertype"].ToString().Trim();
                        userProfile.userName = reader["username"].ToString().Trim();
                        userProfile.postcode = reader["postcode"].ToString().Trim();
                        userProfile.address = reader["Address"].ToString().Trim();
                        userProfile.doj = reader["doj"].ToString().Trim();
                        userProfile.cServiceType = reader["cserviceType"].ToString().Trim();
                        userProfile.ServiceDesc = reader["serviceDesc"].ToString().Trim();
                        userProfile.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                        userProfile.ContactHide = reader["ContactHide"].ToString().Trim();
                        userProfile.user_level = reader["user_level"].ToString().Trim();
                        userProfile.logopath = reader["logopath"].ToString().Trim();
                        userProfile.isActive = reader["ActiveUser"].ToString().Trim();
                        userProfile.ReviewReceived = (int)reader["ReviewReceived"];
                        userProfile.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                        userProfile.msgLikeReceived = (int)reader["msgLikeReceived"];
                        userProfile.msgReadBy = (int)reader["msgReadBy"];
                        userProfile.msgTotpublished = (int)reader["msgTotpublished"];
                        userProfile.subscribersCnt = (int)reader["Subscribercnt"];
                        userProfile.first_page_announcement = reader["first_page_advertisement"].ToString();


                    }
                    reader.Close();*/
                    /*********GET INBOX MSG COUNTS **************************/
                    readerin = cmdin.ExecuteReader();
                    userProfile.newCountsbyTitle = new List<cUpdatecount>();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "Subscribers Post";
                    List<allids> ids = new List<allids>();
                    //ids.Add("0");
                    allids all;
                    while (readerin.Read())
                    {
                        all = new allids();
                        all.id = readerin["messageid"].ToString();
                        all.key = 0;
                        all.type = "";
                        ids.Add(all);

                    }
                    userProfile.newCountsbyTitle[collcnt].ids = ids;
                    userProfile.newCountsbyTitle[collcnt].count = ids.Count;
                    readerin.Close();

                    /*********GET OTHER MSG COUNTS **************************/
                    collcnt++;
                    readerall = cmdall.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "Others Post";
                    List<allids> idsO = new List<allids>();
                    //idsO.Add("0");
                    while (readerall.Read())
                    {
                        all = new allids();
                        all.id = readerall["messageid"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsO.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsO;
                    userProfile.newCountsbyTitle[collcnt].count = idsO.Count;
                    readerall.Close();



                    /*********GET NEW SERVICE PROVIDER  COUNTS **************************/
                    collcnt++;
                    readerSercnt = cmdSercount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "SERVICE COUNT";
                    List<allids> idsS = new List<allids>();
                    //idsS.Add("0");
                    while (readerSercnt.Read())
                    {
                        all = new allids();
                        all.id = readerSercnt["senderid"].ToString();
                        all.key = (int)readerSercnt["ID"];
                        all.type = "";
                        idsS.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsS;
                    userProfile.newCountsbyTitle[collcnt].count = idsS.Count;
                    readerSercnt.Close();


                    /*********GET NEW ENQUIRY COUNTS **************************/
                    collcnt++;
                    readerEnqcnt = cmdenqcount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "ENQUIRY COUNT";
                    List<allids> idsE = new List<allids>();
                    //idsE.Add("0");
                    while (readerEnqcnt.Read())
                    {
                        all = new allids();
                        all.id = readerEnqcnt["enquiryid"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsE.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsE;
                    userProfile.newCountsbyTitle[collcnt].count = idsE.Count;
                    readerEnqcnt.Close();


                    /*********GET TOTAL USERS COUNTS **************************/
                    /* collcnt++;
                     readerusercnt = cmdusercount.ExecuteReader();
                     userProfile.newCountsbyTitle.Add(new cUpdatecount());
                     userProfile.newCountsbyTitle[collcnt].title = "TOTAL COUNT";
                     List<allids> idsU = new List<allids>();
                     //idsU.Add("0");
                     while (readerusercnt.Read())
                     {
                         all = new allids();
                         all.id = readerusercnt["TOT_USERS"].ToString();
                         all.key = 0;
                         all.type = "";
                         idsU.Add(all);
                     }
                     userProfile.newCountsbyTitle[collcnt].ids = idsU;
                     userProfile.newCountsbyTitle[collcnt].count = 1;
                     readerusercnt.Close();
                     */
                    /*********GET New Comments  COUNTS **************************/
                    collcnt++;
                    readercmntcnt = cmdCmntcount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "COMMENT COUNT";
                    List<allids> idsCm = new List<allids>();
                    //idsU.Add("0");
                    while (readercmntcnt.Read())
                    {
                        all = new allids();
                        all.id = readercmntcnt["messageid"].ToString();
                        all.key = 0;
                        all.type = "";
                        idsCm.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsCm;
                    userProfile.newCountsbyTitle[collcnt].count = idsCm.Count;
                    readercmntcnt.Close();


                    /*********GET USER ALERT  **************************/
                   /* collcnt++;
                    readerUserAlert = cmdUserAlert.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "ALERT MESSAGE";
                    List<allids> idsUA = new List<allids>();
                    while (readerUserAlert.Read())
                    {
                        all = new allids();
                        all.id = readerUserAlert["userAlert"].ToString();
                        all.key = 0;
                        all.type = readerUserAlert["userAlertAction"].ToString();
                        idsUA.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsUA;
                    userProfile.newCountsbyTitle[collcnt].count = idsUA.Count;
                    readerUserAlert.Close();
                    */
                    /*********GET VERSION DETAIL **************************/
                   /* collcnt++;
                    cmdVersion.ExecuteNonQuery();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "VERSION";
                    List<allids> idsV = new List<allids>();
                    all = new allids();
                    all.id = cmdVersion.Parameters["@version"].Value.ToString();
                    all.key = 0;
                    all.type = "";
                    idsV.Add(all);

                    userProfile.newCountsbyTitle[collcnt].ids = idsV;
                    userProfile.newCountsbyTitle[collcnt].count = idsV.Count;
                    */
                    /*********GET NEW Advertisement count   **************************/
                    collcnt++;
                    readerAdcnt = cmdAdcount.ExecuteReader();
                    userProfile.newCountsbyTitle.Add(new cUpdatecount());
                    userProfile.newCountsbyTitle[collcnt].title = "ADS COUNT";
                    List<allids> idsAdcnt = new List<allids>();
                    while (readerAdcnt.Read())
                    {
                        all = new allids();
                        all.id = readerAdcnt["cnt"].ToString();
                        userProfile.newCountsbyTitle[collcnt].count = (int)readerAdcnt["cnt"];
                        all.key = 0;
                        all.type = "ADS COUNT";
                        idsAdcnt.Add(all);
                    }
                    userProfile.newCountsbyTitle[collcnt].ids = idsAdcnt;

                    readerAdcnt.Close();


                    /*********GET SUGGESTIONED USERS   **************************/
                   /* readerSuggUser = cmdSuggUsers.ExecuteReader();
                    userProfile.SuggUsers = new List<cSuggestUsers>();
                    // userProfile.SuggUsers.Add(new cSuggestUsers());
                    List<cSuggestUsers> idsSU = new List<cSuggestUsers>();
                    cSuggestUsers csugg;
                    while (readerSuggUser.Read())
                    {
                        csugg = new cSuggestUsers();
                        csugg.userID = readerSuggUser["SuggUserID"].ToString();
                        csugg.userName = readerSuggUser["SuggUserName"].ToString();
                        csugg.logopath = readerSuggUser["userlogopath"].ToString();
                        csugg.ServiceDesc = readerSuggUser["BusinessDesc"].ToString();
                        csugg.followerscnt = (int)readerSuggUser["followers"];
                        csugg.userType = readerSuggUser["suggusertype"].ToString();
                        idsSU.Add(csugg);
                    }
                    userProfile.SuggUsers = idsSU;
                    readerSuggUser.Close();
                   
                    if (userProfile.SuggUsers.Count != 0)
                    {
                        userProfile.first_page_announcement = "";
                    }
                    else
                    {
                       
                        readerAdvert = cmdAdvert.ExecuteReader();

                        while (readerAdvert.Read())
                        {
                            userProfile.first_page_announcement = readerAdvert["imagePath"].ToString();
                        }

                        readerAdvert.Close();
                       
                    }
                    */



                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return userProfile;


        }



        // POST: api/Message
        [Route("api/GetAllAdvertisements")]
        [HttpGet]
        [ResponseType(typeof(List<cAllAdvertisements>))]
        public async Task<List<cAllAdvertisements>> GetAllAdvertisements(String userID, String userType)
        {
            cStatus status = new cStatus();
            List<cAllAdvertisements> listAdverts = new List<cAllAdvertisements>();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_get_All_Advertisements", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlParameter paramuserID = new SqlParameter();
                    paramuserID.ParameterName = "@userID";
                    paramuserID.Value = userID;
                    cmd.Parameters.Add(paramuserID);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    con.Open();
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        cAllAdvertisements c = new cAllAdvertisements();
                        c.SenderName = reader["senderName"].ToString().Trim();
                        c.PhoneNo= reader["sendercontactno_1"].ToString().Trim();
                        c.imagePath = reader["imagepath"].ToString().Trim();
                        c.datePublised = (DateTime)reader["datepublished"];
                        listAdverts.Add(c);
                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }
            return listAdverts;
        }


        /*[Route("api/DeleteUser")]
        [HttpGet]
        [AllowAnonymous]
        public async Task GetDeleteUser(string phoneNo)
        {
           
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_deleteUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramphoneNo = new SqlParameter();
                    paramphoneNo.ParameterName = "@phoneNo";
                    paramphoneNo.Value = phoneNo;
                    cmd.Parameters.Add(paramphoneNo);

              
                    con.Open();
                    cmd.ExecuteNonQuery();
                    //retvalue = cmd.Parameters["@retval"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

           
        }
    */

        [Route("api/ResetLastopen")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> ResetLastopen(string phoneNo, int hours_goback)
        {

           try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_reset_for_notification", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                   SqlParameter paramphoneNo = new SqlParameter();
                    paramphoneNo.ParameterName = "@phoneno";
                    paramphoneNo.Value = phoneNo;
                    cmd.Parameters.Add(paramphoneNo);

                    SqlParameter paramgoback = new SqlParameter();
                    paramgoback.ParameterName = "@gobackhour";
                    paramgoback.Value = hours_goback;
                    cmd.Parameters.Add(paramgoback);

                    con.Open();
                    cmd.ExecuteNonQuery();
                  
                }
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

            return "Success";
        }


        [Route("api/CheckThandora")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus>  getChecksubscription([FromBody]cAuthorization cAuthorization)
        {
            cStatus status = new cStatus();

            string retvalue;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_checkSubscription", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramPhoneNo = new SqlParameter();
                    paramPhoneNo.ParameterName = "@PhoneNo";
                    paramPhoneNo.Value = cAuthorization.phoneNo;
                    cmd.Parameters.Add(paramPhoneNo);
                

                    cmd.Parameters.Add("@IME", SqlDbType.NVarChar, 300);
                    cmd.Parameters["@IME"].Direction = ParameterDirection.Output;

      
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@IME"].Value.ToString();
                    cAuthorization.deviceID = retvalue;
                    FCMController fcm = new FCMController();
                    SendNotificationController s = new SendNotificationController();
                    cStatus st = new cStatus();
                    if (cAuthorization.deviceID != null)
                     if (cAuthorization.deviceID.Length >1)
                            {
                        st = await s.GetfcmsubcriptioncheckAsync(cAuthorization.deviceID);
                        status.StatusID = st.StatusID;
                        status.StatusMsg = st.StatusMsg;
                        status.DesctoDev = st.DesctoDev;
                    }
                    else
                    {
                        status.StatusID = 1;
                            status.DesctoDev = "Never installed Thandora";


                        status.StatusMsg = "Not a user of Thandora";
                    }
                    fcm.PostFCMResponse(status.DesctoDev, status.StatusMsg + "@PhoneNo="+cAuthorization.phoneNo, (status.StatusID == 0) ? "Y" : "N");
                    
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return (status);
        }

        [Route("api/DeviceStatus")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> DeviceStatus()
        {
            cStatus status = new cStatus();

            string retvalue;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;
                string deviceid;
                int  senderID;

                using (SqlConnection Logcon = new SqlConnection(constr))
                {
                    SqlCommand logcmd = new SqlCommand("sp_add_log", Logcon);
                    logcmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramspname = new SqlParameter();
                    paramspname.ParameterName = "@spName";
                    logcmd.Parameters.Add(paramspname);

                    SqlParameter paramAction = new SqlParameter();
                    paramAction.ParameterName = "@Action";
                    logcmd.Parameters.Add(paramAction);

                    SqlParameter paramparameters = new SqlParameter();
                    paramparameters.ParameterName = "@parameters";
                    logcmd.Parameters.Add(paramparameters);

                    paramspname.Value = "API-DEVICE-STATUS";
                    paramAction.Value = "START";
                    paramparameters.Value = "";
                    Logcon.Open();
                    logcmd.ExecuteNonQuery();
                }


                    using (SqlConnection con = new SqlConnection(constr))
                    {


                    SqlCommand cmd = new SqlCommand("sp_FCM_reresh_allUsers", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    con.Open();
                    reader = cmd.ExecuteReader();


                    FCMController fcm = new FCMController();
                    SendNotificationController s = new SendNotificationController();
                    cStatus st = new cStatus();
                    while (reader.Read())
                    {

                        senderID = (int)reader["userid"];
                        deviceid = reader["fcm_registration"].ToString().Trim();


                        st = await s.GetfcmsubcriptioncheckAsync(deviceid);
                        /* status.StatusID = st.StatusID;
                         status.StatusMsg = st.StatusMsg;
                         status.DesctoDev = st.DesctoDev;*/

                        using (SqlConnection fcmCon = new SqlConnection(constr))
                        {

                            SqlCommand cmdFCmUp = new SqlCommand("sp_fcm_status_update", fcmCon);
                            cmdFCmUp.CommandType = CommandType.StoredProcedure;

                            SqlParameter paramuserID = new SqlParameter();
                            paramuserID.ParameterName = "@userid";
                            cmdFCmUp.Parameters.Add(paramuserID);

                            SqlParameter paramfcmResponse = new SqlParameter();
                            paramfcmResponse.ParameterName = "@fcmResponse";
                            cmdFCmUp.Parameters.Add(paramfcmResponse);

                            SqlParameter paramstatusMsg = new SqlParameter();
                            paramstatusMsg.ParameterName = "@statusMsg";
                            cmdFCmUp.Parameters.Add(paramstatusMsg);

                            paramuserID.Value = senderID;
                            paramfcmResponse.Value = st.DesctoDev;
                            paramstatusMsg.Value = st.StatusMsg;
                            fcmCon.Open();
                            cmdFCmUp.ExecuteNonQuery();
                        }


                    }
                }
                using (SqlConnection Logcon = new SqlConnection(constr))
                {
                    SqlCommand FCMcmd = new SqlCommand("sp_fcm_status_update_history", Logcon);
                    FCMcmd.CommandType = CommandType.StoredProcedure;
                   
                    Logcon.Open();
                    FCMcmd.ExecuteNonQuery();
                }
                using (SqlConnection Logcon = new SqlConnection(constr))
                {
                    SqlCommand logcmd = new SqlCommand("sp_add_log", Logcon);
                    logcmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramspname = new SqlParameter();
                    paramspname.ParameterName = "@spName";
                    logcmd.Parameters.Add(paramspname);

                    SqlParameter paramAction = new SqlParameter();
                    paramAction.ParameterName = "@Action";
                    logcmd.Parameters.Add(paramAction);

                    SqlParameter paramparameters = new SqlParameter();
                    paramparameters.ParameterName = "@parameters";
                    logcmd.Parameters.Add(paramparameters);

                    paramspname.Value = "API-DEVICE-STATUS";
                    paramAction.Value = "END";
                    paramparameters.Value = "";
                    Logcon.Open();
                    logcmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
          
            return (status);
        }


        [Route("api/GetOTP")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> GetOTP(string phoneno)
        {
            string retvalue;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetOTP", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramPhoneNo = new SqlParameter();
                    paramPhoneNo.ParameterName = "@PhoneNo";
                    paramPhoneNo.Value = phoneno;
                    cmd.Parameters.Add(paramPhoneNo);

                    cmd.Parameters.Add("@OTP", SqlDbType.Int);
                    cmd.Parameters["@OTP"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@OTP"].Value.ToString();
                    
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return (retvalue);
        }


        [Route("api/RefreshUsers")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> RefreshUsers()
        {
            cStatus status = new cStatus();

            string retvalue;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_getPeopleCount", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@result", SqlDbType.Int);
                    cmd.Parameters["@result"].Direction = ParameterDirection.Output;


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                   retvalue = cmd.Parameters["@result"].Value.ToString();
                   

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return "Users refreshes completed";
        }


        [Route("api/BlockUser")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> BlockUser([FromBody]cBlockUser blockuser)
        {
            cStatus status = new cStatus();

            int retvalue;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_add_BlockListUser", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = blockuser.ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramRecUserType = new SqlParameter();
                    paramRecUserType.ParameterName = "@recUserType";
                    paramRecUserType.Value = blockuser.RecUserType;
                    cmd.Parameters.Add(paramRecUserType);


                    SqlParameter paramBlockedID = new SqlParameter();
                    paramBlockedID.ParameterName = "@BlockedUserID";
                    paramBlockedID.Value = blockuser.BlockedID;
                    cmd.Parameters.Add(paramBlockedID);


                    SqlParameter paramBlockedUserType = new SqlParameter();
                    paramBlockedUserType.ParameterName = "@blockedUserType";
                    paramBlockedUserType.Value = blockuser.BlockedUserType;
                    cmd.Parameters.Add(paramBlockedUserType);


                    SqlParameter paramMessageID = new SqlParameter();
                    paramMessageID.ParameterName = "@messageid";
                    paramMessageID.Value = blockuser.MessageID;
                    cmd.Parameters.Add(paramMessageID);

                    cmd.Parameters.Add("@result", SqlDbType.Int);
                    cmd.Parameters["@result"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = int.Parse( cmd.Parameters["@result"].Value.ToString());

                    if ( retvalue == -1)
                    {
                        status.StatusMsg = "CANNOT BLOCK  YOUR OWN";
                        status.StatusID = 1;
                    }
                    if (retvalue == -2)
                    {
                        status.StatusMsg = "Invalid User";
                        status.StatusID = 1;
                    }
                    if (retvalue == -3)
                    {
                        status.StatusMsg = "Invalid Blocked User";
                        status.StatusID = 1;
                    }
                    if (retvalue == -4)
                    {
                        status.StatusMsg = "CANNOT BLOCK  THANDORA ADMIN";
                        status.StatusID = 1;
                    }
                    if (retvalue == 0)
                    {
                        status.StatusMsg = "User Blocked Successfully";
                        status.StatusID = 0;
                    }

                }
            }
            catch (Exception ex)
            {
               
                status.StatusID = 1;
                status.StatusMsg = ex.Message.ToString(); 
            }
            return status;
        }


        [Route("api/CheckFCMSubscribed")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(cFCMSubscribed))]
        public async Task<cFCMSubscribed> postChecksubscription([FromBody]cFCMSubscribed cfcmsubscribed)
        {
            cStatus status = new cStatus();

            return cfcmsubscribed;

           
        }




        [Route("api/Login")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus>  LoginProcess([FromBody]cAuthorization cAuthorization)
        {
            cStatus status = new cStatus();

            string retvalue, userName;

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_check_phoneNo", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramPhoneNo = new SqlParameter();
                    paramPhoneNo.ParameterName = "@PhoneNo";
                    paramPhoneNo.Value = cAuthorization.phoneNo;
                    cmd.Parameters.Add(paramPhoneNo);

                    SqlParameter paramIME = new SqlParameter();
                    paramIME.ParameterName = "@IME";
                    paramIME.Value = cAuthorization.deviceID;
                    cmd.Parameters.Add(paramIME);


                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@UserType", SqlDbType.NVarChar, 1);
                    cmd.Parameters["@UserType"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@userName", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@userName"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@Result"].Value.ToString();

                    status.returnID = int.Parse(retvalue);
                    cStatus st = new cStatus();
                    FCMController fcm = new FCMController();

                    if (status.returnID == -1)
                    {
                        status.StatusID = 1;

                        status.StatusMsg = "Please Sign up ";
                        cAuthorization = PostcAuthorization(cAuthorization);
                        // string s = SendOTP("USER", cAuthorization.phoneNo, cAuthorization.AuthorizeKey); ///its working fine. commented for development completion. 
                        st = await SendOTPAsync("NEWUSER", cAuthorization.phoneNo, cAuthorization.AuthorizeKey); ///its working fine. commented for development completion. 
                        fcm.PostFCMResponse(st.DesctoDev, st.StatusMsg + "@phoneno=" + cAuthorization.phoneNo, (st.StatusID == 0) ? "Y" : "N");
                        status.StatusMsg = st.StatusMsg;
                        status.OTP = cAuthorization.AuthorizeKey;
                        

                       /* string s = "000";
                        if (s.Contains("13"))
                        {
                            status.StatusMsg = "Please Check your Phone Number ";
                            status.OTP = 0;
                        }
                        if (s.Equals("000"))
                        {
                            status.DesctoDev = "Not Existing user. Phone number verified. Show sign up Screen ";
                            status.OTP = cAuthorization.AuthorizeKey;
                        }*/
                    }
                    if (status.returnID >= 100000)
                    {
                        status.StatusID = 0;

                        userName = cmd.Parameters["@userName"].Value.ToString();
                        status.DesctoDev = userName;

                        status.userType = cmd.Parameters["@UserType"].Value.ToString();
                        if (status.userType.Equals("R"))
                            status.StatusMsg = " LISTENER user ";
                        if (status.userType.Equals("S"))
                            status.StatusMsg = " TELLER user ";

                        cAuthorization = PostcAuthorization(cAuthorization);
                        //s= SendOTP(userName, cAuthorization.phoneNo, cAuthorization.AuthorizeKey);  //its working..commented for development complete. 
                        st = await SendOTPAsync(userName, cAuthorization.phoneNo, cAuthorization.AuthorizeKey); ///its working fine. commented for development completion. 
                        fcm.PostFCMResponse(st.DesctoDev, st.StatusMsg + "@phoneno=" + cAuthorization.phoneNo, (st.StatusID == 0) ? "Y" : "N");

                        status.OTP = cAuthorization.AuthorizeKey;

                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return status;
        }


        [Route("api/SendOTP")]
        [AllowAnonymous]
        [ResponseType(typeof(String))]
        public   string SendOTP(string UserName, string phoneNo,int OTP)
        {
            WebRequest tRequest;
            String sResponseFromServer;
            tRequest = WebRequest.Create("http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=" + phoneNo + "&sid=SMSHUB&msg=Dear%20" + UserName + ",%20Welcome%20to%20my%20App%20name.%20Please%20enter%20this%20" + OTP.ToString() + "%20verification%20code%20in%20android%20app.&fl=0&gwid=2");
            tRequest.Method = "post";

         
            tRequest.ContentType = "application/json";


            tRequest.ContentLength = 0;

            try
            {
                tRequest.GetRequestStream();
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
                        sResponseFromServer = sResponseFromServer.Substring(13, 3);
                        tReader.Close();
                        dataStream.Close();
                        tResponse.Close();
                        /* if (sResponseFromServer.Contains(SenderTopicName))
                         {
                             sResponseFromServer = "Topic Created  in the Name of " + SenderTopicName;
                         }*/
                    }


                }
            }
            catch (Exception ex)
            {
                sResponseFromServer = ex.Message.ToString();
            }
            //sResponseFromServer = CheckTopicGroup(SenderTokenID, SenderTopicName);

            return sResponseFromServer;
        }


        [Route("api/SendOTPAsync")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> SendOTPAsync(string UserName, string phoneNo, int OTP)
        {
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            ctblFCM fcmDetails = new ctblFCM();
            fcmDetails = (ctblFCM)fcm.GetctblFCM(1);

            string uri;
          /*  if (UserName.Contains('&'))
            {
                UserName.Replace('&', ' ');
            }*/
            uri = "https://control.msg91.com/api/sendotp.php?authkey=155444Aqq1zmafugQ5938de07&mobile=91" + phoneNo + "&message=Welcome%20to%20THANDORA%2C%20Your%20OTP%20is%20" + OTP.ToString() + "&sender=THNDRA&otp=" + OTP.ToString();

            //uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneNo + "&sid=SMSHUB&msg=Dear%20" + UserName + ",%20Welcome%20to%20my%20App%20name.%20Please%20enter%20this%20" + OTP.ToString() + "%20verification%20code%20in%20android%20app.&fl=0&gwid=2";
           /* if (UserName == "NEWUSER")
            {
                uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneNo + "&sid=THNDRA&msg=Welcome%20to%20THANDORA,%20please%20enter%20" + OTP.ToString() + "%20to%20validate%20your%20mobile%20number.&fl=0&gwid=2";
                             
            }
            else
            {
                //uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneNo + "&sid=THNDRA&msg=Dear%20" + UserName + ",%20Please%20enter%20" + OTP.ToString() + "%20for%20THANDORA%20verification.&fl=0&gwid=2";

                uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneNo + "&sid=THNDRA&msg=Welcome%20to%20THANDORA,%20please%20enter%20" + OTP.ToString() + "%20to%20validate%20your%20mobile%20number.&fl=0&gwid=2";

            }*/

            //uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91"+ phoneNo + "&sid=SMSHUB&msg=Welcome%20to%20THANDORA,%20please%20enter%20" + OTP.ToString() + "%20to%20validate%20your%20mobile%20number.&fl=0&gwid=2";

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse<cSMSReponse> asyncHandle = await client.ExecuteGetTaskAsync<cSMSReponse>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();
               
                if (st.DesctoDev.Contains("\"ErrorMessage\":\"Success\""))
                {
                    st.StatusMsg = "SMS Sent successfully";
                    st.StatusID = 0;
                    st.returnID = 0;
                }
                else
                {
                    string str = st.DesctoDev.Substring(14, 3);
                    st.returnID = int.Parse(str);
                    st.StatusID = 1;
                    switch (st.returnID)
                    {
                        case 13:
                            st.StatusMsg = "Invalid Mobile number";
                            break;
                        case 21:
                            st.StatusMsg = "Insufficient credits";
                            break;
                    }
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

        [Route("api/SendSMSAdvPublish")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> SendSMSAdvPublish(string phoneNo)
        {
            cStatus st = new cStatus();
         
            string uri;
            /*  if (UserName.Contains('&'))
              {
                  UserName.Replace('&', ' ');
              }*/
            //uri = "https://control.msg91.com/api/sendotp.php?authkey=155444Aqq1zmafugQ5938de07&mobile=91" + phoneNo + "&message=Welcome%20to%20THANDORA%2C%20Your%20OTP%20is%20" + OTP.ToString() + "&sender=THNDRA&otp=" + OTP.ToString();

            uri = "https://control.msg91.com/api/sendhttp.php?authkey=155444Aqq1zmafugQ5938de07&mobiles=91" + phoneNo + "&message=Now%20your%20post%20available%20in%20Local%20Ads%20in%20Thandora%202.0.%20Please%20check%20in%20https://goo.gl/Hjkv6X&sender=THNDRA&route=4&country=91";


            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                IRestResponse<cSMSReponse> asyncHandle = await client.ExecuteGetTaskAsync<cSMSReponse>(request);
                //return asyncHandle.Content.ToString();
                st.DesctoDev = asyncHandle.Content.ToString();

                if (st.DesctoDev.Contains("\"ErrorMessage\":\"Success\""))
                {
                    st.StatusMsg = "SMS Sent successfully";
                    st.StatusID = 0;
                    st.returnID = 0;
                }
                else
                {
                    string str = st.DesctoDev.Substring(14, 3);
                    st.returnID = int.Parse(str);
                    st.StatusID = 1;
                    switch (st.returnID)
                    {
                        case 13:
                            st.StatusMsg = "Invalid Mobile number";
                            break;
                        case 21:
                            st.StatusMsg = "Insufficient credits";
                            break;
                    }
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

        [Route("api/SendSearchEnquirySMS")]
        [AllowAnonymous]
        public async Task SendSearchEnquirySMS(string phoneno, string searchWord, string distance)
        {
            cStatus st = new cStatus();
           
            string uri;
            /*  if (UserName.Contains('&'))
              {
                  UserName.Replace('&', ' ');
              }*/

            // uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneNo + "&sid=THNDRA&msg=Welcome%20to%20THANDORA,%20please%20enter%20" + OTP.ToString() + "%20to%20validate%20your%20mobile%20number.&fl=0&gwid=2";


            //uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneno + "&sid=THNDRA&msg=Thandora%20user%20found%20your%20name%20on%20searching%20%27"+ searchWord.ToString() + "%27%20from%20" + distance + ".%20Please%20visit%20enquiry%20menu%20in%20Thandora.%20Regards,%20Thandora%20Admin.&fl=0&gwid=2";

            uri = "http://cloud.smsindiahub.in/vendorsms/pushsms.aspx?user=Pajani&password=Iniyan04&msisdn=91" + phoneno + "&sid=THNDRA&msg=Dear%20Thandora%20User,%20You%20were%20suggested%20on%20search%20%27" + searchWord.ToString() + "%20%27%20in%20Thandora%20by%20a%20user%20from%20" + distance + ".%20Please%20check%20in%20Thandora%20-%3EEnquiries%20to%20know%20the%20details.%20Thank%20you.&fl=0&gwid=2";

            try
            {

                var client = new RestClient(uri);
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
              //  IRestResponse<cSMSReponse> asyncHandle = await client.ExecuteGetTaskAsync<cSMSReponse>(request);
                client.ExecuteGetTaskAsync<cSMSReponse>(request);

                
                //return asyncHandle.Content.ToString();
                /*st.DesctoDev = asyncHandle.Content.ToString();

                if (st.DesctoDev.Contains("\"ErrorMessage\":\"Success\""))
                {
                    st.StatusMsg = "SMS Sent successfully";
                    st.StatusID = 0;
                    st.returnID = 0;
                }
                else
                {
                    string str = st.DesctoDev.Substring(14, 3);
                    st.returnID = int.Parse(str);
                    st.StatusID = 1;
                    switch (st.returnID)
                    {
                        case 13:
                            st.StatusMsg = "Invalid Mobile number";
                            break;
                        case 21:
                            st.StatusMsg = "Insufficient credits";
                            break;
                    }
                }

                return st;*/

            }
            catch (Exception ex)
            {
                st.StatusID = 1;//  Notification failure
                st.StatusMsg = ex.Message.ToString();
                //return st;

            }

        }


    }
}