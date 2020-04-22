using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SualCavabAPI.Model.Database;
using SualCavabAPI.Model.Structs;

namespace SualCavabAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class sualcavabController : Controller
    {



        private readonly IWebHostEnvironment _hostingEnvironment;
        public IConfiguration Configuration;
        public sualcavabController(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            Configuration = configuration;

            _hostingEnvironment = hostingEnvironment;
        }


        [HttpGet]
        [Route("get/publications")]
        // GET: sualcavab/Details/5
        public ActionResult<List<PublicationsStruct>> getPublications(string mail, string pass, int topicId)
        {
            DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
            return select.publications(mail, pass, topicId);

        }
        [HttpGet]
        [Route("get/top/publications")]
        // GET: sualcavab/Details/5
        public ActionResult<List<PublicationsStruct>> getTopPublications(string mail,string pass)
        {
            DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
            return select.topPublicactions(mail,pass);

        }
        [HttpGet]
        [Route("get/comments")]
        // GET: sualcavab/Details/5
        public ActionResult<List<CommentStruct>> getComments(int pID)
        {
            DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
            return select.comments(pID);

        }
        [HttpGet]
        [Route("get/pTopics")]
        // GET: sualcavab/Details/5
        public ActionResult<List<TopicStruct>> getTopics(int topicID)
        {
            DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
            return select.pTopics();

        }

        [HttpPost]
        [Route("add/reaction")]
        // GET: sualcavab/Details/5
        public ActionResult<List<StatusStruct>> addReaction(string mail, string pass, int publicationID, int reaction)
        {
            DbInsert insert = new DbInsert(Configuration, _hostingEnvironment);
            return insert.addReaction(mail, pass, publicationID, reaction);

        }
        [HttpPost]
        [Route("add/comment")]
        // GET: sualcavab/Details/5
        public ActionResult<List<StatusStruct>> addComment(string mail, string pass, int publicationID, int commentID, string comment)
        {
            DbInsert insert = new DbInsert(Configuration, _hostingEnvironment);
            return insert.addComment(mail, pass, publicationID, commentID,comment);

        }
      //  [HttpPost]
       // [Route("accounts/add/post")]
        // GET: sualcavab/Details/5
        //public ActionResult<List<StatusStruct>> addPost([FromBody]NewPublication newPublication)
       // {
         //   DbInsert insert = new DbInsert(Configuration, _hostingEnvironment);
       //     return insert.signUp(newUser);

//        }
        [HttpPost]
        [Route("accounts/register")]
        // GET: sualcavab/Details/5
        public ActionResult<List<StatusStruct>> signUp([FromBody]NewUser newUser)
        {
            DbInsert insert = new DbInsert(Configuration, _hostingEnvironment);
            return insert.signUp(newUser);

        }
        [HttpPost]
        [Route("accounts/login")]
        // GET: sualcavab/Details/5
        public ActionResult<List<User>> logIn(string mail,string pass)
        {
            DbSelect select = new DbSelect(Configuration, _hostingEnvironment);
          
            return select.logIn(mail,pass);

        }

        // GET: sualcavab/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: sualcavab/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: sualcavab/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: sualcavab/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: sualcavab/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: sualcavab/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}