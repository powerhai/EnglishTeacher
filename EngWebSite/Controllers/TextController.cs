using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EngWebSite.Controllers
{
    [Route("api/[controller]")]
    [Consumes("application/json", "multipart/form-data")]
    public class TextController : Controller
    {
        private readonly FoolDbContext mContext;

        public TextController(FoolDbContext context)
        {
            mContext = context;
        }

        [HttpPost]
        [Route("New")]
        public IActionResult CreateNew(NewTextModel model)
        {
            bool v = TryValidateModel(model);
            if (v == false)
                return this.NotFound();

            var publisher = mContext.Publishers.FirstOrDefault(a=>a.Title.Equals(model.Publisher));
            if(publisher == null)
            {
                publisher = new Publisher(){Title = model.Title};
                mContext.Publishers.Add(publisher);
            }
            var book = mContext.Books.FirstOrDefault(a=> a.Publisher == publisher && a.Title.Equals(model.Book));
            if(book == null){
                book = new Book(){ Title = model.Book , Publisher = publisher};
                mContext.Books.Add(book);
            }

            var text = mContext.Texts.FirstOrDefault(a=> a.Book == book && a.Title.Equals(model.Title)); // mContext.Texts.FirstOrDefault(a=>a.Title.Equals(model.Title))
            if(text == null){
                text = new Text(){ Title = model.Title, Body = model.Body, Book = book};
                mContext.Texts.Add(text);
            }else{
                text.Body = model.Body;
            }
            mContext.SaveChanges();
            return this.Ok(text.Id);
        }

        [HttpPost]
        [Route("UpdateText")]
        public IActionResult UpdateText(UpdateTextModel model){
            bool v = TryValidateModel(model);
            if (v == false)
                return this.NotFound();
            var txt = mContext.Texts.FirstOrDefault(a=>a.Id == model.Id);
            if(txt == null)
                return this.NotFound();

            txt.Title = model.Title;
            txt.Body = model.Body;
            var publisher = mContext.Publishers.FirstOrDefault(a=>a.Title.Equals(model.Publisher));
            if(publisher == null)
            {
                publisher = new Publisher(){Title = model.Publisher};
                mContext.Publishers.Add(publisher);
            }
            var book = mContext.Books.FirstOrDefault(a=> a.Publisher == publisher && a.Title.Equals(model.Book));
            if(book == null){
                book = new Book(){ Title = model.Book , Publisher = publisher};
                mContext.Books.Add(book);
            }
            
            txt.Book = book;
            mContext.SaveChanges();
            return Ok();
        }

        [HttpGet]
        [Route("GetTextsOfBook")]
        public IEnumerable<Text> GetTextsOfBook(int bookId){
            var list = mContext.Texts.Where(a=>a.Book.Id == bookId).ToList();
            return list;
        }
        
        [HttpGet]
        [Route("GetTexts")]
        public IEnumerable<LightTextModelA> GetTexts(){
            var list = mContext.Texts.Select(a=>new LightTextModelA(){ Id = a.Id, Title = a.Title, BookId = a.Book.Id});
            return list;
        }

        [HttpGet]
        [Route("GetText")]
        public Text GetText(int Id){
            return mContext.Texts.FirstOrDefault(a=>a.Id == Id);
        }
        [HttpGet]
        [Route("RemoveText")]
        public bool RemoveText(int Id){
            var text = mContext.Texts.FirstOrDefault(a=>a.Id == Id);
            if(text != null){
                mContext.Texts.Remove(text);
                mContext.SaveChanges();
            }
            return true;
        }
 
        public class LightTextModelA{
            public string Title{get;set;}
            public int BookId{get;set;}
            public int Id{get;set;}
        }
        public class UpdateTextModel{
            [Required]
            public int Id{get;set;}
            [Required]
            [StringLength(30)]
            public string Title { get; set; }

            [Required]
            [StringLength(30)]
            public string Publisher { get; set; }

            [Required]
            [StringLength(30)]
            public string Book { get; set; }

            [Required]
            public string Body{get;set;}

        }
        public class NewTextModel
        {
            [Required]
            [StringLength(10)]
            public string Title { get; set; }

            [Required]
            [StringLength(10)]
            public string Publisher { get; set; }

            [Required]
            [StringLength(10)]
            public string Book { get; set; }

            [Required]
            public string Body{get;set;}
        }
    }

}