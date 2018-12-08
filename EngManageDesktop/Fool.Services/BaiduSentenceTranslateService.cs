using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Fool.Contracts;
using RestSharp;
namespace Fool.Services
{
    public class BaiduSentenceTranslateService : ISentenceTranslateService
    {
        private const string APP_ID = "20180527000167467";
        private const string KEY = "1cuzoRdGSExIdAWbecTd";
        private const string API_URI = "https://fanyi-api.baidu.com/api/trans/vip/translate"; 
        public   string Translate(string sentence)
        {
            var client = new RestClient(API_URI);
            var request = new RestRequest(Method.POST);
            var ro = new System.Random();
            var s = ro.Next(100000, 999999);
            var sign = EncryptWithMD5(APP_ID + sentence + s + KEY);

            request.AddParameter("q", sentence);
            request.AddParameter("from", "en");
            request.AddParameter("to", "zh");
            request.AddParameter("appid", APP_ID);
            request.AddParameter("salt", s);
            request.AddParameter("sign", sign);
            var response =   client.Execute<ResData>(request);
            return response.Data.trans_result[0]?.Dst;
        }
        private   string EncryptWithMD5(string source)
        {
            byte[] sor = Encoding.UTF8.GetBytes(source);
            MD5 md5 = MD5.Create();
            byte[] result = md5.ComputeHash(sor);
            StringBuilder strbul = new StringBuilder(40);
            for (int i = 0; i < result.Length; i++) {
                strbul.Append(result[i].ToString("x2"));//加密结果"x2"结果为32位,"x3"结果为48位,"x4"结果为64位

            }
            return strbul.ToString();
        }
        class ResData
        {
            public List<SrcDstData> trans_result { get; set; }
        }

        class SrcDstData
        {
            public string src { get; set; }
            public string Dst { get; set; }
        }
    }
}