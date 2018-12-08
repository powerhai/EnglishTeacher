using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EngWebSite.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json", "multipart/form-data")]
    public class PublisherController : Controller
    {
        private readonly FoolDbContext mContext;

        public PublisherController(FoolDbContext context)
        {
            mContext = context;
        }

        [HttpGet]
        public IEnumerable<Publisher> Get()
        {
            return mContext.Publishers.ToArray();
        }
        [HttpGet]
        [Route("GetPublishersAndBooks")]
        
        public IEnumerable<Publisher> GetPublishersAndBooks()
        {
            var ccc = mContext.Publishers.Include(a=>a.Books).ToArray();
            return ccc;
        }
         [HttpGet]
        [Route("GetPublisher")]
          public Publisher GetPublisher()
        {
            var ccc = mContext.Publishers.FirstOrDefault();
            return ccc;
        }
    }
}