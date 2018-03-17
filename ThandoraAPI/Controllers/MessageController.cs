using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class MessageController : ApiController
    {
        public cmsgPostReturn msgPostRetrun = new cmsgPostReturn();
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/Message
        /*  public IQueryable<ctblMessage> GetctblMessages()
          {
              return db.ctblMessages;
          }
          */

        // GET: api/Message/5
        [Route("api/GetMessagesbyID")]
        [ResponseType(typeof(List<ctblReadMessage>))]
        public async Task<List<ctblReadMessage>> GetctblMessage(int MsgID, int ReceiverID, string RecUserType)
        {
            
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
       
            string retvalue = "1";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_messageReadByID", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;

                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);

                    SqlParameter paramMsgID = new SqlParameter();
                    paramMsgID.ParameterName = "@messageID";
                    paramMsgID.Value = MsgID;
                    cmd.Parameters.Add(paramMsgID);


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                  
                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();



                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments = readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper() == RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }

        [Route("api/GetInboxMessages")]
        [ResponseType(typeof(List<ctblReadMessage>))]
        public async Task<List<ctblReadMessage>> GetctblMessage(int ReceiverID, string RecUserType)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;
               
                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Inbox_Messages", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;

                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();
                     
                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished =reader["msgPublished"].ToString();
                        sp.msgUpdated = (DateTime)reader["msgUpdated"];



                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                    foreach( ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments= readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper()== RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }
                       
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }



        [Route("api/GetAllFavoritesMessages")]
        [HttpGet]
        [ResponseType(typeof(List<ctblReadMessage>))]
        public async Task<List<ctblReadMessage>> GetAllFavoritesMessages(int ReceiverID, string RecUserType)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_get_all_favorite_msgs", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;

                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@userId";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@userType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                   /* cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;
                    */
                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();
                        sp.msgUpdated = (DateTime)reader["msgUpdated"];



                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments = readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper() == RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }


        [Route("api/GetAdminMessages")]
        [ResponseType(typeof(List<ctblReadMessage>))]
        public async Task<List<ctblReadMessage>> GetAdminMessages(int ReceiverID, string RecUserType,string stime,string etime)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_All_Messages_for_Admin", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                  /*  SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;
                    
                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);
                    */

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    SqlParameter paramstime = new SqlParameter();
                    paramstime.ParameterName = "@stime";
                    paramstime.Value = stime;
                    cmd.Parameters.Add(paramstime);

                    SqlParameter parametime = new SqlParameter();
                    parametime.ParameterName = "@etime";
                    parametime.Value = etime;
                    cmd.Parameters.Add(parametime);


                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();
                        sp.msgUpdated = (DateTime)reader["msgUpdated"];
                        sp.PeopleLikedby = new List<cmsgLikedBy>();
                        sp.PeopleCommentedby = new List<cmsgCommentedBy>();


                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                   /* foreach (ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments = readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper() == RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }*/

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }



        [Route("api/GetUserlast10Posts")]
        [ResponseType(typeof(List<ctblReadMessage>))]
        public async Task<List<ctblReadMessage>> GetUserlast10Posts(int ReceiverID, string RecUserType)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_User_last_10_posts", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                      SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                      cmdLike.CommandType = CommandType.StoredProcedure;
                      SqlDataReader readerLike;


                      SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                      cmdcomment.CommandType = CommandType.StoredProcedure;
                      SqlDataReader readercomment;

                      SqlParameter parammsgId = new SqlParameter();
                      parammsgId.ParameterName = "@msgID";
                      cmdLike.Parameters.Add(parammsgId);


                      SqlParameter paramcmsgId = new SqlParameter();
                      paramcmsgId.ParameterName = "@msgID";
                      cmdcomment.Parameters.Add(paramcmsgId);
                      

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();
                        sp.msgUpdated = (DateTime)reader["msgUpdated"];
                        sp.PeopleLikedby = new List<cmsgLikedBy>();
                        sp.PeopleCommentedby = new List<cmsgCommentedBy>();


                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                     foreach (ctblReadMessage c in listReadMessages)
                     {
                         parammsgId.Value = c.MessageID;
                         readerLike = cmdLike.ExecuteReader();
                         c.PeopleLikedby = new List<cmsgLikedBy>();
                         while (readerLike.Read())
                         {
                             cmsgLikedBy cl = new cmsgLikedBy();
                             cl.likerName = readerLike["likername"].ToString();
                             cl.logopath = readerLike["logopath"].ToString();
                             cl.likerID = (int)readerLike["receiverid"];
                             cl.likedTime = readerLike["likedTime"].ToString();

                             c.PeopleLikedby.Add(cl);
                         }
                         c.likeCnt = c.PeopleLikedby.Count;
                         readerLike.Close();

                     }
                     foreach (ctblReadMessage c in listReadMessages)
                     {
                         paramcmsgId.Value = c.MessageID;
                         readercomment = cmdcomment.ExecuteReader();
                         c.PeopleCommentedby = new List<cmsgCommentedBy>();
                         while (readercomment.Read())
                         {
                             cmsgCommentedBy c2 = new cmsgCommentedBy();
                             c2.commenterName = readercomment["commenterName"].ToString();
                             c2.logopath = readercomment["logopath"].ToString();
                             c2.commenterID = (int)readercomment["receiverid"];
                             c2.comments = readercomment["comments"].ToString();
                             c2.commentTime = readercomment["commentTime"].ToString();
                             c2.commentID = (int)readercomment["commentID"];
                             c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper() == RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                             c.PeopleCommentedby.Add(c2);
                         }
                         c.cmntCnt = c.PeopleCommentedby.Count;
                         readercomment.Close();
                     }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }


        [Route("api/GetAllMessages")]
        [ResponseType(typeof(List<cAllMessages>))]
        public async Task<List<cAllMessages>> GetAllMessages(int ReceiverID, string RecUserType)
        {
            List<cAllMessages> listAllMessage = new List<cAllMessages>();
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Others_All_Messages", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;

                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();
                        sp.msgUpdated = (DateTime)reader["msgUpdated"];


                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments = readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper() == RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }
                    cMessageSepration allMsgNational = new cMessageSepration();
                    allMsgNational.listMessages = new List<ctblReadMessage>();
                    cMessageSepration allMsgState = new cMessageSepration();
                    allMsgState.listMessages = new List<ctblReadMessage>();
                    cMessageSepration allMsgDistrict = new cMessageSepration();
                    allMsgDistrict.listMessages = new List<ctblReadMessage>();
                    cMessageSepration allMsgLocal = new cMessageSepration();
                    allMsgLocal.listMessages = new List<ctblReadMessage>();

                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        if (c.msgDistributedto == "NATIONAL")
                        {

                            allMsgNational.count = allMsgNational.count + 1;
                            allMsgNational.listMessages.Add(c);

                        }
                        if (c.msgDistributedto == "STATE")
                        {

                            allMsgState.count = allMsgState.count + 1;
                            allMsgState.listMessages.Add(c);

                        }
                        if (c.msgDistributedto == "DISTRICT")
                        {

                            allMsgDistrict.count = allMsgDistrict.count + 1;
                            allMsgDistrict.listMessages.Add(c);

                        }
                        if (c.msgDistributedto == "LOCAL")
                        {

                            allMsgLocal.count = allMsgLocal.count + 1;
                            allMsgLocal.listMessages.Add(c);

                        }

                    }

                    cAllMessages allmessage = new cAllMessages();
                    allmessage.msgBroadcast = "NATIONAL";
                    allmessage.count = allMsgNational.count;
                    allmessage.listMessage = allMsgNational.listMessages;
                    listAllMessage.Add(allmessage);

                    cAllMessages allstatemessage = new cAllMessages();
                    allstatemessage.msgBroadcast = "STATE";
                    allstatemessage.count = allMsgState.count;
                    allstatemessage.listMessage = allMsgState.listMessages;
                    listAllMessage.Add(allstatemessage);

                    cAllMessages alldistmessage = new cAllMessages();
                    alldistmessage.msgBroadcast = "DISTRICT";
                    alldistmessage.count = allMsgDistrict.count;
                    alldistmessage.listMessage = allMsgDistrict.listMessages;
                    listAllMessage.Add(alldistmessage);

                    cAllMessages alllocalmessage = new cAllMessages();
                    alllocalmessage.msgBroadcast = "LOCAL";
                    alllocalmessage.count = allMsgLocal.count;
                    alllocalmessage.listMessage = allMsgLocal.listMessages;
                    listAllMessage.Add(alllocalmessage);




                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listAllMessage;
        }



        [Route("api/GetPostHistoryMessages")]
        [ResponseType(typeof(ctblReadMessage))]
        public async Task<List<ctblReadMessage>> GetPostHistoryMessages(int ReceiverID, string RecUserType)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_user_post_History", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;

                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@UserID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@UserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();



                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments = readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == ReceiverID && c.SenderuserType.ToUpper() == RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }



        [Route("api/GetExpiredMessages")]
        [ResponseType(typeof(ctblReadMessage))]
        public async Task<List<ctblReadMessage>> GetExpiredMessages(int ReceiverID, string RecUserType)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Messages_expired", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

               
                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@userID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@UserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                      
                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                       
                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();

                        listReadMessages.Add(sp);
                    }
                    reader.Close();
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }


        // GET: api/Message/5

        [Route("api/GetOtherMessageTitle")]
        [ResponseType(typeof(List<cmsgOverallDistributed>))]
        public async Task<List<cmsgOverallDistributed>> GetOtherMessageTitle(int ReceiverID, string RecUserType, string POSTCODE = null)
        {
            List<cmsgOverallDistributed> msgOverallDistributedList = new List<cmsgOverallDistributed>();
            List<cmsgforOthers> allcategoryList = new List<cmsgforOthers>();
            List<cmsgSender> allSendersList = new List<cmsgSender>();
            //listOtherMsgTitle. = new List<ctblReadMessage>();
            string retvalue = "1";
            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                /*if (msgTabMenu.Equals("UNREAD")) stored_procedure = "sp_Others_UnRead_Title_Messages";
                if (msgTabMenu.Equals("READ")) stored_procedure = "sp_Others_Read_Title_Messages";
                if (msgTabMenu.Equals("FAVORITE")) stored_procedure = "sp_Others_Favorite_Title_Messages";*/


                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Others_Title_Messages", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);


                    SqlParameter paramPOSTCODE = new SqlParameter();
                    paramPOSTCODE.ParameterName = "@POSTCODE";
                    paramPOSTCODE.Value = POSTCODE;
                    cmd.Parameters.Add(paramPOSTCODE);


                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    //retvalue = Int32.Parse(cmd.Parameters["@Result"]);



                    while (reader.Read())
                    {
                        cmsgOverallDistributed c = new cmsgOverallDistributed();
                        c.msgDistributedto = reader["msgDistributedto"].ToString().Trim();
                   
                        // c.displayorder= reader["cDisplayOrder"].ToString().Trim();
                        c.cnt = (int)reader["cnt"];
                        c.newFlag = reader["newFlag"].ToString().Trim();
                        c.msgCategoryList = new List<cmsgforOthers>();
                        msgOverallDistributedList.Add(c);

                    }
                    reader.NextResult();


                    while (reader.Read())
                    {
                        cmsgforOthers c = new cmsgforOthers();
                        c.msgDistributedto= reader["msgDistributedto"].ToString().Trim();
                        c.msgCategory = reader["cMessageCategory"].ToString().Trim();
                       // c.displayorder= reader["cDisplayOrder"].ToString().Trim();
                        c.cnt = (int)reader["cnt"];
                        c.newFlag = reader["newFlag"].ToString().Trim();
                        c.msgSenderList = new List<cmsgSender>();
                        allcategoryList.Add(c);
                       
                    }
                    reader.NextResult();
                   
                    while (reader.Read())
                    {
                        cmsgSender c = new cmsgSender();
                        c.msgDistributedto = reader["msgDistributedto"].ToString().Trim();
                        c.msgCategory = reader["msgCategory"].ToString().Trim();
                        c.sendername = reader["sendername"].ToString().Trim();
                        if (reader["senderid"] != null)
                            if (reader["senderid"].ToString().Length > 0)
                                c.senderid = (int)reader["senderid"];
                        c.senderusertype = reader["usertype"].ToString().Trim();
                        c.logopath = reader["logopath"].ToString().Trim();
                        c.cnt = (int)reader["cnt"];
                        c.newFlag = reader["newFlag"].ToString().Trim();
                        allSendersList.Add(c);
                       
                    }

                   foreach(cmsgSender s in allSendersList)
                    {
                        foreach(cmsgforOthers  c in allcategoryList)
                        {
                            if (s.msgCategory== c.msgCategory && s.msgDistributedto == c.msgDistributedto)
                            {
                                if (s.senderid.ToString().Trim() != null)
                                    if (s.senderid !=0)
                                        c.msgSenderList.Add(s);
                                    else
                                        c.cnt = c.cnt - 1;
                                break;
                            }
                        }
                    }


                    foreach (cmsgforOthers s in allcategoryList)
                    {
                        foreach (cmsgOverallDistributed c in msgOverallDistributedList)
                        {
                            if (s.msgDistributedto == c.msgDistributedto)
                            {
                                 c.msgCategoryList.Add(s);
                                 break;
                            }
                        }
                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }


            return msgOverallDistributedList;
        }

        [Route("api/GetOthersMessages")]
        [ResponseType(typeof(ctblReadMessage))]
        [HttpPost]
        public async Task<List<ctblReadMessage>> GetOthersMessagesforSender([FromBody]cReadOtherMessagesforSender crms)
        {
            List<ctblReadMessage> listReadMessages = new List<ctblReadMessage>();
            string retvalue = "1";

            //string stored_procedure = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Others_Messages_For_Sender", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlCommand cmdLike = new SqlCommand("sp_Msg_PeopleLikedBy", con);
                    cmdLike.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readerLike;


                    SqlCommand cmdcomment = new SqlCommand("sp_Msg_PeopleCommentedBy", con);
                    cmdcomment.CommandType = CommandType.StoredProcedure;
                    SqlDataReader readercomment;

                    SqlParameter parammsgId = new SqlParameter();
                    parammsgId.ParameterName = "@msgID";
                    cmdLike.Parameters.Add(parammsgId);


                    SqlParameter paramcmsgId = new SqlParameter();
                    paramcmsgId.ParameterName = "@msgID";
                    cmdcomment.Parameters.Add(paramcmsgId);


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = crms.ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@RecUserType";
                    paramReceiverUserType.Value = crms.RecUserType;
                    cmd.Parameters.Add(paramReceiverUserType);


                    SqlParameter paramSenderId = new SqlParameter();
                    paramSenderId.ParameterName = "@senderID";
                    paramSenderId.Value = crms.SenderId;
                    cmd.Parameters.Add(paramSenderId);

                    SqlParameter paramsenderUsertype = new SqlParameter();
                    paramsenderUsertype.ParameterName = "@senderusertype";
                    paramsenderUsertype.Value = crms.senderUsertype;
                    cmd.Parameters.Add(paramsenderUsertype);


                    SqlParameter parammsgCategory = new SqlParameter();
                    parammsgCategory.ParameterName = "@msgCategory";
                    parammsgCategory.Value = crms.msgCategory;
                    cmd.Parameters.Add(parammsgCategory);


                    SqlParameter parammsgDistributedTo = new SqlParameter();
                    parammsgDistributedTo.ParameterName = "@msgDistributedto";
                    parammsgDistributedTo.Value = crms.msgDistributedTo;
                    cmd.Parameters.Add(parammsgDistributedTo);


                    SqlParameter paramPostCode = new SqlParameter();
                    paramPostCode.ParameterName = "@POSTCODE";
                    paramPostCode.Value = crms.PostCode;
                    cmd.Parameters.Add(paramPostCode);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();
                    // retvalue = cmd.Parameters["@Result"].Value.ToString();

                    while (reader.Read())
                    {
                        ctblReadMessage sp = new ctblReadMessage();
                        sp.MessageID = (int)reader["MessageID"];
                        sp.SenderID = (int)reader["SenderID"];
                        sp.SenderName = reader["SenderName"].ToString().Trim();
                        sp.SenderuserType = reader["UserType"].ToString().Trim();

                        sp.contactHide = (Int16)reader["ContactHide"];
                        sp.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                        sp.logopath = reader["logopath"].ToString().Trim();
                        sp.msgCategory = reader["msgCategory"].ToString().Trim();
                        sp.msgContent = reader["msgContent"].ToString().Trim();
                        sp.msgDistributedto = reader["msgDistributedto"].ToString().Trim();

                        sp.Readflag = reader["Readflag"].ToString().Trim();
                        sp.Likeflag = reader["Likeflag"].ToString().Trim();
                        sp.Favflag = reader["FavFlag"].ToString().Trim();
                        sp.expiredFlag = reader["expiredFlag"].ToString().Trim();

                        sp.likeCnt = (int)reader["msgLikedBy"];
                        sp.readCnt = (int)reader["msgReadBy"];
                        sp.imagePath = reader["imagePath"].ToString().Trim();
                        sp.msgPublished = reader["msgPublished"].ToString();
                        sp.msgUpdated = (DateTime)reader["msgUpdated"];



                        listReadMessages.Add(sp);
                    }
                    reader.Close();

                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        parammsgId.Value = c.MessageID;
                        readerLike = cmdLike.ExecuteReader();
                        c.PeopleLikedby = new List<cmsgLikedBy>();
                        while (readerLike.Read())
                        {
                            cmsgLikedBy cl = new cmsgLikedBy();
                            cl.likerName = readerLike["likername"].ToString();
                            cl.logopath = readerLike["logopath"].ToString();
                            cl.likerID = (int)readerLike["receiverid"];
                            cl.likedTime = readerLike["likedTime"].ToString();

                            c.PeopleLikedby.Add(cl);
                        }
                        c.likeCnt = c.PeopleLikedby.Count;
                        readerLike.Close();

                    }
                    foreach (ctblReadMessage c in listReadMessages)
                    {
                        paramcmsgId.Value = c.MessageID;
                        readercomment = cmdcomment.ExecuteReader();
                        c.PeopleCommentedby = new List<cmsgCommentedBy>();
                        while (readercomment.Read())
                        {
                            cmsgCommentedBy c2 = new cmsgCommentedBy();
                            c2.commenterName = readercomment["commenterName"].ToString();
                            c2.logopath = readercomment["logopath"].ToString();
                            c2.commenterID = (int)readercomment["receiverid"];
                            c2.comments = readercomment["comments"].ToString();
                            c2.commentTime = readercomment["commentTime"].ToString();
                            c2.commentID = (int)readercomment["commentID"];
                            c2.is_delete_but_visible = (c.SenderID == crms.ReceiverID && c.SenderuserType.ToUpper()==crms.RecUserType.ToUpper()) ? "TRUE" : "FALSE";
                            c.PeopleCommentedby.Add(c2);
                        }
                        c.cmntCnt = c.PeopleCommentedby.Count;
                        readercomment.Close();
                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            return listReadMessages;
        }



        // POST: api/PostMessage
        /*   [Route("api/PostMessage")]
           [ResponseType(typeof(cStatus))]
           public async Task<IHttpActionResult> PostctblMessage([FromBody]ctblMessage ctblMessage)
           {
               cStatus status = new cStatus();
               string retvalue, senderName, broadcast, postcode;
               try
               {
                   string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                   using (SqlConnection con = new SqlConnection(constr))
                   {
                       SqlCommand cmd = new SqlCommand("sp_new_message", con);
                       cmd.CommandType = CommandType.StoredProcedure;

                       SqlParameter paramSenderID = new SqlParameter();
                       paramSenderID.ParameterName = "@userID";
                       paramSenderID.Value = ctblMessage.SenderID;
                       cmd.Parameters.Add(paramSenderID);

                       SqlParameter parammsgType = new SqlParameter();
                       parammsgType.ParameterName = "@msgCategory";
                       parammsgType.Value = ctblMessage.msgCategory;
                       cmd.Parameters.Add(parammsgType);

                       SqlParameter parammsgContent = new SqlParameter();
                       parammsgContent.ParameterName = "@msgContent";
                       parammsgContent.Value = ctblMessage.msgContent;
                       cmd.Parameters.Add(parammsgContent);

                       SqlParameter paramimagePath = new SqlParameter();
                       paramimagePath.ParameterName = "@userType";
                       paramimagePath.Value = ctblMessage.userType;
                       cmd.Parameters.Add(paramimagePath);

                       SqlParameter parammsgDistributedTo = new SqlParameter();
                       parammsgDistributedTo.ParameterName = "@msgDistributedTo";
                       parammsgDistributedTo.Value = ctblMessage.msgDistributedTo;
                       cmd.Parameters.Add(parammsgDistributedTo);


                       //SqlParameter parammsgResRecGroup = new SqlParameter();
                       //parammsgResRecGroup.ParameterName = "@msgResRecGroup";
                       //parammsgResRecGroup.Value = ctblMessage.msgResRecGroup;
                       //cmd.Parameters.Add(parammsgResRecGroup);

                       cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 50);
                       cmd.Parameters["@SenderName"].Direction = ParameterDirection.Output;

                       cmd.Parameters.Add("@BroadCast", SqlDbType.NVarChar, 1);
                       cmd.Parameters["@BroadCast"].Direction = ParameterDirection.Output;

                       cmd.Parameters.Add("@postcode", SqlDbType.NVarChar, 6);
                       cmd.Parameters["@postcode"].Direction = ParameterDirection.Output;


                       cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                       cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                       con.Open();
                       cmd.ExecuteNonQuery();
                       con.Close();
                       retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                       status.returnID = int.Parse(retvalue);

                       senderName = cmd.Parameters["@SenderName"].Value.ToString();
                       broadcast = cmd.Parameters["@BroadCast"].Value.ToString();
                       postcode = cmd.Parameters["@postcode"].Value.ToString();

                       if (status.returnID == -1)
                       {
                           status.StatusID = 1;
                           status.DesctoDev = "TELLER IS INACTIVE ";
                           status.StatusMsg = "You are not active user. ";
                       }
                       if (status.returnID == -2)
                       {
                           status.StatusID = 1;
                           status.DesctoDev = "Cannot publish more than 3 message in a day";
                           status.StatusMsg = "You Reached Your Limit";
                       }
                       if (status.returnID == -3)
                       {
                           status.StatusID = 1;
                           status.DesctoDev = "Restricted word in your message. ";
                           status.StatusMsg = "Restricted words in your message ";
                       }

                       if (status.returnID >= 1)
                       {
                           status.StatusID = 0;
                           status.DesctoDev = "Message Saved. ";
                           status.StatusMsg = "Message Saved. ";


                           cStatus st = new cStatus();


                           if (broadcast.Equals("Y"))
                           {
                               ////SEND NOTIFICATION FOR BROADCAST MESSAGECATEGORY 

                               if (ctblMessage.msgDistributedTo.Equals("NATIONAL"))
                               {
                                   st = await PostMessageBroadcast("BROADCAST TO NATIONAL ","INDIA", senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                               }

                               if (ctblMessage.msgDistributedTo.Equals("STATE"))
                               {

                                   try
                                   {
                                       List<string> topics;
                                       topics = getTopicsforpostcode(postcode, "STATE");
                                       foreach (string topic in topics)
                                       {
                                           st = await PostMessageBroadcast("BROADCAST TO "+topic, topic, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                                       }
                                   }
                                   catch (Exception ex)
                                   {
                                       retvalue = ex.Message.ToString();
                                   }
                               }

                               if (ctblMessage.msgDistributedTo.Equals("DISTRICT"))
                               {
                                   try
                                   {
                                       List<string> topics;
                                       //postcode = "110006";
                                       topics = getTopicsforpostcode(postcode, "DISTRICT");
                                       foreach (string topic in topics)
                                       {
                                           st = await PostMessageBroadcast("BROADCAST TO " + topic,topic, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                                       }
                                   }
                                   catch (Exception ex)
                                   {
                                       retvalue = ex.Message.ToString();
                                   }
                               }

                               if (ctblMessage.msgDistributedTo.Equals("POSTCODE"))
                               {
                                   st = await PostMessageBroadcast("BROADCAST TO LOCAL ", postcode, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                               }

                               if (ctblMessage.msgDistributedTo.Equals("SUBSCRIBER"))
                               {
                                   st = await PostMessageBroadcast("BROADCAST TO SUBSCRIBER ",ctblMessage.SenderID.ToString(), senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                               }
                           }
                           else
                           {
                               st= await PostMessageBroadcast("BROADCAST TO SUBSCRIBER ",ctblMessage.SenderID.ToString(), senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                           }

                           status.StatusMsg = st.StatusMsg;
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
   */

        // POST: api/PostMessage
        [Route("api/PostMessage")]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostctblMessage([FromBody]ctblMessage ctblMessage)
        {
            cStatus status = new cStatus();
            string retvalue, senderName, broadcast, postcode;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_new_message", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@userID";
                    paramSenderID.Value = ctblMessage.SenderID;
                    cmd.Parameters.Add(paramSenderID);

                    SqlParameter parammsgType = new SqlParameter();
                    parammsgType.ParameterName = "@msgCategory";
                    parammsgType.Value = ctblMessage.msgCategory;
                    cmd.Parameters.Add(parammsgType);

                    SqlParameter parammsgContent = new SqlParameter();
                    parammsgContent.ParameterName = "@msgContent";
                    parammsgContent.Value = ctblMessage.msgContent;
                    cmd.Parameters.Add(parammsgContent);

                    SqlParameter paramimagePath = new SqlParameter();
                    paramimagePath.ParameterName = "@userType";
                    paramimagePath.Value = ctblMessage.userType;
                    cmd.Parameters.Add(paramimagePath);

                    SqlParameter parammsgDistributedTo = new SqlParameter();
                    parammsgDistributedTo.ParameterName = "@msgDistributedTo";
                    parammsgDistributedTo.Value = ctblMessage.msgDistributedTo;
                    cmd.Parameters.Add(parammsgDistributedTo);


                    /*SqlParameter parammsgResRecGroup = new SqlParameter();
                    parammsgResRecGroup.ParameterName = "@msgResRecGroup";
                    parammsgResRecGroup.Value = ctblMessage.msgResRecGroup;
                    cmd.Parameters.Add(parammsgResRecGroup);*/

                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@SenderName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@BroadCast", SqlDbType.NVarChar, 1);
                    cmd.Parameters["@BroadCast"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@postcode", SqlDbType.NVarChar, 6);
                    cmd.Parameters["@postcode"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //cmsgPostReturn msgPostRetrun = new cmsgPostReturn();

                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    status.returnID = int.Parse(retvalue);

                    senderName = cmd.Parameters["@SenderName"].Value.ToString();
                    broadcast = cmd.Parameters["@BroadCast"].Value.ToString();
                    postcode = cmd.Parameters["@postcode"].Value.ToString();

                    msgPostRetrun.broadCast = broadcast;
                    msgPostRetrun.retValue = status.returnID;
                    msgPostRetrun.sendername = senderName;
                    msgPostRetrun.POSTCODE = postcode;

                    if (status.returnID == -1)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "TELLER IS INACTIVE ";
                        status.StatusMsg = "You are not active user. ";
                    }
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Cannot publish more than 3 message in a day";
                        status.StatusMsg = "You Reached Your Limit";
                    }
                    if (status.returnID == -3)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Restricted word in your message. ";
                        status.StatusMsg = "Restricted words in your message ";
                    }

                    if (status.returnID >= 1)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "Message Saved. ";
                        status.StatusMsg = "Message Saved. ";

                        /*
                        cStatus st = new cStatus();

                        
                        if (broadcast.Equals("Y"))
                        {
                            ////SEND NOTIFICATION FOR BROADCAST MESSAGECATEGORY 

                            if (ctblMessage.msgDistributedTo.Equals("NATIONAL"))
                            {
                                st = await PostMessageBroadcast("INDIA", "INDIA", senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                            }

                            if (ctblMessage.msgDistributedTo.Equals("STATE"))
                            {

                                try
                                {
                                    List<string> topics;
                                    topics = getTopicsforpostcode(postcode, "STATE");
                                    foreach (string topic in topics)
                                    {
                                        st = await PostMessageBroadcast(topic, topic, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retvalue = ex.Message.ToString();
                                }
                            }

                            if (ctblMessage.msgDistributedTo.Equals("DISTRICT"))
                            {
                                try
                                {
                                    List<string> topics;
                                    //postcode = "110006";
                                    topics = getTopicsforpostcode(postcode, "DISTRICT");
                                    foreach (string topic in topics)
                                    {
                                        st = await PostMessageBroadcast(topic, topic, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retvalue = ex.Message.ToString();
                                }
                            }

                            if (ctblMessage.msgDistributedTo.Equals("POSTCODE"))
                            {
                                st = await PostMessageBroadcast("LOCAL ", postcode, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                            }

                            if (ctblMessage.msgDistributedTo.Equals("SUBSCRIBER"))
                            {
                                st = await PostMessageBroadcast("TO SUBSCRIBER ", ctblMessage.SenderID.ToString(), senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                            }
                        }
                        else
                        {
                            st = await PostMessageBroadcast("TO SUBSCRIBER ", ctblMessage.SenderID.ToString(), senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                        }

                        status.StatusMsg = st.StatusMsg;*/
                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }

            return status;

        }

        // POST: api/PostMessage
        [Route("api/EditPostMessage")]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> EditPostMessage([FromBody]ctblMessage ctblMessage)
        {
            cStatus status = new cStatus();
            string retvalue, senderName, broadcast, postcode;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_new_messagev1_3", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@userID";
                    paramSenderID.Value = ctblMessage.SenderID;
                    cmd.Parameters.Add(paramSenderID);


                    SqlParameter paramMsgID = new SqlParameter();
                    paramMsgID.ParameterName = "@msgID";
                    paramMsgID.Value = ctblMessage.MessageID;
                    cmd.Parameters.Add(paramMsgID);


                    SqlParameter parammsgType = new SqlParameter();
                    parammsgType.ParameterName = "@msgCategory";
                    parammsgType.Value = ctblMessage.msgCategory;
                    cmd.Parameters.Add(parammsgType);

                    SqlParameter parammsgContent = new SqlParameter();
                    parammsgContent.ParameterName = "@msgContent";
                    parammsgContent.Value = ctblMessage.msgContent;
                    cmd.Parameters.Add(parammsgContent);

                    SqlParameter paramimagePath = new SqlParameter();
                    paramimagePath.ParameterName = "@userType";
                    paramimagePath.Value = ctblMessage.userType;
                    cmd.Parameters.Add(paramimagePath);

                    SqlParameter parammsgDistributedTo = new SqlParameter();
                    parammsgDistributedTo.ParameterName = "@msgDistributedTo";
                    parammsgDistributedTo.Value = ctblMessage.msgDistributedTo;
                    cmd.Parameters.Add(parammsgDistributedTo);


                    SqlParameter parammsgimagePath = new SqlParameter();
                    parammsgimagePath.ParameterName = "@imagePath";
                    parammsgimagePath.Value = ctblMessage.imagePath;
                    cmd.Parameters.Add(parammsgimagePath);

                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@SenderName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@BroadCast", SqlDbType.NVarChar, 1);
                    cmd.Parameters["@BroadCast"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@postcode", SqlDbType.NVarChar, 6);
                    cmd.Parameters["@postcode"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    //cmsgPostReturn msgPostRetrun = new cmsgPostReturn();

                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    status.returnID = int.Parse(retvalue);

                    senderName = cmd.Parameters["@SenderName"].Value.ToString();
                    broadcast = cmd.Parameters["@BroadCast"].Value.ToString();
                    postcode = cmd.Parameters["@postcode"].Value.ToString();

                    msgPostRetrun.broadCast = broadcast;
                    msgPostRetrun.retValue = status.returnID;
                    msgPostRetrun.sendername = senderName;
                    msgPostRetrun.POSTCODE = postcode;

                    if (status.returnID == -1)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "TELLER IS INACTIVE ";
                        status.StatusMsg = "You are not active user. ";
                    }
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Cannot publish more than 3 message in a day";
                        status.StatusMsg = "You Reached Your Limit";
                    }
                    if (status.returnID == -3)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Restricted word in your message. ";
                        status.StatusMsg = "Restricted words in your message ";
                    }

                    if (status.returnID >= 1)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "Message Saved. ";
                        status.StatusMsg = "Message Saved. ";

                        /*
                        cStatus st = new cStatus();

                        
                        if (broadcast.Equals("Y"))
                        {
                            ////SEND NOTIFICATION FOR BROADCAST MESSAGECATEGORY 

                            if (ctblMessage.msgDistributedTo.Equals("NATIONAL"))
                            {
                                st = await PostMessageBroadcast("INDIA", "INDIA", senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                            }

                            if (ctblMessage.msgDistributedTo.Equals("STATE"))
                            {

                                try
                                {
                                    List<string> topics;
                                    topics = getTopicsforpostcode(postcode, "STATE");
                                    foreach (string topic in topics)
                                    {
                                        st = await PostMessageBroadcast(topic, topic, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retvalue = ex.Message.ToString();
                                }
                            }

                            if (ctblMessage.msgDistributedTo.Equals("DISTRICT"))
                            {
                                try
                                {
                                    List<string> topics;
                                    //postcode = "110006";
                                    topics = getTopicsforpostcode(postcode, "DISTRICT");
                                    foreach (string topic in topics)
                                    {
                                        st = await PostMessageBroadcast(topic, topic, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retvalue = ex.Message.ToString();
                                }
                            }

                            if (ctblMessage.msgDistributedTo.Equals("POSTCODE"))
                            {
                                st = await PostMessageBroadcast("LOCAL ", postcode, senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                            }

                            if (ctblMessage.msgDistributedTo.Equals("SUBSCRIBER"))
                            {
                                st = await PostMessageBroadcast("TO SUBSCRIBER ", ctblMessage.SenderID.ToString(), senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                            }
                        }
                        else
                        {
                            st = await PostMessageBroadcast("TO SUBSCRIBER ", ctblMessage.SenderID.ToString(), senderName, ctblMessage.msgCategory, ctblMessage.msgContent, int.Parse(retvalue));
                        }

                        status.StatusMsg = st.StatusMsg;*/
                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }

            return status;

        }


        // POST: api/PostMessage
        [Route("api/RePostMessage")]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> RePostMessage(int userID, string UserType,int messageID)
        {
            cStatus status = new cStatus();
            string retvalue, senderName, broadcast, postcode,msgDistributedto, msgCategory,msgContent;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_message_repost", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@userID";
                    paramSenderID.Value = userID;
                    cmd.Parameters.Add(paramSenderID);

                    SqlParameter paramimagePath = new SqlParameter();
                    paramimagePath.ParameterName = "@userType";
                    paramimagePath.Value = UserType;
                    cmd.Parameters.Add(paramimagePath);

                    SqlParameter parammsgDistributedTo = new SqlParameter();
                    parammsgDistributedTo.ParameterName = "@messageid";
                    parammsgDistributedTo.Value = messageID;
                    cmd.Parameters.Add(parammsgDistributedTo);


                    cmd.Parameters.Add("@SenderName", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@SenderName"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@BroadCast", SqlDbType.NVarChar, 1);
                    cmd.Parameters["@BroadCast"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@postcode", SqlDbType.NVarChar, 6);
                    cmd.Parameters["@postcode"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@msgCategory", SqlDbType.NVarChar, 60);
                    cmd.Parameters["@msgCategory"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@msgContent", SqlDbType.NVarChar, 100);
                    cmd.Parameters["@msgContent"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@msgDistributedto", SqlDbType.NVarChar, 15);
                    cmd.Parameters["@msgDistributedto"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@Result"].Value.ToString();
                    status.returnID = int.Parse(retvalue);

                    senderName = cmd.Parameters["@SenderName"].Value.ToString();
                    broadcast = cmd.Parameters["@BroadCast"].Value.ToString();
                    msgDistributedto = cmd.Parameters["@msgDistributedto"].Value.ToString();
                    msgCategory = cmd.Parameters["@msgCategory"].Value.ToString();
                    msgContent = cmd.Parameters["@msgContent"].Value.ToString();

                    postcode = cmd.Parameters["@postcode"].Value.ToString();

                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "TELLER IS INACTIVE ";
                        status.StatusMsg = "You are not active user. ";
                    }
                    if (status.returnID == -4)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Cannot publish more than 3 message in a day";
                        status.StatusMsg = "You Reached Your Limit";
                    }
                    if (status.returnID == -3)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Message is still active. ";
                        status.StatusMsg = "Message is still active. ";
                    }

                    if (status.returnID >= 1)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "Message Saved. ";
                        status.StatusMsg = "Message Saved. ";


                        cStatus st = new cStatus();


                        if (broadcast.Equals("Y"))
                        {
                            ////SEND NOTIFICATION FOR BROADCAST MESSAGECATEGORY 

                            if (msgDistributedto.Equals("NATIONAL"))
                            {
                                st = await PostMessageBroadcast("INDIA", "INDIA", senderName, msgCategory, msgContent, int.Parse(retvalue));
                            }

                            if (msgDistributedto.Equals("STATE"))
                            {

                                try
                                {
                                    List<string> topics;
                                    topics = getTopicsforpostcode(postcode, "STATE");
                                    foreach (string topic in topics)
                                    {
                                        st = await PostMessageBroadcast(topic, topic, senderName, msgCategory, msgContent, int.Parse(retvalue));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retvalue = ex.Message.ToString();
                                }
                            }

                            if (msgDistributedto.Equals("DISTRICT"))
                            {
                                try
                                {
                                    List<string> topics;
                                    //postcode = "110006";
                                    topics = getTopicsforpostcode(postcode, "DISTRICT");
                                    foreach (string topic in topics)
                                    {
                                        st = await PostMessageBroadcast(topic, topic, senderName, msgCategory, msgContent, int.Parse(retvalue));
                                    }
                                }
                                catch (Exception ex)
                                {
                                    retvalue = ex.Message.ToString();
                                }
                            }

                            if (msgDistributedto.Equals("POSTCODE"))
                            {
                                st = await PostMessageBroadcast("TO ", postcode, senderName, msgCategory, msgContent, int.Parse(retvalue));
                            }

                            if (msgDistributedto.Equals("SUBSCRIBER"))
                            {
                                st = await PostMessageBroadcast("TO FOLLOWERS ", userID.ToString(), senderName, msgCategory, msgContent, int.Parse(retvalue));
                            }
                        }
                        else
                        {
                            st = await PostMessageBroadcast("TO FOLLOWERS ", userID.ToString(), senderName, msgCategory, msgContent, int.Parse(retvalue));
                        }

                        status.StatusMsg = st.StatusMsg;
                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }

            return status;

        }


        // POST: api/PostMessage
        [HttpPost]
        [Route("api/PostMessageandImage")]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostMessageandImage()
        {
            cStatus status = new cStatus();
            StringBuilder sb = new StringBuilder();
            try

            {
                if (!Request.Content.IsMimeMultipartContent("form-data"))
                {
                    throw new InvalidOperationException("Media type not supported");
                 }

                var root = HttpContext.Current.Server.MapPath("~/App_Data");
                var provider = new MultipartFormDataStreamProvider(root);
                await Request.Content.ReadAsMultipartAsync(provider);

          
                /// this multipart code works fine. 
                /*foreach (MultipartFileData file in provider.FileData)
                {
                    Debug.WriteLine(file.Headers.ContentDisposition.FileName);
                    Debug.WriteLine("File path: " + file.LocalFileName);
                }*/

                // Form data
                //
                UploadImageController ui = new UploadImageController();
                
                foreach (var key in provider.FormData.AllKeys)
                {
                    foreach (var val in provider.FormData.GetValues(key))
                    {
                        ctblMessage msg = (ctblMessage)Newtonsoft.Json.JsonConvert.DeserializeObject(val,typeof(ctblMessage));
                        if (!msg.msgDistributedTo.Equals("SUBSCRIBER")) msg.msgDistributedTo = "NATIONAL"; /* INITIALLY ALL MESSAGE POSTED TO NATIONAL LEVEL. SO THAT EVERYUSERS SHOULD AWARE */
                        if (msg.MessageID > 0)
                        {
                            status = await EditPostMessage(msg);
                            ui.imageFileURL = msg.imagePath;////////the post already having image then should be kept the same image. 
                        }
                        else
                        {
                            status = await PostctblMessage(msg);
                            ui.imageFileURL = "";////////new image path will be created after saved the image in the path. 
                        }
                         
                        if (status.returnID > 0)
                        {
                            cStatus st = new cStatus();
                            string logmsg;


                            msgPostRetrun.msgContent = msg.msgContent;
                            msgPostRetrun.msgCategory = msg.msgCategory;
                            msgPostRetrun.msgDistributedTo = msg.msgDistributedTo;
                            msgPostRetrun.senderID = msg.SenderID;
                            msgPostRetrun.msgImagePath = "";

                            st = await ui.PostUserMessageImage(status.returnID);/////////////save the image in server path. 
                            //  Debug.WriteLine(string.Format("{0}: {1}", key, val));

                            if (st.DesctoDev != null)
                                if (st.DesctoDev.ToString().Length > 0)
                                    msgPostRetrun.msgImagePath = st.DesctoDev;
                                else
                                    msgPostRetrun.msgImagePath = "";
                            
                           
                            FCMController fcm = new FCMController();
                            SendNotificationController s = new SendNotificationController();
                            st = await s.PostMessageNotification(msgPostRetrun);
                         
                            logmsg = ":Distributedto=" + msg.msgDistributedTo + ":senderName:=" + msgPostRetrun.sendername  
                                + ":msgCategory=" + msg.msgCategory + ":msgContent=" + msg.msgContent + ":Messageid=" + msgPostRetrun.retValue.ToString();
                            if (st.DesctoDev.Contains("message_id"))
                            {
                                st.StatusMsg = "Broadcast success";
                                fcm.PostFCMResponse(st.DesctoDev, logmsg, "Y");
                            }
                            else
                            {
                                st.StatusMsg = "Broadcast failed";
                                fcm.PostFCMResponse(st.DesctoDev, logmsg, "N");
                            }
                           // postMessageforeachDevice(msgPostRetrun); ///////////////SOME DEVICES not getting messages while posting as TOPICS
                        }
                    }
                }
            

            }
            catch (Exception ex)
            {
                cStatus st = new cStatus();
                FCMController fcm = new FCMController();
                SendNotificationController s = new SendNotificationController();
                st = await s.PostMessageNotification(msgPostRetrun);

                status.StatusID = 0;
                status.StatusMsg= ex.Message.ToString();
                //postMessageforeachDevice(msgPostRetrun); ///////////////SOME DEVICES not getting messages while posting as TOPICS
            }
            CreateEnquiryToAdmin(msgPostRetrun);
            return status;
            
         

            // return Ok(status);

        }

        /********************ONCE MESSAGE POSTED, IT SHOULD CREATE AN ENQUIRY TO ADMIN TO REVIEW */
        [Route("api/CreatePostEnquiryToAdmin")]
        [HttpPost]
        [AllowAnonymous]

        public async Task CreateEnquiryToAdmin(cmsgPostReturn msgPosted)
        {
     
            MessageController mc = new MessageController();
          
            //System.Net.WebUtility.UrlEncode(serProviders[0].cSvcDesc);
            //HttpUtility.UrlEncode("search");
                ctblEnquiry cEnquiry = new ctblEnquiry();
                cEnquiry.FromUserID = msgPosted.senderID;
                cEnquiry.FromUserType = "S";
                cEnquiry.ToUserID = 100089;
                cEnquiry.ToUserType = "S";
                /* THIS SENTENCE SHOULD NOT BE CHANGED, IT USED IN SP sp_addEnquiryAnswer */
                string str;
            str = @"@Image " + msgPosted.msgImagePath + " \nPC:  " + msgPosted.POSTCODE +
            " \n@Dist To:@ " + msgPosted.msgDistributedTo +
            " \n\nSET ADV: https://joinwithme.in/ThandoraAPI/api/setAdvertismentbyAdmin?MessageID=" + msgPosted.retValue +
            " \n\nSET POSTCODE: https://joinwithme.in/ThandoraAPI/api/setMessageDistribution?MessageID=" + msgPosted.retValue +"&msgDistributedto=POSTCODE"+
            " \n\nSET DIST: https://joinwithme.in/ThandoraAPI/api/setMessageDistribution?MessageID=" + msgPosted.retValue+"&msgDistributedto=DISTRICT" +
           " \n\nSET STATE: https://joinwithme.in/ThandoraAPI/api/setMessageDistribution?MessageID=" + msgPosted.retValue+"&msgDistributedto=STATE" +

       " \n@Cont:@ " + msgPosted.msgContent;


            cEnquiry.EnquiryMessage = str;
                cEnquiry.EnquiryID = 0;
                mc.PostEnquiry(cEnquiry);
               

           
        }



        [HttpPost]
        [Route("api/postMessageforeachDevice")]
        [ResponseType(typeof(cStatus))]
        public async Task postMessageforeachDevice(cmsgPostReturn msgPostReturn)
        {
            string topic, deviceid;
            SendNotificationController s = new SendNotificationController();
           
                ////SEND NOTIFICATION FOR EACH DEVICE
                try
                {
                    string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                    using (SqlConnection con = new SqlConnection(constr))
                    {
                        SqlCommand cmd = new SqlCommand("sp_getDeviceIDs", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlDataReader reader;

                        SqlParameter paramPOSTCODE = new SqlParameter();
                        paramPOSTCODE.ParameterName = "@postcode";
                        paramPOSTCODE.Value = msgPostRetrun.POSTCODE;
                        cmd.Parameters.Add(paramPOSTCODE);

                        SqlParameter parammsgDistributedTo = new SqlParameter();
                        parammsgDistributedTo.ParameterName = "@msgDistributedTo";
                        parammsgDistributedTo.Value = msgPostRetrun.msgDistributedTo;
                        cmd.Parameters.Add(parammsgDistributedTo);

                        con.Open();
                        reader = cmd.ExecuteReader();
                        // retvalue = cmd.Parameters["@Result"].Value.ToString();

                        while (reader.Read())
                        {
                             topic = reader["topicName"].ToString().Trim();
                             deviceid = reader["IME"].ToString().Trim();

                            s.PostMessageforEachDeviceAsync(topic, deviceid, msgPostReturn.sendername, msgPostReturn.msgCategory, msgPostReturn.msgContent, msgPostReturn.msgImagePath, msgPostReturn.retValue,msgPostReturn.senderID);

                        }
                    }
                }
                catch
                {

                }
           
        }

        public List<string> getTopicsforpostcode(string postcode,string distributedto)
        {
            List<string> topicnames = new List<string>();

            try
            {
                string connstr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection conn = new SqlConnection(connstr))
                {

                    SqlCommand comd = new SqlCommand("sp_getTopicforPostcode", conn);
                    comd.CommandType = CommandType.StoredProcedure;

                    SqlParameter parampostcode = new SqlParameter();
                    parampostcode.ParameterName = "@POSTCODE";
                    parampostcode.Value = postcode;
                    comd.Parameters.Add(parampostcode);


                    SqlParameter paramSTATE = new SqlParameter();
                    paramSTATE.ParameterName = "@distributedTo";
                    paramSTATE.Value = distributedto;
                    comd.Parameters.Add(paramSTATE);

                    SqlDataReader reader;
                    conn.Open();
                    reader = comd.ExecuteReader();

                  
                    while (reader.Read())
                    {
                        topicnames.Add(reader["TopicName"].ToString());
 
                    }
                }
            }
            catch (Exception ex)
            {
                topicnames.Add(ex.Message.ToString());
            }
            return topicnames;

        }


        // POST: api/Message
        [Route("api/PostMessageBroadcast")]
        [ResponseType(typeof(cStatus))]
        private async Task<cStatus> PostMessageBroadcast(string titlebroadcast, string Distributedto,string senderName, string msgCategory,string msg,int msgid)
        {
            cStatus st = new cStatus();
            FCMController fcm = new FCMController();
            SendNotificationController s = new SendNotificationController();

            string logmsg;
            try
            {
                st = await s.PostNotificationAsync(titlebroadcast,Distributedto, senderName, msgCategory, msg, msgid);
                logmsg = ":Distributedto=" + Distributedto + ":senderName:=" + senderName + ":msgCategory=" + msgCategory + ":msgContent=" + msg + ":Messageid=" + msgid.ToString();
                if (st.DesctoDev.Contains("message_id"))
                {
                    st.StatusMsg = "Broadcast success";
                     fcm.PostFCMResponse(st.DesctoDev, logmsg, "Y");
                }
                else
                {
                    st.StatusMsg = "Broadcast failed";
                     fcm.PostFCMResponse(st.DesctoDev, logmsg, "N");
                }
            }
            catch(Exception e)
            {
                st.StatusMsg = e.Message.ToString();
                

            }
             return st;

        }

        // POST: api/Message
        [Route("api/PostEnquiry")]
        [ResponseType(typeof(cStatus))]
        public async Task<IHttpActionResult> PostEnquiry([FromBody]ctblEnquiry ctblEnquiry)
        {
            cStatus status = new cStatus();
            string retvalue, deviceID, fromUser;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addEnquiryAnswer", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramEnquiryID = new SqlParameter();
                    paramEnquiryID.ParameterName = "@EnquiryID";
                    paramEnquiryID.Value = ctblEnquiry.EnquiryID;
                    cmd.Parameters.Add(paramEnquiryID);

                    SqlParameter paramEnquiryMessage = new SqlParameter();
                    paramEnquiryMessage.ParameterName = "@EnquiryMessage";
                    paramEnquiryMessage.Value = ctblEnquiry.EnquiryMessage;
                    cmd.Parameters.Add(paramEnquiryMessage);

                    SqlParameter paramToUserID = new SqlParameter();
                    paramToUserID.ParameterName = "@ToUserID";
                    paramToUserID.Value = ctblEnquiry.ToUserID;
                    cmd.Parameters.Add(paramToUserID);


                    SqlParameter paramToUserType = new SqlParameter();
                    paramToUserType.ParameterName = "@ToUserType";
                    paramToUserType.Value = ctblEnquiry.ToUserType;
                    cmd.Parameters.Add(paramToUserType);

                    SqlParameter paramFromUserID = new SqlParameter();
                    paramFromUserID.ParameterName = "@FromUserID";
                    paramFromUserID.Value = ctblEnquiry.FromUserID;
                    cmd.Parameters.Add(paramFromUserID);

                    SqlParameter paramFromUserType = new SqlParameter();
                    paramFromUserType.ParameterName = "@FromUserType";
                    paramFromUserType.Value = ctblEnquiry.FromUserType;
                    cmd.Parameters.Add(paramFromUserType);



                    cmd.Parameters.Add("@LastEnquiryID", SqlDbType.Int);
                    cmd.Parameters["@LastEnquiryID"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@deviceID", SqlDbType.NVarChar, 300);
                    cmd.Parameters["@deviceID"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@fromUser", SqlDbType.NVarChar, 60);
                    cmd.Parameters["@fromUser"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@LastEnquiryID"].Value.ToString();
                    deviceID = cmd.Parameters["@deviceID"].Value.ToString();
                    fromUser = cmd.Parameters["@fromUser"].Value.ToString();
                    status.returnID = int.Parse(retvalue);

                    

                    if (status.returnID >= 1)
                    {
                        status.StatusID = 0;                    
                        status.StatusMsg = "Message Saved. ";
                        ////SEND NOTIFICATION  ONLY FOR SUBCRIBED TOPIC USERS. 
                        SendNotificationController s = new SendNotificationController();
                        int cnt = 0;
                        int orginEnquiryid = status.returnID;
                        string msg;
                        FCMController fcm = new FCMController();
                        cStatus st = new cStatus();
                        do
                        {

                            st = await s.EnquiryNotificationAsync(deviceID,  fromUser, ctblEnquiry.EnquiryMessage, orginEnquiryid, "Enquiry", ctblEnquiry.FromUserID);
                            msg = "@deviceID=" + deviceID + "@fromuser=" + fromUser + "@EnquiryMessage=" + ctblEnquiry.EnquiryMessage + "@Messageid=" + orginEnquiryid.ToString();
                            if (st.returnID == 1)
                            {
                                status.StatusMsg = "Message Saved and Sent";
                                 fcm.PostFCMResponse(st.DesctoDev, msg,"Y");

                            }
                            if (st.StatusID == 1)
                            {
                                status.StatusMsg = "FCM Notification failed";
                                 fcm.PostFCMResponse(st.DesctoDev, msg, "N");
                            }
                            /* statuse.returnID = 1 for success post. statuse.StatusID = 1 for failure post  */
                        } while (st.returnID !=1 && ++cnt<3) ;

                    }
                    else if (status.returnID ==-3)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Restricted word in enquiry message. ";
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


        // POST: api/Message
        [Route("api/GetEnquiries")]
        [ResponseType(typeof(List<ctblEnquiry>))]
        public async Task<IHttpActionResult> GetEnquiries(String userID, String userType)
        {
            cStatus status = new cStatus();
            List<ctblEnquiry> listEnquiries = new List<ctblEnquiry>();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_listEnquiries", con);
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
                        ctblEnquiry c = new ctblEnquiry();
                        c.EnquiryID = (int)reader["enquiryid"];
                        c.AnswerID = 0;
                        if (reader["AnswerID"] != null)
                            if (reader["AnswerID"].ToString().Length > 0)
                                c.AnswerID =(int) reader["AnswerID"];
                                    
                        c.ToUserID = (int)reader["touserid"];
                        c.ToUserType = reader["tousertype"].ToString().Trim();
                        c.ToUserName = reader["tousername"].ToString().Trim();
                        c.touserImage = reader["touserimage"].ToString().Trim();
                        c.FromUserID = (int)reader["fromuserid"];
                        c.FromUserType = reader["fromusertype"].ToString().Trim(); ;
                        c.FromUserName = reader["fromuserName"].ToString().Trim();
                        c.FromUserImage= reader["fromuserImage"].ToString().Trim();
                        c.EnquiryMessage = reader["enquirymessage"].ToString().Trim();
                        //  c.doc = (DateTime)reader["createdOn"];
                        c.doc =reader["createdOn"].ToString();
                        c.newflag = reader["new_flag"].ToString().Trim();
                        listEnquiries.Add(c);

                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }
            return Ok(listEnquiries);
        }


        // POST: api/Message
        [Route("api/GetEnquiriesbyID")]
        [HttpGet]
        [ResponseType(typeof(List<ctblEnquiry>))]
        public async Task<IHttpActionResult> GetEnquiriesbyId(int enquiryID, String userID, String userType)
        {
            cStatus status = new cStatus();
            List<ctblEnquiry> listEnquiries = new List<ctblEnquiry>();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_listEnquiriesbyid", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlParameter paramenquiryID = new SqlParameter();
                    paramenquiryID.ParameterName = "@enquiryid";
                    paramenquiryID.Value = enquiryID;
                    cmd.Parameters.Add(paramenquiryID);

                    SqlParameter parameuserID = new SqlParameter();
                    parameuserID.ParameterName = "@userID";
                    parameuserID.Value = userID;
                    cmd.Parameters.Add(parameuserID);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    con.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        ctblEnquiry c = new ctblEnquiry();
                        c.EnquiryID = (int)reader["enquiryid"];
                        c.AnswerID = 0;
                        if (reader["AnswerID"] != null)
                            if (reader["AnswerID"].ToString().Length > 0)
                                c.AnswerID = (int)reader["AnswerID"];

                        c.ToUserID = (int)reader["touserid"];
                        c.ToUserType = reader["tousertype"].ToString().Trim();
                        c.ToUserName = reader["tousername"].ToString().Trim();
                        c.touserImage= reader["touserimage"].ToString().Trim();
                        c.FromUserID = (int)reader["fromuserid"];
                        c.FromUserType = reader["fromusertype"].ToString().Trim(); ;
                        c.FromUserName = reader["fromuserName"].ToString().Trim();
                        c.FromUserImage= reader["fromuserimage"].ToString().Trim();
                        c.EnquiryMessage = reader["enquirymessage"].ToString().Trim();

                        c.doc = reader["createdOn"].ToString();
                        c.newflag = reader["new_flag"].ToString().Trim();

                        listEnquiries.Add(c);

                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }
            return Ok(listEnquiries);
        }

        // POST: api/Message
        [HttpDelete]
        [Route("api/GetEnquiriesbyID")]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> DeleteEnquiriesbyId(int enquiryID, String userID, String userType)
        {
            cStatus status = new cStatus();
            List<ctblEnquiry> listEnquiries = new List<ctblEnquiry>();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_DeleteEnquiriesbyid", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramenquiryID = new SqlParameter();
                    paramenquiryID.ParameterName = "@enquiryid";
                    paramenquiryID.Value = enquiryID;
                    cmd.Parameters.Add(paramenquiryID);

                    SqlParameter parameuserID = new SqlParameter();
                    parameuserID.ParameterName = "@userID";
                    parameuserID.Value = userID;
                    cmd.Parameters.Add(parameuserID);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    status.StatusID = 0;
                    status.StatusMsg = "Conversation deleted";



                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
            }
            return (status);
        }


        // POST: api/Message
        [Route("api/IsNewexists")]
        [ResponseType(typeof(cStatus))]
        public async Task<IHttpActionResult> GetIsnewexists(String userID, String userType)
        {
            cStatus status = new cStatus();
            List<ctblEnquiry> listEnquiries = new List<ctblEnquiry>();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_isNewexists", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                                  
                    SqlParameter parameuserID = new SqlParameter();
                    parameuserID.ParameterName = "@userID";
                    parameuserID.Value = userID;
                    cmd.Parameters.Add(parameuserID);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    retvalue = cmd.Parameters["@Result"].Value.ToString();
                    status.returnID = int.Parse(retvalue);

                    if (status.returnID == -1)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "Not Active user ";
                        status.StatusMsg = "NOTUSER";
                    }

                    if (status.returnID == 1)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "New Enquiry message available ";
                        status.StatusMsg = "ENQUIRY";
                    }

                    if (status.returnID == 2)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "New Inbox message available ";
                        status.StatusMsg = "INBOX";
                    }

                    if (status.returnID == 3)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "New Inbox & Enquiry message available ";
                        status.StatusMsg = "BOTH";
                    }

                    if (status.returnID == 0)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "No new messages ";
                        status.StatusMsg = "NoNew";
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


       
        [Route("api/PostMessageResponse")]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostMessageResponse([FromBody]cMsgResponse cmsgResponse)
        {
            string retvalue, userName, deviceID;
            int cmntID;
            cStatus status = new cStatus();
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_MessageResponse", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                   /* SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    paramSenderID.Value = cmsgResponse.SenderID;
                    cmd.Parameters.Add(paramSenderID);*/

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@userID";
                    paramReceiverID.Value = cmsgResponse.userID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramRecUserType = new SqlParameter();
                    paramRecUserType.ParameterName = "@userType";
                    paramRecUserType.Value = cmsgResponse.userType;
                    cmd.Parameters.Add(paramRecUserType);

                    SqlParameter paramMessageID = new SqlParameter();
                    paramMessageID.ParameterName = "@msgID";
                    paramMessageID.Value = cmsgResponse.MessageID;
                    cmd.Parameters.Add(paramMessageID);


                    SqlParameter paramcomments = new SqlParameter();
                    paramcomments.ParameterName = "@comments";
                    paramcomments.Value = cmsgResponse.comments;
                    cmd.Parameters.Add(paramcomments);

                    /*SqlParameter paramcmnt = new SqlParameter();
                    paramcmnt.ParameterName = "@nocmnt";
                    paramcmnt.Value = cmsgResponse.cmnt;
                    cmd.Parameters.Add(paramcmnt);*/

                    /*SqlParameter paramread = new SqlParameter();
                    paramread.ParameterName = "@noread";
                    paramread.Value = cmsgResponse.read;
                    cmd.Parameters.Add(paramread);*/


                    SqlParameter paramlike = new SqlParameter();
                    paramlike.ParameterName = "@nolike";
                    paramlike.Value = cmsgResponse.like;
                    cmd.Parameters.Add(paramlike);

                    SqlParameter paramfavflag = new SqlParameter();
                    paramfavflag.ParameterName = "@favflag";
                    paramfavflag.Value = cmsgResponse.favflag;
                    cmd.Parameters.Add(paramfavflag);

                    SqlParameter paramdelFlag = new SqlParameter();
                    paramdelFlag.ParameterName = "@delFlag";
                    paramdelFlag.Value = cmsgResponse.delFlag;
                    cmd.Parameters.Add(paramdelFlag);


                    cmd.Parameters.Add("@LastupdatedID", SqlDbType.Int);
                    cmd.Parameters["@LastupdatedID"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@cmntid", SqlDbType.Int);
                    cmd.Parameters["@cmntid"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@username", SqlDbType.NVarChar,50);
                    cmd.Parameters["@username"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar,300);
                    cmd.Parameters["@deviceid"].Direction = ParameterDirection.Output;



                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@LastupdatedID"].Value.ToString();
                    cmntID = int.Parse(cmd.Parameters["@cmntid"].Value.ToString());
                    userName = cmd.Parameters["@username"].Value.ToString();
                    deviceID = cmd.Parameters["@deviceid"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Invalid User in Thandora ";
                        status.StatusMsg = " Invalid user. ";
                    }
                    if (status.returnID == 1)
                    {
                        status.StatusID = 0;
                        status.StatusMsg = " Updated successfully ";
                       

                        if (cmntID > 0)
                        {
                            int cnt = 0;
                            int msgid = status.returnID;
                            string msg;
                           
                            FCMController fcm = new FCMController();
                            SendNotificationController s = new SendNotificationController();
                            cStatus st = new cStatus();
                            do
                            {

                                st = await s.EnquiryNotificationAsync(deviceID,  userName, cmsgResponse.comments, cmsgResponse.MessageID, "Comments", cmsgResponse.userID);
                                status.DesctoDev = st.DesctoDev;
                                msg = ":@deviceID=" + deviceID + ":@fromuser=" + userName + ":@message comments=" + cmsgResponse.comments + ":@Messageid= " + msgid.ToString() + ":@comment id=" + cmntID;
                                if (st.returnID == 1)
                                {
                                    status.StatusMsg = "Comment Saved and Sent";
                                    fcm.PostFCMResponse(st.DesctoDev, msg, "Y");

                                }
                                if (st.StatusID == 1)
                                {
                                    status.StatusMsg = "FCM Notification failed";
                                    fcm.PostFCMResponse(st.DesctoDev, msg, "N");
                                }
                                /* statuse.returnID = 1 for success post. statuse.StatusID = 1 for failure post  */
                            } while (st.returnID != 1 && ++cnt < 2);

                            status.returnID = cmntID; // COMMNETID required to handles the comments in UI locally as GP requested
                        }

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

        /********************ONCE ADMIN SET THE POST AS LOCAL ADVERTISEMENT, IT SHOULD CREATE AN ENQUIRY TO SENDER AND SMS TO SENDER */

        [Route("api/setAdvertismentbyAdmin")]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> setAdvertismentbyAdmin(int MessageID)
        {
            int senderId;
            string status,senderPhone;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addAdvertisements_byManual", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramMessageID = new SqlParameter();
                    paramMessageID.ParameterName = "@Messageid";
                    paramMessageID.Value = MessageID;
                    cmd.Parameters.Add(paramMessageID);

                    cmd.Parameters.Add("@Senderid", SqlDbType.Int);
                    cmd.Parameters["@Senderid"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@senderPhone", SqlDbType.NVarChar,15);
                    cmd.Parameters["@senderPhone"].Direction = ParameterDirection.Output;


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    status= "Success";
         
                    senderId = int.Parse(cmd.Parameters["@Senderid"].Value.ToString());
                    senderPhone = cmd.Parameters["@senderPhone"].Value.ToString();

                    EnqtoSendertosetAdv(senderId, senderPhone);

                }
            }
            catch (Exception ex)
            {
                status= ex.Message.ToString();
            }
            return status;
        }


        [Route("api/setMessageDistribution")]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> setMessageDistribution(int MessageID,string msgDistributedTo)
        {
            int senderId;
            string status, senderPhone;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_setMessageDistribution", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramMessageID = new SqlParameter();
                    paramMessageID.ParameterName = "@Messageid";
                    paramMessageID.Value = MessageID;
                    cmd.Parameters.Add(paramMessageID);

                    SqlParameter parammsgDistributedTo = new SqlParameter();
                    parammsgDistributedTo.ParameterName = "@msgDistributedTo";
                    parammsgDistributedTo.Value = msgDistributedTo;
                    cmd.Parameters.Add(parammsgDistributedTo);

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    status = "Success";

                 
                }
            }
            catch (Exception ex)
            {
                status = ex.Message.ToString();
            }
            return status;
        }


        /********************ONCE ADMIN SET THE POST AS LOCAL ADVERTISEMENT, IT SHOULD CREATE AN ENQUIRY TO SENDER AND SMS TO SENDER */
        [Route("api/EnqtoSendertosetAdv")]
        [HttpPost]
        [AllowAnonymous]

        public async Task EnqtoSendertosetAdv(int senderID,string senderPhone)
        {
            AuthorizationController ac = new AuthorizationController();
            MessageController mc = new MessageController();

            //System.Net.WebUtility.UrlEncode(serProviders[0].cSvcDesc);
            //HttpUtility.UrlEncode("search");
            ctblEnquiry cEnquiry = new ctblEnquiry();
            cEnquiry.FromUserID = 100089;
            cEnquiry.FromUserType = "S";
            cEnquiry.ToUserID = senderID;
            cEnquiry.ToUserType = "S";
            /* THIS SENTENCE SHOULD NOT BE CHANGED, IT USED IN SP sp_addEnquiryAnswer */

            string str;
            str = @"Dear User," +
            " \n\nThank you for using Thandora 2.0. " +
            " \nYour Post has been published in Advertisement." +
            " \nPlease check in Local Ads in menu." +
            " \n\nRegards," +
            " \nThandora Admin";


            cEnquiry.EnquiryMessage = str;
            cEnquiry.EnquiryID = 0;
            mc.PostEnquiry(cEnquiry);
            ac.SendSMSAdvPublish(senderPhone);



        }


        [Route("api/PostFeedBack")]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostFeedBack([FromBody]cFeedback feedback)
        {
            string retvalue, userName, deviceID;
            int cmntID;
            cStatus status = new cStatus();
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addFeedBack", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@FromUserID";
                    paramReceiverID.Value = feedback.userID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramRecUserType = new SqlParameter();
                    paramRecUserType.ParameterName = "@FromUserType";
                    paramRecUserType.Value = feedback.userType;
                    cmd.Parameters.Add(paramRecUserType);

                    SqlParameter paramcomments = new SqlParameter();
                    paramcomments.ParameterName = "@FeebBackMessage";
                    paramcomments.Value = feedback.Message;
                    cmd.Parameters.Add(paramcomments);

                    cmd.Parameters.Add("@result", SqlDbType.Int);
                    cmd.Parameters["@result"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@FromUsername", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@FromUsername"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@deviceid", SqlDbType.NVarChar, 300);
                    cmd.Parameters["@deviceid"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@result"].Value.ToString();
                    userName = cmd.Parameters["@FromUsername"].Value.ToString();
                    deviceID = cmd.Parameters["@deviceid"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Invalid User in Thandora ";
                        status.StatusMsg = " Invalid user. ";
                    }
                    if (status.returnID == -4)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "EXCEED ";
                        status.StatusMsg = " Feedback exceed ";
                    }
                    if (status.returnID == -3)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "badword";
                        status.StatusMsg = " Bad word in Feedback ";
                    }
                    if (status.returnID >0 )
                    {
                        status.StatusID = 0;
                        status.StatusMsg = " Updated successfully ";
         
                           string msg;

                            FCMController fcm = new FCMController();
                            SendNotificationController s = new SendNotificationController();
                            cStatus st = new cStatus();
                            st = await s.EnquiryNotificationAsync(deviceID, userName, feedback.Message, status.returnID, "Feedback", feedback.userID);
                            status.DesctoDev = st.DesctoDev;
                            msg = ":@deviceID=" + deviceID + ":@fromuser=" + userName + ":@message =" + feedback.Message + ":@feedback id=" + status.returnID;

                            status.StatusMsg = "Sent successfully";
                            fcm.PostFCMResponse(st.DesctoDev, msg, "Y");

                        /*if (st.returnID == 1)
                             {
                                    status.StatusMsg = "Comment Saved and Sent";
                                    fcm.PostFCMResponse(st.DesctoDev, msg, "Y");

                             }
                             if (st.StatusID == 1)
                             {
                                    status.StatusMsg = "FCM Notification failed";
                                    fcm.PostFCMResponse(st.DesctoDev, msg, "N");
                             }*/
                               /* statuse.returnID = 1 for success post. statuse.StatusID = 1 for failure post  */
                           

                           // status.returnID = cmntID; // COMMNETID required to handles the comments in UI locally as GP requested
                        
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



        [Route("api/blockComment")]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> blockComment(int userID, string userType, int CommentID)
        {
            int retvalue ;
          
            cStatus status = new cStatus();
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Msg_BlockComments", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@userID";
                    paramReceiverID.Value = userID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramRecUserType = new SqlParameter();
                    paramRecUserType.ParameterName = "@userType";
                    paramRecUserType.Value = userType;
                    cmd.Parameters.Add(paramRecUserType);

                    SqlParameter paramCommentID = new SqlParameter();
                    paramCommentID.ParameterName = "@Commentid";
                    paramCommentID.Value = CommentID;
                    cmd.Parameters.Add(paramCommentID);

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    retvalue =int.Parse(cmd.Parameters["@Result"].Value.ToString());
                   
                    status.returnID = retvalue;
                    if (status.returnID == 1)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Failed to Block Comment ";
                        status.StatusMsg = " Failed to Block Comment ";
                    }
                    if (status.returnID == 0)
                    {
                        status.StatusID = 0;
                        status.StatusMsg = " Blocked successfully ";

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



        // DELETE: api/Message/5
        /* [ResponseType(typeof(ctblMessage))]
         public IHttpActionResult DeletectblMessage(int id)
         {
             ctblMessage ctblMessage = db.ctblMessages.Find(id);
             if (ctblMessage == null)
             {
                 return NotFound();
             }

             db.ctblMessages.Remove(ctblMessage);
             db.SaveChanges();

             return Ok(ctblMessage);
         }

         protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 db.Dispose();
             }
             base.Dispose(disposing);
         }

         private bool ctblMessageExists(int id)
         {
             return db.ctblMessages.Count(e => e.MessageID == id) > 0;
         }*/
    }
}