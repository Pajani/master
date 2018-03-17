using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Web.Http;

using System.Threading.Tasks;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using RestSharp;
using System.Web;

using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class GetAllAreaController : ApiController
    {

        [Route("api/GetDetailedAreaforLatLang")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> GetDetailedAreaforLatLang()
        {
            string uri;
            string latlong, loclatlong;
            int id;

            // uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=12.8069792,80.197437&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";

            // uri = "http://maps.googleapis.com/maps/api/distancematrix/json?origins=" + origin + ",india&destinations=" + destination + ",india&mode=driving&language=en-EN&sensor=false";
            try
            {

                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    /* SqlCommand cmd = new SqlCommand("sp_addLatLong", con);
                     cmd.CommandType = CommandType.StoredProcedure;



                     cmd.Parameters.Add("@id", SqlDbType.Int );
                     cmd.Parameters["@id"].Direction = ParameterDirection.Output;


                     cmd.Parameters.Add("@LATLANG", SqlDbType.VarChar, 30);
                     cmd.Parameters["@LATLANG"].Direction = ParameterDirection.Output;

                     con.Open();
                     cmd.ExecuteNonQuery();
                     con.Close();
                     latlong = cmd.Parameters["@LATLANG"].Value.ToString();
                     id =  (int)cmd.Parameters["@id"].Value;
 */
                    uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=12.8069792,80.197437&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";

                    // uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng="+latlong+"&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";



                    HttpClient client = new HttpClient();
                    var getResult = client.GetAsync(uri);
                    getResult.Wait();
                    var result = getResult.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        RootObject cnt = new RootObject();
                        cnt = await result.Content.ReadAsAsync<RootObject>();
                        if (cnt.results.Count > 0)
                            loclatlong = cnt.results[0].geometry.location.lat.ToString() + "," + cnt.results[0].geometry.location.lng.ToString();
                    }


                    /*
                                        var client = new RestClient(uri);
                                        var request = new RestRequest(Method.GET);
                                        IRestResponse<RootObject> asyncHandle = await client.ExecutePostTaskAsync<RootObject>(request);
                                        result = asyncHandle.Content;


                                        JavaScriptSerializer jss = new JavaScriptSerializer();
                                        RootObject test = jss.Deserialize<RootObject>(result);


                                        SqlCommand cmdUpdate = new SqlCommand("sp_addlatLongapivalue", con);
                                        cmdUpdate.CommandType = CommandType.StoredProcedure;

                                        SqlParameter paramID = new SqlParameter();
                                        paramID.ParameterName = "@ID";
                                        paramID.Value = id;
                                        cmdUpdate.Parameters.Add(paramID);


                                        SqlParameter paramresult = new SqlParameter();
                                        paramresult.ParameterName = "@result";
                                        paramresult.Value = result;
                                        cmdUpdate.Parameters.Add(paramresult);

                                        con.Open();
                                        cmdUpdate.ExecuteNonQuery();
                                        con.Close();

                                        */

                }
            }
            catch (Exception ex)
            {
                // distance.status = "failed";
                return ex.Message;

            }
            return "success";


        }



        [Route("api/GetLatLongforAddress")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> GetLatLongforAddress()
        {
            string uri;
            string address, loclatlong = "-1,-1";
            int SenderID;

            try
            {

                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addLatLong_unreg_user", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@SenderID", SqlDbType.Int);
                    cmd.Parameters["@SenderID"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@address", SqlDbType.VarChar, 300);
                    cmd.Parameters["@address"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    address = cmd.Parameters["@address"].Value.ToString();
                    SenderID = (int)cmd.Parameters["@SenderID"].Value;

                    // uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=12.8069792,80.197437&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";

                    // uri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode("Kelambakkam Village, Gdt Road, Vandalur, Chennai 600048") + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";
                    uri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(address.Replace("-", "")) + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";


                    HttpClient client = new HttpClient();
                    var getResult = client.GetAsync(uri);
                    getResult.Wait();
                    var result = getResult.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        RootObject cnt = new RootObject();
                        cnt = await result.Content.ReadAsAsync<RootObject>();
                        if (cnt.results.Count > 0)
                            loclatlong = cnt.results[0].geometry.location.lat.ToString() + "," + cnt.results[0].geometry.location.lng.ToString();
                    }

                    SqlCommand cmdUpdate = new SqlCommand("sp_addlatLongapivalue_unreg_user", con);
                    cmdUpdate.CommandType = CommandType.StoredProcedure;

                    SqlParameter paramID = new SqlParameter();
                    paramID.ParameterName = "@SenderID";
                    paramID.Value = SenderID;
                    cmdUpdate.Parameters.Add(paramID);


                    SqlParameter paramresult = new SqlParameter();
                    paramresult.ParameterName = "@loclatlong";
                    paramresult.Value = loclatlong;
                    cmdUpdate.Parameters.Add(paramresult);

                    con.Open();
                    cmdUpdate.ExecuteNonQuery();
                    con.Close();

                }
            }
            catch (Exception ex)
            {
                // distance.status = "failed";
                return ex.Message;

            }
            return "success";

        }



        [Route("api/GetAddressforLatLong")]
        [AllowAnonymous]
        [HttpGet]
        [ResponseType(typeof(string))]
        public async Task<string> GetAddressforLatLong()
        {
            string uri;
            string loclatlng, address = "";
            int id;

            try
            {

                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addAddress_to_svcProviders", con);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@id", SqlDbType.Int);
                    cmd.Parameters["@id"].Direction = ParameterDirection.Output;

                    cmd.Parameters.Add("@loclatlng", SqlDbType.VarChar, 50);
                    cmd.Parameters["@loclatlng"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    loclatlng = cmd.Parameters["@loclatlng"].Value.ToString();
                    id = (int)cmd.Parameters["@id"].Value;

                    if (id > 0)
                    {
                        // uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=12.8069792,80.197437&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";

                        // uri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode("Kelambakkam Village, Gdt Road, Vandalur, Chennai 600048") + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";
                        uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + loclatlng + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";


                        HttpClient client = new HttpClient();
                        var getResult = client.GetAsync(uri);
                        getResult.Wait();
                        var result = getResult.Result;
                        if (result.IsSuccessStatusCode)
                        {
                            RootObject cnt = new RootObject();
                            cnt = await result.Content.ReadAsAsync<RootObject>();
                            if (cnt.results.Count > 0)
                                address = cnt.results[0].formatted_address;
                        }

                        SqlCommand cmdUpdate = new SqlCommand("sp_addAddress_to_svcvProviders", con);
                        cmdUpdate.CommandType = CommandType.StoredProcedure;

                        SqlParameter paramID = new SqlParameter();
                        paramID.ParameterName = "@id";
                        paramID.Value = id;
                        cmdUpdate.Parameters.Add(paramID);


                        SqlParameter paramresult = new SqlParameter();
                        paramresult.ParameterName = "@address";
                        paramresult.Value = address;
                        cmdUpdate.Parameters.Add(paramresult);

                        con.Open();
                        cmdUpdate.ExecuteNonQuery();
                        con.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                // distance.status = "failed";
                return ex.Message;

            }
            return "success";

        }




        [Route("api/GetServicesKeyWords")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cSvcKeyWords>))]
        public async Task<List<cSvcKeyWords>> GetServiceKeyWords()
        {

            string retvalue, wd;
            List<cSvcKeyWords> cDictWords = new List<cSvcKeyWords>();
            cSvcKeyWords word;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_get_Svc_Keyword", con);
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
                        word = new cSvcKeyWords();
                        word.words = new List<string>();
                        while (reader.Read())
                        {
                            wd = reader["keyword"].ToString();
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



        [Route("api/GetPlacesKeyWords")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cSvcKeyWords>))]
        public async Task<List<cSvcKeyWords>> GetPlacesKeyWords()
        {

            string retvalue, wd;
            List<cSvcKeyWords> cDictWords = new List<cSvcKeyWords>();
            cSvcKeyWords word;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_get_Places_Keywords", con);
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
                        word = new cSvcKeyWords();
                        word.words = new List<string>();
                        while (reader.Read())
                        {
                            wd = reader["keyword"].ToString();
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

        // POST: api/Subscribers

        [HttpPost]
        [AllowAnonymous]
        [Route("api/AddYourServiceProviders")]
        [ResponseType(typeof(string))]
        public async Task<string> AddYourServiceProviders(cAddYourSvcProvider AddYourSvcProvider)
        {


            string retvalue;
            retvalue = "";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_addSvcProviders_byUsers", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramuserId = new SqlParameter();
                    paramuserId.ParameterName = "@userId";
                    paramuserId.Value = AddYourSvcProvider.userId;
                    cmd.Parameters.Add(paramuserId);


                    SqlParameter paramSvcProviderNumber = new SqlParameter();
                    paramSvcProviderNumber.ParameterName = "@SvcProviderNumber";
                    cmd.Parameters.Add(paramSvcProviderNumber);


                    SqlParameter paramSvcProviderName = new SqlParameter();
                    paramSvcProviderName.ParameterName = "@SvcProviderName";
                    cmd.Parameters.Add(paramSvcProviderName);

                    SqlParameter paramSvcProviderService = new SqlParameter();
                    paramSvcProviderService.ParameterName = "@SvcProviderService";
                    cmd.Parameters.Add(paramSvcProviderService);

                    SqlParameter paramLocation = new SqlParameter();
                    paramLocation.ParameterName = "@Location";
                    cmd.Parameters.Add(paramLocation);

                    SqlParameter paramAddress = new SqlParameter();
                    paramAddress.ParameterName = "@Address";
                    cmd.Parameters.Add(paramAddress);


                    cmd.Parameters.Add("@MessagetoUsers", SqlDbType.NVarChar, 100);
                    cmd.Parameters["@MessagetoUsers"].Direction = ParameterDirection.Output;

                    con.Open();
                    for (int i = 0; i < AddYourSvcProvider.serviceProvider.Count; i++)
                    {
                        paramSvcProviderNumber.Value = AddYourSvcProvider.serviceProvider[i].ServiceProviderNumber;
                        paramSvcProviderName.Value = AddYourSvcProvider.serviceProvider[i].ServiceProviderName;
                        paramSvcProviderService.Value = AddYourSvcProvider.serviceProvider[i].Service;
                       
                        if (AddYourSvcProvider.serviceProvider[i].Location.Length <2)
                        { paramLocation.Value = "0.0,0.0"; }
                        else
                        { paramLocation.Value = AddYourSvcProvider.serviceProvider[i].Location; }

                        paramAddress.Value = AddYourSvcProvider.serviceProvider[i].Address;


                        cmd.ExecuteNonQuery();
                        if (cmd.Parameters["@MessagetoUsers"].Value.ToString().Length != 0)
                            retvalue = retvalue + cmd.Parameters["@MessagetoUsers"].Value.ToString() + ", ";

                    }
                    con.Close();
                    if (retvalue.Length == 0)
                    {
                        retvalue = "Thank You for Helping Your Friends";
                    }
                    else
                    {
                        retvalue = retvalue.Substring(0, retvalue.Length - 2) + " already available in Thandora.\n Thank You for Helping Your Friends";
                    }

                    // cmd.Dispose();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();

            }

            return retvalue;

            /* if (!ModelState.IsValid)
             {
                 return BadRequest(ModelState);
             }

             db.ctblSubscribers.Add(ctblSubscriber);
             db.SaveChanges();

             return CreatedAtRoute("DefaultApi", new { id = ctblSubscriber.SubscribeID }, ctblSubscriber);*/
        }

    }
}
