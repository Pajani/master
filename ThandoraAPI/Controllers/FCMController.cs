using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class FCMController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/FCM
       /* public IQueryable<ctblFCM> GetctblFCMs()
        {
            return db.ctblFCMs;
        }*/

        // GET: api/FCM/5
        [Route("api/GetFCMDetails")]
        [ResponseType(typeof(ctblFCM))]
        public ctblFCM GetctblFCM(int id)
        {
            ctblFCM ctblFCM = db.ctblFCMs.Find(id);
            if (ctblFCM == null)
            {
                ctblFCM ctblFC = new ctblFCM();
                ctblFC.SERVER_API_KEY = " No FCM DETAILS FOUND. ";
                return ctblFC;
            }

            return ctblFCM;
        }

        // PUT: api/FCM/5
        /*   [ResponseType(typeof(void))]
           public IHttpActionResult PutctblFCM(int id, ctblFCM ctblFCM)
           {
               if (!ModelState.IsValid)
               {
                   return BadRequest(ModelState);
               }

               if (id != ctblFCM.KEY_ID)
               {
                   return BadRequest();
               }

               db.Entry(ctblFCM).State = EntityState.Modified;

               try
               {
                   db.SaveChanges();
               }
               catch (DbUpdateConcurrencyException)
               {
                   if (!ctblFCMExists(id))
                   {
                       return NotFound();
                   }
                   else
                   {
                       throw;
                   }
               }

               return StatusCode(HttpStatusCode.NoContent);
           }

           // POST: api/FCM
           [ResponseType(typeof(ctblFCM))]
           public IHttpActionResult PostctblFCM(ctblFCM ctblFCM)
           {
               if (!ModelState.IsValid)
               {
                   return BadRequest(ModelState);
               }

               db.ctblFCMs.Add(ctblFCM);
               db.SaveChanges();

               return CreatedAtRoute("DefaultApi", new { id = ctblFCM.KEY_ID }, ctblFCM);
           }

           // DELETE: api/FCM/5
           [ResponseType(typeof(ctblFCM))]
           public IHttpActionResult DeletectblFCM(int id)
           {
               ctblFCM ctblFCM = db.ctblFCMs.Find(id);
               if (ctblFCM == null)
               {
                   return NotFound();
               }

               db.ctblFCMs.Remove(ctblFCM);
               db.SaveChanges();

               return Ok(ctblFCM);
           }

           protected override void Dispose(bool disposing)
           {
               if (disposing)
               {
                   db.Dispose();
               }
               base.Dispose(disposing);
           }

           private bool ctblFCMExists(int id)
           {
               return db.ctblFCMs.Count(e => e.KEY_ID == id) > 0;
           }*/

        // POST: api/Receiver
        [Route("api/PostFCMResponse")]
        public async Task PostFCMResponse(string fcmresponse,string message,string successFlag)
        {
            cStatusStr status = new cStatusStr();
          
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_fcmresponse", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramfcmresponsee = new SqlParameter();
                    paramfcmresponsee.ParameterName = "@fcmResponse";
                    paramfcmresponsee.Value = fcmresponse;
                    cmd.Parameters.Add(paramfcmresponsee);

                    SqlParameter parammessage = new SqlParameter();
                    parammessage.ParameterName = "@message";
                    parammessage.Value = message;
                    cmd.Parameters.Add(parammessage);

                    SqlParameter paramsuccessflag = new SqlParameter();
                    paramsuccessflag.ParameterName = "@successflag";
                    paramsuccessflag.Value = successFlag;
                    cmd.Parameters.Add(paramsuccessflag);
                 
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                
                    //status.returnID = int.Parse(retvalue);
                   
                }
            }
            catch (Exception ex)
            {
                status.StatusMsg = ex.Message.ToString();
                status.StatusID = "1";
            }

        }


        // POST: api/Receiver
        [Route("api/getVersion")]
        [HttpGet]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> getVersion()
        {
            cStatus status = new cStatus();


            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_get_version", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@version", SqlDbType.NVarChar, 5);
                    cmd.Parameters["@version"].Direction = ParameterDirection.Output;


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();


                    status.StatusMsg = cmd.Parameters["@version"].Value.ToString();
                    status.StatusID = 0;


                    //status.returnID = int.Parse(retvalue);

                }
            }
            catch (Exception ex)
            {
                status.StatusMsg = ex.Message.ToString();
                status.StatusID = 1;
            }
            return status;

        }


    }
}