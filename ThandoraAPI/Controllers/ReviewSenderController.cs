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
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class ReviewSenderController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/ReviewSender
        /*public IQueryable<cReviewSender> GetcReviewSenders()
        {
            return db.cReviewSenders;
        }**/

        // GET: api/ReviewSender/5
        [Route("api/GetReviewedMessageforSender")]
        [ResponseType(typeof(cReviewMessages))]
        public List<cReviewMessages> GetcReviewSender(int SenderID)
        {
            List<cReviewMessages> listReviewedMessage = new List<cReviewMessages>();



            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_GetReviewed_messages", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;


                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    paramSenderID.Value = SenderID;
                    cmd.Parameters.Add(paramSenderID);

                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        cReviewMessages c = new cReviewMessages();
                        c.SenderID = (int)reader["SenderID"];
                        c.SenderName = reader["SenderName"].ToString().Trim();

                        c.reviewerID = (int)reader["ReviewerID"]; ;
                        c.ReviewerName = reader["ReviewerName"].ToString().Trim();

                        c.ReviewComments = reader["ReviewComments"].ToString().Trim();
                        c.RevUserType= reader["RevUserType"].ToString().Trim();
                        c.ReviewDate = (DateTime)reader["ReviewDate"];


                        //c.logopath= reader["logopath"].ToString().Trim();

                        listReviewedMessage.Add(c);

                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return listReviewedMessage;

        }

        // PUT: api/ReviewSender/5
        /*[ResponseType(typeof(void))]
        public IHttpActionResult PutcReviewSender(int id, cReviewSender cReviewSender)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != cReviewSender.reviewerID)
            {
                return BadRequest();
            }

            db.Entry(cReviewSender).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!cReviewSenderExists(id))
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

        // POST: api/ReviewSender
        [Route("api/PostReviewforSender")]
        [ResponseType(typeof(cReviewSender))]
        public IHttpActionResult PostcReviewSender([FromBody]cReviewSender cReviewSender)
        {
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_Reviewfor_sender", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramreviewerID = new SqlParameter();
                    paramreviewerID.ParameterName = "@ReviewerID";
                    paramreviewerID.Value = cReviewSender.reviewerID;
                    cmd.Parameters.Add(paramreviewerID);

                    SqlParameter paramRevUserType = new SqlParameter();
                    paramRevUserType.ParameterName = "@RevUserType";
                    paramRevUserType.Value = cReviewSender.RevUserType;
                    cmd.Parameters.Add(paramRevUserType);

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    paramSenderID.Value = cReviewSender.SenderID;
                    cmd.Parameters.Add(paramSenderID);

                    SqlParameter paramReviewComments = new SqlParameter();
                    paramReviewComments.ParameterName = "@ReviewComments";
                    paramReviewComments.Value = cReviewSender.ReviewComments;
                    cmd.Parameters.Add(paramReviewComments);

                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return Ok(retvalue);

        }

        // DELETE: api/ReviewSender/5
       /* [ResponseType(typeof(cReviewSender))]
        public IHttpActionResult DeletecReviewSender(int id)
        {
            cReviewSender cReviewSender = db.cReviewSenders.Find(id);
            if (cReviewSender == null)
            {
                return NotFound();
            }

            db.cReviewSenders.Remove(cReviewSender);
            db.SaveChanges();

            return Ok(cReviewSender);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }*/

        private bool cReviewSenderExists(int id)
        {
            return db.cReviewSenders.Count(e => e.reviewerID == id) > 0;
        }
    }
}