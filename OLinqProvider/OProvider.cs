using System.Linq;
using System.Linq.Expressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace OLinqProvider
{
    public class OProvider :QueryProvider
    {
        private readonly string _url;
        private readonly string _tokenPath;

        public OProvider(string url):this(url,null)
        {    
        }
        public OProvider(string url, string tokenPath)
        {
            _url = url;
            if (!string.IsNullOrEmpty(tokenPath))
            {
                _tokenPath = tokenPath;
            }
           
        }
        public IQueryable<T> CreateQuery<T>(string collection)
        {
            return new OQuery<T>(this, collection);
        }

        public override TResult Execute<TResult>(Expression expression)
        {
            var collectionName = expression.GetCollectionName();

            var reuqestUrl = _url + '/' + collectionName;

            reuqestUrl += Translate(expression);

            var response =RequestHelper.Get(reuqestUrl);

            return DeserializeObject<TResult>(response);
        }

        private T DeserializeObject<T>(string json)
        {
                var jobject = JsonConvert.DeserializeObject(json) as JObject;
                JToken reference = null;
                if (!string.IsNullOrEmpty(_tokenPath))
                {
                    reference = jobject["d"][_tokenPath];
                    return JsonConvert.DeserializeObject<T>(reference.ToString());
                }
                reference = jobject["d"];
                if (reference == null)
                {
                    return JsonConvert.DeserializeObject<T>(json);
                }
                if (reference is JArray && reference.Count() > 1)
                {
                    return JsonConvert.DeserializeObject<T>(reference.ToString());
                }
                return JsonConvert.DeserializeObject<T>(reference[0].ToString());
        }

        private string Translate(Expression expression)
        {
            var visitor = new OExpressionVisitor();

            return visitor.Parse(expression);
        }
    }
}
