using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;

namespace Telegram_bot
{
    public class Quest
    {
        public string category { get; set; }
        public string id { get; set; }
        public string correctAnswer { get; set; }
        public List<string> incorrectAnswers { get; set; }
        public string question { get; set; }
        public List<string> tags { get; set; }
        public string type { get; set; }
        public string difficulty { get; set; }
        public List<object> regions { get; set; }
        public bool isNiche { get; set; }
    }
    public class GetFromApi
    {
        private List<Quest> myQuest;
        private readonly string url;
        public GetFromApi(string url) 
        {
            this.url = url;
            WebRequest _request = HttpWebRequest.Create(url);
            WebResponse response = _request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());

            string Quest_Json = sr.ReadToEnd();
            myQuest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Quest>>(Quest_Json);
        }

        public void get()
        {
            WebRequest _request = HttpWebRequest.Create(url);
            WebResponse response = _request.GetResponse();
            StreamReader sr = new StreamReader(response.GetResponseStream());

            string Quest_Json = sr.ReadToEnd();
            myQuest = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Quest>>(Quest_Json);
        }

        public string GetQuestion()
        {
            return myQuest[0].question;
        }
        public List<string> GetIncAnswers()
        {

            return myQuest[0].incorrectAnswers;
        }
        public string GetCorAnswers()
        {

            return myQuest[0].correctAnswer;
        }
    }
}
