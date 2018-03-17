using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Configuration;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;
using System.Data;
using System.Data.SqlClient;

namespace ThandoraAPI.Controllers
{
    public class myAdminController : ApiController
    {
        [Route("api/GetUserCounts")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(string))]
        public async Task<String> getUserCounts()
        {

            string retvalue;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_v2_0_users_COUNTS", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    /*SqlParameter paramPhoneNo = new SqlParameter();
                    paramPhoneNo.ParameterName = "@PhoneNo";
                    paramPhoneNo.Value = phoneno;
                    cmd.Parameters.Add(paramPhoneNo);*/

                    cmd.Parameters.Add("@vercount", SqlDbType.Int);
                    cmd.Parameters["@vercount"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@totusercount", SqlDbType.Int);
                    cmd.Parameters["@totusercount"].Direction = ParameterDirection.Output;


                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    retvalue ="v2:" +  cmd.Parameters["@vercount"].Value.ToString() + " Tot:" + cmd.Parameters["@totusercount"].Value.ToString();

                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return (retvalue);

        }

        [Route("api/GetThandoraDictionary")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cThnDictionary>))]
        public async Task<List<cThnDictionary>> GetThandoraDictionary()
        {

            string retvalue, wd;
            List<cThnDictionary> cDictWords = new List<cThnDictionary>();
            cThnDictionary word;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_get_Thandora_Dictionary", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    /*SqlParameter paramPhoneNo = new SqlParameter();
                    paramPhoneNo.ParameterName = "@PhoneNo";
                    paramPhoneNo.Value = phoneno;
                    cmd.Parameters.Add(paramPhoneNo);*/


                    con.Open();
                    reader = cmd.ExecuteReader();
                    do
                    {
                         word = new cThnDictionary();
                        word.words = new List<string>();
                        while (reader.Read())
                        {
                            wd = reader["word"].ToString();
                            word.Indexchar = wd.Substring(0, 1);
                            word.words.Add(wd);
                        }
                        cDictWords.Add(word);
                    } while (reader.NextResult());

                    con.Close();
                   
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            return (cDictWords);

        }

    }
}
