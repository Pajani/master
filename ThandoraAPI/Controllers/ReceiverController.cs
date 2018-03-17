using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class ReceiverController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/Receiver     /////////////////NOT USED
        /* public IQueryable<ctblReceiver> GetRs_tblReceiver()
         {
             return db.Rs_tblReceiver;
         }

         // GET: api/Receiver/5     /////////////////NOT USED
         [ResponseType(typeof(ctblReceiver))]
         public IHttpActionResult GetctblReceiver(int id)
         {
             ctblReceiver ctblReceiver = db.Rs_tblReceiver.Find(id);
             if (ctblReceiver == null)
             {
                 return NotFound();
             }

             return Ok(ctblReceiver);
         }*/
        //GET: api/GetReceiverOf(SenderID)
        [Route("api/GetReceiverOf")]
        [HttpGet]
        [ResponseType(typeof(List<cPartialReceiver>))]
        public async Task<List<cPartialReceiver>> GetReceiverOf(int SenderId, string SubscriptionONOFF)
        {
            List<cPartialReceiver> SubRecList = new List<cPartialReceiver>();
            /* cSubscriberReceiver c = new cSubscriberReceiver();
             c.ReceiverID = 1;
             c.ReceiverName = "Pajani";
             c.ReceiverPhoneno = "9654";
             c.RecUserType = "r";

             SubRecList.Add(c);
             */


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

                    SqlParameter paramsubscription = new SqlParameter();
                    paramsubscription.ParameterName = "@subscription";
                    paramsubscription.Value = SubscriptionONOFF;
                    cmd.Parameters.Add(paramsubscription);

                    con.Open();
                    reader = cmd.ExecuteReader();


                    while (reader.Read())
                    {
                        cPartialReceiver c = new cPartialReceiver();
                        c.ReceiverID = (int)reader["ReceiverID"];
                        c.ReceiverPhoneno = reader["ReceiverPhone"].ToString().Trim();
                        c.ReceiverName = reader["ReceiverName"].ToString().Trim();
                        c.RecUserType = reader["userType"].ToString().Trim();
                        c.logoPath = reader["logopath"].ToString().Trim();
                        SubRecList.Add(c);

                    }

                    con.Close();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }

            //return Ok(retvalue);*/


            return SubRecList;
        }

        // PUT: api/Receiver/5
        [ResponseType(typeof(cStatus))]
        public IHttpActionResult PutctblReceiver([FromBody] ctblReceiver ctblReceiver)
        {
            string retvalue;
            cStatus status = new cStatus();
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_update_receiver", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramReceiverID = new SqlParameter();
                    paramReceiverID.ParameterName = "@ReceiverID";
                    paramReceiverID.Value = ctblReceiver.ReceiverID;
                    cmd.Parameters.Add(paramReceiverID);

                    SqlParameter paramRecName = new SqlParameter();
                    paramRecName.ParameterName = "@ReceiverName";
                    paramRecName.Value = ctblReceiver.ReceiverName;
                    cmd.Parameters.Add(paramRecName);

                    SqlParameter paramRecPhone = new SqlParameter();
                    paramRecPhone.ParameterName = "@ReceiverPhone";
                    paramRecPhone.Value = ctblReceiver.ReceiverPhone;
                    cmd.Parameters.Add(paramRecPhone);

                    SqlParameter paramAddress = new SqlParameter();
                    paramAddress.ParameterName = "@Address";
                    paramAddress.Value = ctblReceiver.Address;
                    cmd.Parameters.Add(paramAddress);


                    SqlParameter paramPOSTCODE = new SqlParameter();
                    paramPOSTCODE.ParameterName = "@POSTCODE";
                    paramPOSTCODE.Value = ctblReceiver.POSTCODE;
                    cmd.Parameters.Add(paramPOSTCODE);

                    SqlParameter paramActiveUser = new SqlParameter();
                    paramActiveUser.ParameterName = "@ActiveUser";
                    paramActiveUser.Value = ctblReceiver.ActiveUser;
                    cmd.Parameters.Add(paramActiveUser);

                    cmd.Parameters.Add("@LastupdatedID", SqlDbType.Int);
                    cmd.Parameters["@LastupdatedID"].Direction = ParameterDirection.Output;


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@LastupdatedID"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID == -2)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "-2 is the error code for invalid POSTAL code. ";
                        status.StatusMsg = " Invalid POSTAL Code. ";
                    }
                    if (status.returnID >= 100000)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "Store the returnID in Users Mobile database with UserType = R";
                        status.StatusMsg = " Listener Successfully Created/Updated ";
                    }

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();


            }


            return Ok(status);

        }

        // POST: api/Receiver
        [ResponseType(typeof(cStatusStr))]
        public async Task<IHttpActionResult> PostctblReceiver([FromBody]ctblReceiver ctblReceiver)
        {
            cStatusStr status = new cStatusStr();
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_new_receiver", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramRecName = new SqlParameter();
                    paramRecName.ParameterName = "@ReceiverName";
                    paramRecName.Value = ctblReceiver.ReceiverName;
                    cmd.Parameters.Add(paramRecName);

                    SqlParameter paramRecPhone = new SqlParameter();
                    paramRecPhone.ParameterName = "@ReceiverPhone";
                    paramRecPhone.Value = ctblReceiver.ReceiverPhone;
                    cmd.Parameters.Add(paramRecPhone);

                    if(ctblReceiver.deviceTokenID == null)
                    {
                        ctblReceiver.deviceTokenID= "Assigned by API";
                    }

                    SqlParameter paramRecdeviceTokenID = new SqlParameter();
                    paramRecdeviceTokenID.ParameterName = "@IME";
                    paramRecdeviceTokenID.Value = ctblReceiver.deviceTokenID;
                    cmd.Parameters.Add(paramRecdeviceTokenID);

                    SqlParameter paramRecSIMNO = new SqlParameter();
                    paramRecSIMNO.ParameterName = "@SIMNO";
                    paramRecSIMNO.Value = ctblReceiver.SIMNO;
                    cmd.Parameters.Add(paramRecSIMNO);

                    SqlParameter paramPOSTCODE = new SqlParameter();
                    paramPOSTCODE.ParameterName = "@POSTCODE";
                    paramPOSTCODE.Value = ctblReceiver.POSTCODE;
                    cmd.Parameters.Add(paramPOSTCODE);

                    cmd.Parameters.Add("@LastInsertedID", SqlDbType.Int);
                    cmd.Parameters["@LastInsertedID"].Direction = ParameterDirection.Output;


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue = cmd.Parameters["@LastInsertedID"].Value.ToString();
                    status.returnID =retvalue;
                    //status.returnID = int.Parse(retvalue);

                    if (status.returnID.Equals("-5"))  //(status.returnID==-5)
                    { 
                        status.StatusID = "1";
                        status.DesctoDev = "Restricted LISTENER NAME ";
                        status.StatusMsg = " Restricted LISTENER NAME  ";
                    }

                    if (status.returnID.Equals("-2")) //(status.returnID==-2)
                    {
                        status.StatusID ="1";
                        status.DesctoDev = "-2 is the error code for invalid POSTAL code. ";
                        status.StatusMsg = " Invalid POSTAL Code. ";
                    }
                    if (int.Parse(status.returnID)>=100000)  /// (status.returnID >=100000)
                    {
                        status.StatusID = "0";
                        status.DesctoDev = "Store the returnID in Users Mobile database with UserType = R";
                        status.StatusMsg = " Listener Successfully Created/Updated ";
                        status.userType = "R";
                    }
                }
            }
            catch (Exception ex)
            {
                status.StatusMsg = ex.Message.ToString();
                status.StatusID = "1";
            }
            return Ok(status);

        }


        // POST: api/Receiver
        [Route("api/PostRestReceiver")]
        [ResponseType(typeof(cRestrictedReceiver))]
        public IHttpActionResult PostctblReceiver([FromBody]cRestrictedReceiver cRestrictedReceiver)
        {
            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_new_Restricted_receiver", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramRecName = new SqlParameter();
                    paramRecName.ParameterName = "@ReceiverName";
                    paramRecName.Value = cRestrictedReceiver.ReceiverName;
                    cmd.Parameters.Add(paramRecName);

                    SqlParameter paramRecPhone = new SqlParameter();
                    paramRecPhone.ParameterName = "@ReceiverPhone";
                    paramRecPhone.Value = cRestrictedReceiver.ReceiverPhoneno;
                    cmd.Parameters.Add(paramRecPhone);

                    SqlParameter paramSenderID = new SqlParameter();
                    paramSenderID.ParameterName = "@SenderID";
                    paramSenderID.Value = cRestrictedReceiver.SenderID;
                    cmd.Parameters.Add(paramSenderID);

                    SqlParameter paramRecGroup = new SqlParameter();
                    paramRecGroup.ParameterName = "@ReceiverGroup";
                    paramRecGroup.Value = cRestrictedReceiver.RecGroup;
                    cmd.Parameters.Add(paramRecGroup);

                    SqlParameter paramRecActive = new SqlParameter();
                    paramRecActive.ParameterName = "@ReceiverActive";
                    paramRecActive.Value = cRestrictedReceiver.RecActive;
                    cmd.Parameters.Add(paramRecActive);

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


        // DELETE: api/Receiver/5
        /* [ResponseType(typeof(ctblReceiver))]
         public IHttpActionResult DeletectblReceiver(int id)
         {
             ctblReceiver ctblReceiver = db.Rs_tblReceiver.Find(id);
             if (ctblReceiver == null)
             {
                 return NotFound();
             }

             db.Rs_tblReceiver.Remove(ctblReceiver);
             db.SaveChanges();

             return Ok(ctblReceiver);
         }

         protected override void Dispose(bool disposing)
         {
             if (disposing)
             {
                 db.Dispose();
             }
             base.Dispose(disposing);
         }*/

        private bool ctblReceiverExists(int id)
        {
            return db.Rs_tblReceiver.Count(e => e.ReceiverID == id) > 0;
        }
    }
}