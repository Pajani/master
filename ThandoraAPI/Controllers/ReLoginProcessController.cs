using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class ReLoginProcessController : ApiController
    {
     /*   [Route("api/ReLogin")]
        [AllowAnonymous]
        [ResponseType(typeof(cStatus))]
        public IHttpActionResult ReLoginProcess(String LoginPhoneNo)
        {
            cStatus status = new cStatus();

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {

                   
                        SqlCommand cmd = new SqlCommand("sp_check_phoneNo", con);
                        cmd.CommandType = CommandType.StoredProcedure;

                        SqlParameter paramdeviceID = new SqlParameter();
                        paramdeviceID.ParameterName = "@PhoneNo";
                        paramdeviceID.Value = LoginPhoneNo;
                        cmd.Parameters.Add(paramdeviceID);


                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;


                    cmd.Parameters.Add("@UserType", SqlDbType.NVarChar,1);
                    cmd.Parameters["@UserType"].Direction = ParameterDirection.Output;



                    con.Open();
                        cmd.ExecuteNonQuery();
                        con.Close();
                        retvalue = cmd.Parameters["@Result"].Value.ToString();
                    status.returnID = int.Parse(retvalue);
                    if (status.returnID == -1)
                    {
                        status.StatusID = 1;
                        status.DesctoDev = "Not Existing user. Show Sign Up screen. ";
                        status.StatusMsg = " Not existing user. Please sign up ";
                    }
                    if (status.returnID >= 100000)
                    {
                        status.StatusID = 0;
                        status.DesctoDev = "Existing user. Phone number verification Required.";
                       
                        status.userType = cmd.Parameters["@UserType"].Value.ToString();
                        if (status.userType.Equals("R"))
                            status.StatusMsg = " LISTENER user ";
                        if (status.userType.Equals("S"))
                            status.StatusMsg = " TELLER user ";

                    }
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return Ok(status);
        }*/
    }
}
