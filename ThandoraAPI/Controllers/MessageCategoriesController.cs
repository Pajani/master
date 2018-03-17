using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ThandoraAPI.Models;

namespace ThandoraAPI.Controllers
{
    public class MessageCategoriesController : ApiController
    {
        private MyAbDbContext db = new MyAbDbContext();

        // GET: api/MessageCategories
        [Route("api/GetMessageCategories")]
        public IQueryable<ctblMessageCategory> GetctblMessageCategories(string userType)
        {
            
         

            if (userType.Equals("R"))
                return db.ctblMessageCategories.Where(x =>x.userType.Equals("BOTH")).Select(z =>z);
            else
                    return db.ctblMessageCategories;
        }

        // GET: api/MessageCategories/5
        [ResponseType(typeof(ctblMessageCategory))]
        public IHttpActionResult GetctblMessageCategory(int id)
        {
            ctblMessageCategory ctblMessageCategory = db.ctblMessageCategories.Find(id);
            if (ctblMessageCategory == null)
            {
                return NotFound();
            }

            return Ok(ctblMessageCategory);
        }

        // PUT: api/MessageCategories/5
    /*    [ResponseType(typeof(void))]
        public IHttpActionResult PutctblMessageCategory(int id, ctblMessageCategory ctblMessageCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ctblMessageCategory.ID)
            {
                return BadRequest();
            }

            db.Entry(ctblMessageCategory).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ctblMessageCategoryExists(id))
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

        // POST: api/MessageCategories
        [ResponseType(typeof(ctblMessageCategory))]
        public IHttpActionResult PostctblMessageCategory(ctblMessageCategory ctblMessageCategory)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ctblMessageCategories.Add(ctblMessageCategory);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = ctblMessageCategory.ID }, ctblMessageCategory);
        }

        // DELETE: api/MessageCategories/5
        [ResponseType(typeof(ctblMessageCategory))]
        public IHttpActionResult DeletectblMessageCategory(int id)
        {
            ctblMessageCategory ctblMessageCategory = db.ctblMessageCategories.Find(id);
            if (ctblMessageCategory == null)
            {
                return NotFound();
            }

            db.ctblMessageCategories.Remove(ctblMessageCategory);
            db.SaveChanges();

            return Ok(ctblMessageCategory);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ctblMessageCategoryExists(int id)
        {
            return db.ctblMessageCategories.Count(e => e.ID == id) > 0;
        }*/
    }
}