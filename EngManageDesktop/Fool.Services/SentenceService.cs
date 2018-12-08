using System.Collections.Generic;
using Fool.Contracts;
using Fool.Models;
using RestSharp;
namespace Fool.Services
{
    public class SentenceService : ISentenceService
    {
        private readonly IConfigService mConfigService;
        public SentenceService(IConfigService configService)
        {
            mConfigService = configService;
        }
        public IEnumerable<Sentence> GetSentencesOfText(int textId)
        {
            var client = new RestClient(mConfigService.GetServerPath());
            var request = new RestRequest("api/sentence/GetSentenceOfText", Method.GET);
            request.AddParameter("textId", textId);
            var response = client.Execute<List<Sentence>>(request);
            return response.Data; 
        }
        public bool CreateSentence(int textId, string sentence, string chinese, string audioFile)
        {

            return true;
        }
    }

}