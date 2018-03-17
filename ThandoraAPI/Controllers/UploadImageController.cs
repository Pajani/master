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
    public class UploadImageController : ApiController
    {
        public string imageFileURL { get; set; }

        [Route("api/PostUserImage")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<HttpResponseMessage>  PostUserProfileImage(int SenderID)
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();
            cStatus status = new cStatus();
            var logopath="";
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 1; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {

                           // var message = string.Format("Please Upload image of type .jpg,.gif,.png.");
                            status.StatusMsg = "Please Upload image of type .jpg,.gif,.png.";
                            status.StatusID = 1;


                            //dict.Add("error", message);
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, status);
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                           // var message = string.Format("Please Upload a file upto 1 mb.");
                            status.StatusMsg = "Image below 1 MB preferable.";
                            status.StatusID = 1;

                            //dict.Add("error", message);
                            //return Request.CreateResponse(HttpStatusCode.BadRequest, status);
                        }
                        else
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/Userimage/" + SenderID.ToString() + extension);

                            logopath = "http://joinwithme.in/thandoraAPI/userimage/" + SenderID.ToString() + extension;
                            postedFile.SaveAs (filePath);

                        }
                    }

                   // var message1 = string.Format("Image Updated Successfully.");
                    status.StatusMsg = "Image Updated Successfully.";
                    status.StatusID = 0;

                    string retvalue;
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            SqlCommand cmd = new SqlCommand("sp_update_sender_image", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter paramSenderID = new SqlParameter();
                            paramSenderID.ParameterName = "@SenderID";
                            paramSenderID.Value = SenderID;
                            cmd.Parameters.Add(paramSenderID);

                            SqlParameter paramlogopath = new SqlParameter();
                            paramlogopath.ParameterName = "@logopath";
                            paramlogopath.Value = logopath;
                            cmd.Parameters.Add(paramlogopath);

                            cmd.Parameters.Add("@LastupdatedID", SqlDbType.Int);
                            cmd.Parameters["@LastupdatedID"].Direction = ParameterDirection.Output;


                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            retvalue = cmd.Parameters["@LastupdatedID"].Value.ToString();
                            if (int.Parse(retvalue) > 0)
                            { 
                                status.StatusMsg = "Image Updated Successfully.";
                                status.StatusID = 0;
                                status.DesctoDev = logopath;
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        retvalue =  ex.Message.ToString();
                        status.StatusID = 1;
                        status.StatusMsg = "Image upload failed";

                    }
                    //return Request.CreateResponse(HttpStatusCode.Created, status.StatusMsg); 
                }
              
                return  Request.CreateResponse(HttpStatusCode.OK, status);
            }
            catch (Exception ex)
            {
                status.StatusID = 1;
                status.StatusMsg = ex.Message.ToString();
              
                return Request.CreateResponse(HttpStatusCode.NotFound, status);
            }
        }

        [Route("api/PostMessageImage")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> PostUserMessageImage(int MessageID)
        {
            cStatus status = new cStatus();
            Dictionary<string, object> dict = new Dictionary<string, object>();
            var imagepath = imageFileURL;
            try
            {

                var httpRequest = HttpContext.Current.Request;

                foreach (string file in httpRequest.Files)
                {
                    //HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.Created);

                    var postedFile = httpRequest.Files[file];
                    if (postedFile != null && postedFile.ContentLength > 0)
                    {

                        int MaxContentLength = 1024 * 1024 * 2; //Size = 1 MB  

                        IList<string> AllowedFileExtensions = new List<string> { ".jpg", ".gif", ".png" };
                        var ext = postedFile.FileName.Substring(postedFile.FileName.LastIndexOf('.'));
                        var extension = ext.ToLower();
                        if (!AllowedFileExtensions.Contains(extension))
                        {                    
                            status.StatusID = 1;
                            status.StatusMsg = "Please Upload image of type .jpg,.gif,.png.";
                          
                        }
                        else if (postedFile.ContentLength > MaxContentLength)
                        {

                            var message = string.Format("Please Upload a file upto 1 mb.");
                            status.StatusID = 1;
                            status.StatusMsg = "Upload image below 1 mb.";
                          
                        }
                        else
                        {
                            var filePath = HttpContext.Current.Server.MapPath("~/MessageImage/" + MessageID.ToString() + extension);
                            imagepath = "http://joinwithme.in/thandoraAPI/MessageImage/"+ MessageID.ToString() + extension;
                            postedFile.SaveAs(filePath);

                        }
                    }

                    var message1 = string.Format("Image Updated Successfully.");

                    string retvalue;
                    try
                    {
                        string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                        using (SqlConnection con = new SqlConnection(constr))
                        {
                            SqlCommand cmd = new SqlCommand("sp_update_message_image", con);
                            cmd.CommandType = CommandType.StoredProcedure;

                            SqlParameter paramMessageID = new SqlParameter();
                            paramMessageID.ParameterName = "@MessageID";
                            paramMessageID.Value = MessageID;
                            cmd.Parameters.Add(paramMessageID);

                            SqlParameter paramimagepath = new SqlParameter();
                            paramimagepath.ParameterName = "@imagePath";
                            paramimagepath.Value = imagepath;
                            cmd.Parameters.Add(paramimagepath);

                            cmd.Parameters.Add("@LastupdatedID", SqlDbType.Int);
                            cmd.Parameters["@LastupdatedID"].Direction = ParameterDirection.Output;


                            con.Open();
                            cmd.ExecuteNonQuery();
                            con.Close();
                            retvalue =cmd.Parameters["@LastupdatedID"].Value.ToString();
                            if (int.Parse(retvalue) > 0)
                            {
                                status.StatusMsg = "Image Updated Successfully.";
                                status.StatusID = 0;
                                status.DesctoDev = imagepath;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        retvalue = message1 + " " + ex.Message.ToString();
                        status.StatusID = 1;
                        status.StatusMsg = ex.Message.ToString();

                    }

                   // return Request.CreateResponse(HttpStatusCode.Created, status.StatusMsg); ;
                }
             
               //return Request.CreateResponse(HttpStatusCode.OK, status);
            }
            catch (Exception ex)
            {
                status.StatusID = 1;
                status.StatusMsg = ex.Message.ToString();
               
                //return Request.CreateResponse(HttpStatusCode.OK, status);

            }
            return status;
        }

    }
}
