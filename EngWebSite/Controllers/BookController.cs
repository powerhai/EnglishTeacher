using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EngWebSite.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json", "multipart/form-data")]
    public class BookController : Controller
    {
        private readonly FoolDbContext mContext;

        public BookController(FoolDbContext context)
        {
            mContext = context;
        }

        [HttpGet]
        [Route("GetAll")]
        public IEnumerable<Book> GetAll()
        {
            return mContext.Books.ToArray();
        }
        [HttpGet]
        [Route("GetBooksWithPublisher")]
        public IEnumerable<Book> Get(int publisherId)
        {
            return mContext.Books.Where(a => a.Publisher.Id == publisherId).ToArray();
        }
        
    }
}