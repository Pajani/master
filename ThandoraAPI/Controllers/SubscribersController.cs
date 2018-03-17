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
    public class SubscribersController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/Subscribers
        /* public IQueryable<ctblSubscriber> GetctblSubscribers()
         {
             return db.ctblSubscribers;
         }
         */

        // GET: api/Subscribers
        /* public List<cSubscriberReceiver> GetReceiverOf(int SenderId)
         {
             List<cSubscriberReceiver> SubRecList = new List<cSubscriberReceiver>();



             string retvalue;
             try
             {
                 string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                 using (SqlConnection con = new SqlConnection(constr))
                 {
                     SqlCommand cmd = new SqlCommand("sp_getReceiverOf", con);
                     cmd.CommandType = CommandType.StoredProcedure;
                     SqlDataReader reader;


                     SqlParameter paramReceiverID = new SqlParameter();
                     paramReceiverID.ParameterName = "@SenderID";
                     paramReceiverID.Value = SenderId;
                     cmd.Parameters.Add(paramReceiverID);

                     /////SqlParameter paramReceiverUserType = new SqlParameter();
                     paramReceiverUserType.ParameterName = "@SenderUserType";
                     paramReceiverUserType.Value = SenderUserType;
                     cmd.Parameters.Add(paramReceiverUserType);//////

                     con.Open();
                     reader= cmd.ExecuteReader( );


                     while(reader.Read())
                     {
                         cSubscriberReceiver c = new cSubscriberReceiver();
                         c.ReceiverID = (int) reader["ReceiverID"];
                         c.ReceiverPhoneno = reader["ReceiverPhone"].ToString().Trim();
                         c.ReceiverName = reader["ReceiverName"].ToString().Trim();
                         c.RecUserType= reader["userType"].ToString().Trim();
                         SubRecList.Add(c);

                     }

                     con.Close();

                 }
             }
             catch (Exception ex)
             {
                 retvalue = ex.Message.ToString();
             }

             //return Ok(retvalue);


             return SubRecList;
         }*/

        /*  [Route("subscribers/{customerId}/orders")]
          [HttpGet]
          public IEnumerable<Order> FindOrdersByCustomer(int customerId) { ... }
          */
        // GET: api/Subscribers/5
        /* [ResponseType(typeof(ctblSubscriber))]
         public IHttpActionResult GetctblSubscriber(int id)
         {
             ctblSubscriber ctblSubscriber = db.ctblSubscribers.Find(id);
             if (ctblSubscriber == null)
             {
                 return NotFound();
             }

             return Ok(ctblSubscriber);
         }

         // PUT: api/Subscribers/5
         [ResponseType(typeof(void))]
         public IHttpActionResult PutctblSubscriber(int id, ctblSubscriber ctblSubscriber)
         {
             if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             if (id != ctblSubscriber.SubscribeID)
             {
                 return BadRequest();
             }

             db.Entry(ctblSubscriber).State = EntityState.Modified;

             try
             {
                 db.SaveChanges();
             }
             catch (DbUpdateConcurrencyException)
             {
                 if (!ctblSubscriberExists(id))
                 {
                     return NotFound();
                 }
                 else
                 {
                     throw;
                 }
             }

             return StatusCode(HttpStatusCode.NoContent);
         }*/

        // POST: api/Subscribers
        [Route("api/Subscribersold")]
        [ResponseType(typeof(cStatus))]
        public IHttpActionResult PostctblSubscriber([FromBody]ctblSubscriber ctblSubscriber)
        {
            cStatus status = new cStatus();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addupdate_subscriber", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ctblSubscriber.ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@ReceiverUserType";
                    paramReceiverUserType.Value = ctblSubscriber.ReceiverUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    cmd.Parameters.Add(paramSenderID);


                    SqlParameter paramsubscribeonoff = new SqlParameter();
                    paramsubscribeonoff.ParameterName = "@subscribeonoff";
                    cmd.Parameters.Add(paramsubscribeonoff);

                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    for (int i = 0; i < ctblSubscriber.Senders.Count; i++)
                    {
                        paramSenderID.Value = ctblSubscriber.Senders[i].SenderID;
                        paramsubscribeonoff.Value = ctblSubscriber.Senders[i].subscribeonoff;
                        cmd.ExecuteNonQuery();

                    }
                    con.Close();

                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID > 0)
                    {
                        status.StatusID = 0;
                        status.StatusMsg = "Subcribed successfully";
                        status.DesctoDev = "Subcribed successfully, Need to call FCM subscribe method";


                    }
                    else if (status.returnID == -1)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Subscription failed";
                        status.DesctoDev = "Subscrited failed, Invalid user ";
                    }
                    else if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Subscription failed";
                        status.DesctoDev = "Subscrited failed, Invalid user ";
                    }

                    // cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
                status.DesctoDev = "Subscrited failed, Please check your internet";
            }

            return Ok(status);

            /* if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             db.ctblSubscribers.Add(ctblSubscriber);
             db.SaveChanges();

             return CreatedAtRoute("DefaultApi", new { id = ctblSubscriber.SubscribeID }, ctblSubscriber);*/
        }


        // POST: api/Subscribers
        //[Route("api/subcriberswithnotifiy")]
        [Route("api/Subscribers")]
        [HttpPost]
        [ResponseType(typeof(cStatus))]
        public async Task<cStatus> Postsubcriberswithnotifiy([FromBody]ctblSubscriber ctblSubscriber)
        {
            cStatus status = new cStatus();

            string retvalue, deviceID, receiverName;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addupdate_subscriber", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ctblSubscriber.ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramReceiverUserType = new SqlParameter();
                    paramReceiverUserType.ParameterName = "@ReceiverUserType";
                    paramReceiverUserType.Value = ctblSubscriber.ReceiverUserType;
                    cmd.Parameters.Add(paramReceiverUserType);

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    cmd.Parameters.Add(paramSenderID);


                    SqlParameter paramsubscribeonoff = new SqlParameter();
                    paramsubscribeonoff.ParameterName = "@subscribeonoff";
                    cmd.Parameters.Add(paramsubscribeonoff);

                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@deviceID", SqlDbType.NVarChar, 300);
                    cmd.Parameters["@deviceID"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@receivername", SqlDbType.NVarChar, 50);
                    cmd.Parameters["@receivername"].Direction = ParameterDirection.Output;

                    con.Open();
                    SendNotificationController s = new SendNotificationController();
                    cStatus st = new cStatus();
                    for (int i = 0; i < ctblSubscriber.Senders.Count; i++)
                    {
                        paramSenderID.Value = ctblSubscriber.Senders[i].SenderID;
                        paramsubscribeonoff.Value = ctblSubscriber.Senders[i].subscribeonoff;
                        cmd.ExecuteNonQuery();
                        deviceID = cmd.Parameters["@deviceID"].Value.ToString();
                        receiverName = cmd.Parameters["@receivername"].Value.ToString();
                       
                       /* if (ctblSubscriber.Senders[i].subscribeonoff.Contains("ON"))
                        {
                         st=await   s.SubscriberNotificationAsync(deviceID, receiverName, " following you @ Thandora");
                        }
                        else
                        {
                          st=await  s.SubscriberNotificationAsync(deviceID, receiverName, " leaving you @ Thandora");
                        }
                        status.DesctoDev = st.DesctoDev;
                        */
                        // status.returnID = int.Parse(retvalue);
                    }
                    con.Close();

                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID > 0)
                    {
                        status.StatusID = 0;
                        status.StatusMsg = "Updated successfully";
                        status.DesctoDev = "Updated successfully, Need to call FCM subscribe method";


                    }
                    else if (status.returnID == -1)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Updation failed";
                        status.DesctoDev = "Updation failed, Invalid user ";
                    }
                    else if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.StatusMsg = "Updation failed";
                        status.DesctoDev = "Updation failed, Invalid user ";
                    }

                    // cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
                status.StatusID = 1;
                status.StatusMsg = retvalue;
                status.DesctoDev = "Updation failed, Please check your internet";
            }

            return status;

            /* if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             db.ctblSubscribers.Add(ctblSubscriber);
             db.SaveChanges();

             return CreatedAtRoute("DefaultApi", new { id = ctblSubscriber.SubscribeID }, ctblSubscriber);*/
        }


        // DELETE: api/Subscribers/5
        /* [ResponseType(typeof(ctblSubscriber))]
         public IHttpActionResult DeletectblSubscriber(int id)
         {
             ctblSubscriber ctblSubscriber = db.ctblSubscribers.Find(id);
             if (ctblSubscriber == null)
             {
                 return NotFound();
             }

             db.ctblSubscribers.Remove(ctblSubscriber);
             db.SaveChanges();

             return Ok(ctblSubscriber);
         }

         protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 db.Dispose();
             }
             base.Dispose(disposing);
         }*/

        private bool ctblSubscriberExists(int id)
        {
            return db.ctblSubscribers.Count(e => e.SubscribeID == id) > 0;
        }
    }
}