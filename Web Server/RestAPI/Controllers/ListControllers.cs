

using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RestAPI.Models;

namespace RestAPI.Controllers
{
    [Route("/list")]
    [ApiController]
    public class ListControllers
    {
        private AppDBContext _appDbContext;
        public ListControllers(AppDBContext appDBContext)
        {
            _appDbContext = appDBContext;
        }

        [HttpGet]
        public ActionResult<IEnumerable<Posts>> Get()
        {
            return _appDbContext.Post;
        }
        
        [HttpPost()]
        public ActionResult<Posts> Post([FromBody]Posts posts)
        {
            _appDbContext.Add(posts);
            _appDbContext.SaveChanges();
            return posts;
        }

        [HttpDelete("{id}")]
        public ActionResult<string> Delete(int id)
        {
            var posts = _appDbContext.Post.Find(id);
            _appDbContext.Attach(posts);
            _appDbContext.Remove(posts);
            _appDbContext.SaveChanges();
            return $"Delete data ID : {id}";
        }

        [HttpPatch("{id}")]
        public ActionResult<Posts> Update(int id,[FromBody] Posts updateList)
        {
            var update = _appDbContext.Post.Find(id);
            update.list = updateList.list;
            _appDbContext.SaveChanges();
            return update;
        }

        [HttpPatch]
        [Route("/list/status/{id}")]
        public ActionResult<Posts> updateStatus(int id,[FromBody] Posts updateList)
        {
            var update = _appDbContext.Post.Find(id);
            update.status = updateList.status;
            _appDbContext.SaveChanges();
            return update;
        }




    }
}