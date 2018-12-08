using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EngWebSite.Controllers
{
    [Route("api/sentence")]
    [Consumes("application/json", "multipart/form-data")]
    public class SentenceController : Controller
    {
        public SentenceController(FoolDbContext context)
        {
            mContext = context;
            var word = new Word() { Text = "wwww" };
            var ccc = this.mContext.Words.Add(word);
            this.mContext.SaveChangesAsync();
        }
        private readonly FoolDbContext mContext;
 

        [HttpPost]
        [Route("GetAudio")]
        public IActionResult GetAudio(string sentence)
        {
            var s = mContext.Sentences.FirstOrDefault();
            var response = File(s.Audio, "video/avi");
            return response;
        }
        [HttpPost]
        [Route("New")]
        public async Task<IActionResult> NewSentence(NewSentenceModel model)
        {
            var rv = this.TryValidateModel(model);
            if(rv == false)
                return this.NotFound();
            try
            {
                var text = mContext.Texts.First(a=>a.Id == model.TextID);

                var sa = mContext.Sentences.FirstOrDefault(a=>a.Text.Id == model.TextID && a.English == model.Sentence);
                if(sa == null){
                    sa = new Sentence(){ English = model.Sentence, Chinese = model.Chinese, Text = text};
                    mContext.Sentences.Add(sa);
                }else{
                    sa.Chinese = model.Chinese;
                }

                if(model.Audio != null){
                    var stream = new System.IO.MemoryStream();
                    await model.Audio.CopyToAsync(stream);
                    sa.Audio = stream.GetBuffer();        
                    sa.FileName = model.Audio.FileName;            
                }


                this.mContext.Sentences.Add(sa);
                await this.mContext.SaveChangesAsync();
                return Ok();
            }
            catch (System.Exception eee)
            {
                return this.StatusCode(500, eee.ToString());

            }
        }
        [HttpGet]
        [Route("GetSentenceOfText")]
        public IEnumerable<Sentence> GetSentenceOfText(int textId){
            var list = mContext.Sentences.Where(a=>a.Text.Id == textId);
            return list;
        }
    }
    public class NewSentenceModel
    {
        [Required]
        public int TextID { get; set; }
        [Required]
        public string Sentence { get; set; }
        [Required]
        public string Chinese { get; set; } 
        public IFormFile Audio { get; set; }
    }
}