using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DVVTwitterTalk
{
    class Program
    {
        // oauth application keys
        static string oauth_token = "4401358282-1AMKkalrPWPj2vDVXtrKPUbedCSp9e8FrNJa2wa"; //3
        static string oauth_token_secret = "fOxB2e5QtCifVYUoyTEV3gnTSxG5eGv4VaJ4f42z6PzcB"; //4
        static string oauth_consumer_key = "K4bq6JSglLt0Zmf4FK2Ynz72O"; //1
        static string oauth_consumer_secret = "1cKm2DozVQUuU8VazhWSXNLtKPZuQ8sIF24W2lhVjCBrhQmAU7"; //2

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Post an Update <p>");
            Console.WriteLine("Get Tweets <g>");
            string operation = Console.ReadLine();

            switch (operation)
            {
                case "p":
                    PostUpdate();
                    break;
                case "g":
                    GetTweets();
                    break;
                default:
                    break;
            }

        }

        private static void PostUpdate()
        {
            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";

            // unique request details
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));


            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // message api details
            var status = "Good afternoon from #advprog!!";
            var resource_url = "https://api.twitter.com/1.1/statuses/update.json";

            // create oauth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&status={6}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version,
                                        Uri.EscapeDataString(status)
                                        );

            baseString = string.Concat("POST&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_signature=\"{2}\", " +
                               "oauth_signature_method=\"{3}\", oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_version)
                                    );

            // make the request
            var postBody = "status=" + Uri.EscapeDataString(status);

            ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";


            using (Stream stream = request.GetRequestStream())
            {
                byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
                stream.Write(content, 0, content.Length);
            }

            try
            {
                WebResponse response = request.GetResponse();
                StreamReader oSR = new StreamReader(response.GetResponseStream());
                var responseResult = oSR.ReadToEnd().ToString();
            }
            catch (Exception ex)
            {

                Console.WriteLine("Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authHeader);

            }
            Console.ReadLine();
        }

        private static void GetTweets()
        {
             
            // oauth implementation details
            var oauth_version = "1.0";
            var oauth_signature_method = "HMAC-SHA1";

            // unique request details
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));


            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // message api details
            var resource_url = "https://api.twitter.com/1.1/statuses/user_timeline.json";
            resource_url = "https://api.twitter.com/1.1/statuses/retweets_of_me.json";

            resource_url = "https://api.twitter.com/1.1/followers/list.json";

            // create oauth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version);

            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_consumer_key=\"{0}\", oauth_nonce=\"{1}\", oauth_signature=\"{2}\", " +
                               "oauth_signature_method=\"{3}\", oauth_timestamp=\"{4}\", oauth_token=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_version)
                                    );

            // make the request
            var postBody = "screen_name=Bizzerk222&count=5";  // user_timeline
            postBody = "count=1";                             // retweets_of_me

            ServicePointManager.Expect100Continue = false;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);
            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";


            //using (Stream stream = request.GetRequestStream())
            //{
            //    byte[] content = ASCIIEncoding.ASCII.GetBytes(postBody);
            //    stream.Write(content, 0, content.Length);
            //}

            try
            {
                //WebResponse response = request.GetResponse();
                //StreamReader oSR = new StreamReader(response.GetResponseStream());
                //var responseResult = oSR.ReadToEnd().ToString();

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {

                    if (response.StatusCode != HttpStatusCode.OK)
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    //DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(Response));
                    //object objResponse = jsonSerializer.ReadObject(response.GetResponseStream());
                    //Response jsonResponse = objResponse as Response;
                    //return jsonResponse;

                    StreamReader oSR = new StreamReader(response.GetResponseStream());
                    var responseResult = oSR.ReadToEnd().ToString();
                    //Console.WriteLine(responseResult);

                    dynamic oResp = JsonConvert.DeserializeObject(responseResult);

                    int usercount = 0;

                    foreach(var data in oResp.users)
                    {
                        Console.WriteLine(usercount.ToString() + ") " +
                                          data.name + " (" + data.screen_name + ")");
                        usercount++;
                    }

                    Console.WriteLine("Users : " + usercount.ToString());
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine("Twitter Post Error: " + ex.Message.ToString() + ", authHeader: " + authHeader);

            }
            Console.ReadLine();
        }
    }
}
