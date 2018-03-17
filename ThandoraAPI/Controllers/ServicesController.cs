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
using System.Web;

using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class ServicesController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/Services
        [Route("api/Services")]
        [HttpGet]
        [AllowAnonymous]
        public IQueryable<ctblServices> GetRs_tblServices(String search)
        {
           // if (search.Contains("all") || )
           // {
                return db.Rs_tblServices.OrderBy(x => x.cDisplayOrder);
           // }
            /*else
            {
                return db.Rs_tblServices.Where(element => element.cSvcDesc.Contains(search)).Select(x => x);

            }*/

        }


        //GET: api/GetReceiverOf(SenderID)
        [Route("api/Services")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cServiceProvider>))]
        public async Task< List<cServiceProvider>> GetServiceProviders_v1(int ReceiverID, string RecUserType)
        {
            List<cServiceProvider> ServiceProviders = new List<cServiceProvider>();

            string retvalue="1";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_listServiceProvider_v1", con);
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

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();


                    //reader.Read();

                    while (reader.Read())
                    {
                        cServiceProvider sp = new cServiceProvider();
                        sp.providers = new List<cPartialSender>();
                        sp.cDisplayOrder = (double)reader["cDisplayOrder"];
                        sp.cServiceCategory = reader["cServiceCategory"].ToString().Trim();
                        sp.cSvcDesc = reader["cSvcDesc"].ToString().Trim();
                        sp.cServicetypeid = (int)reader["ID"];
                        sp.providers = new List<cPartialSender>();
                        sp.cCount = 0;
                        ServiceProviders.Add(sp);
                    }
                    reader.NextResult();
                    List<cPartialSender> partialSenders;
                    reader.Read();
                    while (1==1)
                    {
                        partialSenders = new List<cPartialSender>();
                        string cServiceType;
                        do
                        {
                            cPartialSender c = new cPartialSender();
                            //sp.cCount = sp.cCount + 1;
                            c.SenderID = (int)reader["SenderID"];
                            c.SenderName = reader["SenderName"].ToString().Trim();
                            c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                            c.ContactHide = (Int16)reader["ContactHide"];
                            c.cServiceType = reader["cServiceCategory"].ToString().Trim();
                            cServiceType = c.cServiceType;
                            c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();

                            if (reader["logopath"] != null)
                                if (reader["logopath"].ToString().Length > 0)
                                    c.logopath = reader["logopath"].ToString().Trim();
                            c.address = reader["address"].ToString().Trim();
                            c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                            c.ReviewReceived = (int)reader["ReviewReceived"];
                            c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                            c.msgLikeReceived = (int)reader["msgLikeReceived"];
                            c.msgTotpublished = (int)reader["msgTotpublished"];
                            c.msgReadBy = (int)reader["msgReadBy"];
                            partialSenders.Add(c);
                        } while (reader.Read() && cServiceType.Equals(reader["cServiceCategory"].ToString().Trim()));

                        for (int i =0;i<ServiceProviders.Count;i++)
                        {
                            if (ServiceProviders[i].cServiceCategory.Equals(partialSenders[0].cServiceType.ToString()))
                            {
                                ServiceProviders[i].providers = partialSenders;
                                ServiceProviders[i].cCount = partialSenders.Count;
                                break;
                            }
                        }

                    }
                    /*do
                    {
                       if (reader["SenderID"] != null)
                        {
                            if (reader["SenderID"].ToString().Length > 0)
                            {
                                cPartialSender c = new cPartialSender();
                                //sp.cCount = sp.cCount + 1;
                                c.SenderID = (int)reader["SenderID"];
                                c.SenderName = reader["SenderName"].ToString().Trim();
                                c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                                c.ContactHide = (Int16)reader["ContactHide"];
                                c.cServiceType = reader["cServiceCategory"].ToString().Trim();
                                c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();

                                if (reader["logopath"] != null)
                                    if (reader["logopath"].ToString().Length > 0)
                                        c.logopath = reader["logopath"].ToString().Trim();

                                c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                                c.ReviewReceived = (int)reader["ReviewReceived"];
                                c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                                c.msgLikeReceived = (int)reader["msgLikeReceived"];
                                c.msgTotpublished = (int)reader["msgTotpublished"];
                                c.msgReadBy = (int)reader["msgReadBy"];
                                //sp.providers.Add(c);
                            }
                        }



                    } while (reader.Read() && sp.cServiceCategory.Equals(reader["cServiceCategory"].ToString().Trim())) ;

                    */

                        /* while (retvalue.Equals("1"))
                         {
                             cServiceProvider sp = new cServiceProvider();
                             sp.providers = new List<cPartialSender>();
                             sp.cDisplayOrder= (double)reader["cDisplayOrder"];
                             sp.cServiceCategory = reader["cServiceCategory"].ToString().Trim();
                             sp.cSvcDesc  = reader["cSvcDesc"].ToString().Trim();
                             sp.cServicetypeid = (int)reader["ID"];
                             sp.cCount = 0;
                             do
                             {
                                 if (reader["SenderID"] != null)
                                 {
                                     if (reader["SenderID"].ToString().Length > 0)
                                     {
                                         cPartialSender c = new cPartialSender();
                                         sp.cCount = sp.cCount + 1;
                                         c.SenderID = (int)reader["SenderID"];
                                         c.SenderName = reader["SenderName"].ToString().Trim();
                                         c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                                         c.ContactHide = (Int16)reader["ContactHide"];
                                         c.cServiceType = reader["cServiceCategory"].ToString().Trim();
                                         c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();

                                         if (reader["logopath"] != null)
                                             if (reader["logopath"].ToString().Length > 0)
                                                 c.logopath = reader["logopath"].ToString().Trim();

                                         c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                                         c.ReviewReceived = (int)reader["ReviewReceived"];
                                         c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                                         c.msgLikeReceived = (int)reader["msgLikeReceived"];
                                         c.msgTotpublished = (int)reader["msgTotpublished"];
                                         c.msgReadBy = (int)reader["msgReadBy"];
                                         sp.providers.Add(c);
                                     }
                                 }



                             } while (reader.Read() && sp.cServiceCategory.Equals(reader["cServiceCategory"].ToString().Trim()));

                             ServiceProviders.Add(sp);




                         }*/
                        con.Close();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            //return Ok(retvalue);*/
            cServiceProvider tempSP = new cServiceProvider();
            for (int i = 0; i < ServiceProviders.Count; i++)
            {
                for (int j = 0; j < ServiceProviders.Count - i-1; j++)
                {

                    if (ServiceProviders[j].cCount< ServiceProviders[j+1].cCount)
                    {
                        tempSP = ServiceProviders[j + 1];
                        ServiceProviders[j+1] = ServiceProviders[j];
                        ServiceProviders[j] = tempSP;
                       
                    }
                }
            }
            return ServiceProviders;
        }


        //GET: api/GetReceiverOf(SenderID)
        [Route("api/ServicesProd")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cServiceProvider>))]
        public async Task<List<cServiceProvider>> GetServiceProviders(int ReceiverID, string RecUserType)
        {
            List<cServiceProvider> ServiceProviders = new List<cServiceProvider>();

            string retvalue = "1";
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_listServiceProvider", con);
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

                    cmd.Parameters.Add("@Result", SqlDbType.Int);
                    cmd.Parameters["@Result"].Direction = ParameterDirection.Output;

                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();


                    reader.Read();

                    while (retvalue.Equals("1"))
                    {
                        cServiceProvider sp = new cServiceProvider();
                        sp.providers = new List<cPartialSender>();
                        sp.cDisplayOrder = (double)reader["cDisplayOrder"];
                        sp.cServiceCategory = reader["cServiceCategory"].ToString().Trim();
                        sp.cSvcDesc = reader["cSvcDesc"].ToString().Trim();
                        sp.cServicetypeid = (int)reader["ID"];
                        sp.cCount = 0;
                        do
                        {
                            if (reader["SenderID"] != null)
                            {
                                if (reader["SenderID"].ToString().Length > 0)
                                {
                                    cPartialSender c = new cPartialSender();
                                    sp.cCount = sp.cCount + 1;
                                    c.SenderID = (int)reader["SenderID"];
                                    c.SenderName = reader["SenderName"].ToString().Trim();
                                    c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                                    c.ContactHide = (Int16)reader["ContactHide"];
                                    c.cServiceType = reader["cServiceCategory"].ToString().Trim();
                                    c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();

                                    if (reader["logopath"] != null)
                                        if (reader["logopath"].ToString().Length > 0)
                                            c.logopath = reader["logopath"].ToString().Trim();

                                    c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                                    c.ReviewReceived = (int)reader["ReviewReceived"];
                                    c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                                    c.msgLikeReceived = (int)reader["msgLikeReceived"];
                                    c.msgTotpublished = (int)reader["msgTotpublished"];
                                    c.msgReadBy = (int)reader["msgReadBy"];
                                    sp.providers.Add(c);
                                }
                            }



                        } while (reader.Read() && sp.cServiceCategory.Equals(reader["cServiceCategory"].ToString().Trim()));

                        ServiceProviders.Add(sp);

                        /* if ((int)reader["SenderID"] > 100000)
                         { retvalue = "1"; }
                         else
                         { retvalue = "-1";}*/


                    }
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            //return Ok(retvalue);*/
            return ServiceProviders;
        }


        [Route("api/ServicesbySearchlive")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cServiceProvider>))]
        public async Task<List<cServiceProvider>> GetServiceProvidersforSearch(int userid, string userType, string SEARCH)
        {
            List<cServiceProvider> ServiceProviders = new List<cServiceProvider>();
            //List<cPartialSender> SendersList = new List<cPartialSender>();

            string retvalue = "1",userPostcode="";
            
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_listServiceProviderforSearch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlParameter paramsearch = new SqlParameter();
                    paramsearch.ParameterName = "@search";
                    paramsearch.Value = SEARCH;
                    cmd.Parameters.Add(paramsearch);

                    SqlParameter paramuserid = new SqlParameter();
                    paramuserid.ParameterName = "@userID";
                    paramuserid.Value = userid;
                    cmd.Parameters.Add(paramuserid);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);



                    /*   cmd.Parameters.Add("@Result", SqlDbType.Int);
                       cmd.Parameters["@Result"].Direction = ParameterDirection.Output;*/

                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();


                    reader.Read();

                    while (retvalue.Equals("1"))
                    {
                        cServiceProvider sp = new cServiceProvider();
                        sp.providers = new List<cPartialSender>();
                        sp.cDisplayOrder = (double)reader["cDisplayOrder"];
                        sp.cServiceCategory = reader["cServiceType"].ToString().Trim();
                        sp.cCount = 0;
                        do
                        {
                            if (reader["SenderID"] != null)
                            {
                                if (reader["SenderID"].ToString().Length > 0)
                                {
                                    cPartialSender c = new cPartialSender();
                                    sp.cCount = sp.cCount + 1;
                                    c.SenderID = (int)reader["SenderID"];
                                    c.SenderName = reader["SenderName"].ToString().Trim();
                                    c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();

                                    c.ContactHide = (Int16)reader["ContactHide"];
                                    c.cServiceType = reader["cServiceType"].ToString().Trim();
                                    c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();
                                    c.postcode= reader["POSTCODE"].ToString().Trim();

                                    if (reader["logopath"] != null)
                                        if (reader["logopath"].ToString().Length > 0)
                                            c.logopath = reader["logopath"].ToString().Trim();

                                    c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                                    c.ReviewReceived = (int)reader["ReviewReceived"];
                                    c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                                    c.msgLikeReceived = (int)reader["msgLikeReceived"];
                                    c.msgTotpublished = (int)reader["msgTotpublished"];
                                    c.msgReadBy = (int)reader["msgReadBy"];
                                    sp.providers.Add(c);
                                }
                            }

                        } while (reader.Read() && sp.cServiceCategory.Equals(reader["cServiceType"].ToString().Trim()));

                        ServiceProviders.Add(sp);
                    }
                }

            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            //return Ok(retvalue);*/
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_getPostcode", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlParameter paramuserid = new SqlParameter();
                    paramuserid.ParameterName = "@userID";
                    paramuserid.Value = userid;
                    cmd.Parameters.Add(paramuserid);

                    cmd.Parameters.Add("@postcode", SqlDbType.NVarChar, 6);
                    cmd.Parameters["@postcode"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    userPostcode = cmd.Parameters["@postcode"].Value.ToString();
             

                }
            }
            catch (Exception e)
            { }
            SendNotificationController s = new SendNotificationController();
            cPostCodeDistance distance = new cPostCodeDistance();

            for (int i = 0; i < ServiceProviders.Count; i++)
            {
                foreach (cPartialSender snd in ServiceProviders[i].providers)
                {
                    if (!userPostcode.Equals(snd.postcode))
                    {
                        distance = await s.GetCalculatedistanceAsync(userPostcode, snd.postcode);
                        if(distance.rows[0].elements[0].status.Contains("OK"))
                            snd.postcode = distance.rows[0].elements[0].distance.text;
                    }
                    else
                    {
                        snd.postcode = "LOCAL";
                    }

                }
            }


           return ServiceProviders;
        }
        
        [Route("api/ServicesbySearch")]
        [HttpGet]
        [AllowAnonymous]
        [ResponseType(typeof(List<cServiceProvider>))]
        public async Task<List<cServiceProvider>> ServicesbySearchv1_3(int userid, string userType, string SEARCH)
        {
            List<cServiceProvider> spCollctionlist = new List<cServiceProvider>();
            List<cPartialSender> ServiceProviders = new List<cPartialSender>();
            //List<cPartialSender> SendersList = new List<cPartialSender>();

            string retvalue = "1", userPostcode="";

            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_listServiceProviderforSearch", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlParameter paramsearch = new SqlParameter();
                    paramsearch.ParameterName = "@search";
                    paramsearch.Value = SEARCH;
                    cmd.Parameters.Add(paramsearch);

                    SqlParameter paramuserid = new SqlParameter();
                    paramuserid.ParameterName = "@userID";
                    paramuserid.Value = userid;
                    cmd.Parameters.Add(paramuserid);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);



                    /*   cmd.Parameters.Add("@Result", SqlDbType.Int);
                       cmd.Parameters["@Result"].Direction = ParameterDirection.Output;*/

                    con.Open();
                    reader = cmd.ExecuteReader();

                    //retvalue = cmd.Parameters["@Result"].Value.ToString();
                   
                    while (reader.Read())
                    {
                                  cPartialSender c = new cPartialSender();
                                    c.SenderID = (int)reader["SenderID"];
                                    c.SenderName = reader["SenderName"].ToString().Trim();
                                    c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                                    c.SenderContactNo_2= reader["SenderContactNo_2"].ToString().Trim();

                        c.ContactHide = (Int16)reader["ContactHide"];
                                    c.cServiceType = reader["cServiceType"].ToString().Trim();
                        c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();
                        c.postcode = reader["POSTCODE"].ToString().Trim();

                                    if (reader["logopath"] != null)
                                        if (reader["logopath"].ToString().Length > 0)
                                            c.logopath = reader["logopath"].ToString().Trim();

                                    c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                                    c.ReviewReceived = (int)reader["ReviewReceived"];
                                    c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                                    c.msgLikeReceived = (int)reader["msgLikeReceived"];
                                    c.msgTotpublished = (int)reader["msgTotpublished"];
                                    c.msgReadBy = (int)reader["msgReadBy"];
                        c.isFCMActive = reader["isFCMActive"].ToString().Trim();
                                    ServiceProviders.Add(c);
                                //}
                           // }

                       // } while (reader.Read() && sp.cServiceCategory.Equals(reader["cServiceType"].ToString().Trim()));

                       
                    }
                }

            }
            catch (Exception ex)
            {
                retvalue = ex.Message.ToString();
            }
            //return Ok(retvalue);*/
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_getPostcode", con);
                    cmd.CommandType = CommandType.StoredProcedure;


                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = userType;
                    cmd.Parameters.Add(paramuserType);

                    SqlParameter paramuserid = new SqlParameter();
                    paramuserid.ParameterName = "@userID";
                    paramuserid.Value = userid;
                    cmd.Parameters.Add(paramuserid);

                    cmd.Parameters.Add("@postcode", SqlDbType.NVarChar, 6);
                    cmd.Parameters["@postcode"].Direction = ParameterDirection.Output;

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    userPostcode = cmd.Parameters["@postcode"].Value.ToString();


                }
            }
            catch (Exception e)
            { }
            SendNotificationController s = new SendNotificationController();
            cPostCodeDistance distance = new cPostCodeDistance();

            foreach (cPartialSender snd in ServiceProviders)
            {
                if (!userPostcode.Equals(snd.postcode))
                {
                    distance = await s.GetCalculatedistanceAsync(userPostcode, snd.postcode);
                    if (distance.rows[0].elements[0].status.Contains("OK"))
                    {
                        snd.postcode = distance.rows[0].elements[0].distance.text;   ////the value in KMS. eg 2,435 KM
                        snd.ReviewReceived = int.Parse(distance.rows[0].elements[0].distance.value);/// the value in meters. eg 2435675 
                    }

                }
                else
                {
                    snd.postcode = "LOCAL";
                    snd.ReviewReceived = 0;
                }

            }


            for (int i = 0; i < ServiceProviders.Count; i++)
            {
                for (int j = 0; j < ServiceProviders.Count; j++)
                {
                    cPartialSender swp = new cPartialSender();
                    if (ServiceProviders[i].ReviewReceived < ServiceProviders[j].ReviewReceived)
                    {
                        swp = ServiceProviders[j];
                        ServiceProviders[j] = ServiceProviders[i];
                        ServiceProviders[i] = swp;
                    }
                }

            }
            cServiceProvider spprov = new cServiceProvider();
            spprov.providers = ServiceProviders;
            spprov.cCount = ServiceProviders.Count;
            spprov.cServiceCategory = "Results Found";
            spprov.cSvcDesc = SEARCH;
            spCollctionlist.Add(spprov);
            //return ServiceProviders;
            CreateEnquiries(userid, userType, spCollctionlist);
            return spCollctionlist;
        }


        [Route("api/ServicesSearch_v2")]
        [AllowAnonymous]
        [HttpPost]
        [ResponseType(typeof(List<cPartialSender>))]
        public async Task<List<cPartialSender>> ServicesbySearch_v2(cSearch search)
        {
            string uri;
            string  loclatlong = "-1,-1";
        
            List<cPartialSender> ServiceProviders = new List<cPartialSender>();

            try
            {

                string constr = ConfigurationManager.ConnectionStrings["MyAbDbContext"].ConnectionString;

                using (SqlConnection con = new SqlConnection(constr))
                {
                    SqlCommand cmd = new SqlCommand("sp_ServiceProviderforSearchwithUnReg", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader;

                    SqlParameter paramwhat = new SqlParameter();
                    paramwhat.ParameterName = "@what";
                    paramwhat.Value = search.what;
                    cmd.Parameters.Add(paramwhat);

                  
                    SqlParameter paramuserid = new SqlParameter();
                    paramuserid.ParameterName = "@userID";
                    paramuserid.Value = search.userID;
                    cmd.Parameters.Add(paramuserid);

                    SqlParameter paramuserType = new SqlParameter();
                    paramuserType.ParameterName = "@userType";
                    paramuserType.Value = search.userType;
                    cmd.Parameters.Add(paramuserType);

                    search = await findLatLngLocality(search);
                    
                    SqlParameter paramwhere = new SqlParameter();
                    paramwhere.ParameterName = "@where";
                    paramwhere.Value = search.where;
                    cmd.Parameters.Add(paramwhere);

                    if (search.where.Length <2)
                    {
                        return ServiceProviders;
                    }

                    SqlParameter paramlat = new SqlParameter();
                    paramlat.ParameterName = "@lat";
                    paramlat.Value = search.lat;
                    cmd.Parameters.Add(paramlat);

                    SqlParameter paramlng = new SqlParameter();
                    paramlng.ParameterName = "@lng";
                    paramlng.Value = search.lng;
                    cmd.Parameters.Add(paramlng);

                    con.Open();
                    reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cPartialSender c = new cPartialSender();
                        c.SenderID = (int)reader["SenderID"];
                        c.SenderName = reader["SenderName"].ToString().Trim();
                        c.SenderContactNo_1 = reader["SenderContactNo_1"].ToString().Trim();
                       

                        c.ContactHide = (Int16)reader["ContactHide"];
                        c.cServiceType = reader["cServiceType"].ToString().Trim();
                        c.ServiceDesc = reader["ServiceDesc"].ToString().Trim();
                        c.postcode = reader["POSTCODE"].ToString().Trim();

                        if (reader["logopath"] != null)
                            if (reader["logopath"].ToString().Length > 0)
                                c.logopath = reader["logopath"].ToString().Trim();

                        c.subscribeonoff = reader["subscribeonoff"].ToString().Trim();
                        c.ReviewReceived = (int)reader["ReviewReceived"];
                        c.msgCommentsReceived = (int)reader["msgCommentsReceived"];
                        c.msgLikeReceived = (int)reader["msgLikeReceived"];
                        c.msgTotpublished = (int)reader["msgTotpublished"];
                        c.msgReadBy = (int)reader["msgReadBy"];
                        c.address = reader["address"].ToString().Trim();
                        c.isFCMActive = reader["isFCMActive"].ToString().Trim();
                        ServiceProviders.Add(c);
                      

                    }


                }
            }
            catch (Exception ex)
            {
                // distance.status = "failed";
                //return ex.Message;

            }
            return ServiceProviders;

        }


        private async Task<cSearch> findLatLngLocality(cSearch search)
        {
            string uri, loclatlng;


            // uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=12.8069792,80.197437&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";

            // uri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode("Kelambakkam Village, Gdt Road, Vandalur, Chennai 600048") + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";


             if (search.where.Length > 5)
            {
                uri = "https://maps.googleapis.com/maps/api/geocode/json?address=" + HttpUtility.UrlEncode(search.where) + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";
            }


            else //if ((search.lat != (decimal)-1.0) && (search.lat != (decimal)-1.0)) // if user provide lat,lng, then  need to find the locality.
            {
                uri = "https://maps.googleapis.com/maps/api/geocode/json?latlng=" + search.lat + "," + search.lng + "&key=AIzaSyDZ239b7m3O6c1uZCZez5EXfNYCQ5L4dYY";

            }
           

            HttpClient client = new HttpClient();
            var getResult = client.GetAsync(uri);
            getResult.Wait();
            var result = getResult.Result;
            if (result.IsSuccessStatusCode)
            {
                RootObject cnt = new RootObject();
                cnt = await result.Content.ReadAsAsync<RootObject>();
                if (cnt.results.Count > 0)
                {
                    search.latlngstatus = "OK";
                    search.lat = (decimal)cnt.results[0].geometry.location.lat;
                    search.lng = (decimal)cnt.results[0].geometry.location.lng;
                    search.where = search.where + ","+ extractAddressComponent(cnt.results[0].address_components);//get all sublocality for this area. 

                }
                else
                {
                    search.lat = (decimal)-1.0;
                    search.lng = (decimal)-1.0;

                }
                loclatlng = cnt.results[0].geometry.location.lat.ToString() + "," + cnt.results[0].geometry.location.lng.ToString();
            }
            else
            {
                search.where = "";
                search.lat = (decimal)-1.0;
                search.lng = (decimal)-1.0;
            }
            return search;
        }
       
        [ResponseType(typeof(List<AddressComponent>))]
        private string extractAddressComponent(List<AddressComponent> ac)
        {
            string where = "";

            for (int i =0; i<ac.Count; i++)
            {
                for (int j = 0; j<ac[i].types.Count; j++)
                {
                    if (ac[i].types[j].Equals("sublocality_level_1") || ac[i].types[j].Equals("sublocality_level_2") 
                        || ac[i].types[j].Equals("sublocality_level_3") || ac[i].types[j].Equals("sublocality_level_4") 
                        || ac[i].types[j].Equals("postal_code"))
                    //ac[i].types[j].Equals("locality")
                    {
                        where = where + ac[i].long_name + ",";
                    }
                }
            }

            return where;

        }



        [Route("api/CreateEnquiries")]
        [HttpPost]
        [AllowAnonymous]
        public async Task CreateEnquiries(int userid, string userType, List<cServiceProvider> serProviders)
        {
            AuthorizationController ac = new AuthorizationController();

            MessageController mc = new MessageController();
            int smsCnt = 0;
            //System.Net.WebUtility.UrlEncode(serProviders[0].cSvcDesc);
            //HttpUtility.UrlEncode("search");
            foreach (cPartialSender snd in serProviders[0].providers)
            {
                ctblEnquiry cEnquiry = new ctblEnquiry();
                cEnquiry.FromUserID = userid;
                cEnquiry.FromUserType = userType;
                cEnquiry.ToUserID = snd.SenderID;
                cEnquiry.ToUserType = "S";
                /* THIS SENTENCE SHOULD NOT BE CHANGED, IT USED IN SP sp_addEnquiryAnswer */
                cEnquiry.EnquiryMessage = "Found your reference on searching '" +  serProviders[0].cSvcDesc.Replace(","," ").ToString() +"'. Distance: " + snd.postcode;
                cEnquiry.EnquiryID = 0;
                mc.PostEnquiry(cEnquiry);
                if (smsCnt < 10 /*&& snd.isFCMActive.Equals("Y") */)
                {
                    ac.SendSearchEnquirySMS(snd.SenderContactNo_2, System.Net.WebUtility.UrlEncode(serProviders[0].cSvcDesc.Replace(",", " ").ToString()), snd.postcode);
                    smsCnt++;
                }

            }

        }

   

        private bool ctblServicesExists(int id)
        {
            return db.Rs_tblServices.Count(e => e.ID == id) > 0;
        }
    }
}