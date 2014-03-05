using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace xpf.Http
{
    public class Http : IHttp
    {
        public Http()
        {
        }

        public Http(HttpMessageHandler messageHandler)
        {
            MessageHandler = messageHandler;
        }

        protected HttpMessageHandler MessageHandler { get; set; }

        public async Task<HttpResponse<TR>> GetAsync<TR>(HttpRequest request) 
            where TR : class
        {
            HttpClient client;
            if (MessageHandler == null)
            {
                var clientHandler = new HttpClientHandler();

                clientHandler.AllowAutoRedirect = request.AllowAutoRedirect;

                client = new HttpClient();
            }
            else
            {
                client = new HttpClient(this.MessageHandler)
                {
                    BaseAddress = new Uri("http://www.dummysite.com")
                };
            }

            if (request.AuthenticationToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(request.AuthenticationToken.Scheme, request.AuthenticationToken.Value);
            }

            foreach (HttpHeader h in request.Headers)
            {
                switch (h.Key)
                {
                    case "Content-Type":
                        // Not valid for a GetRequest so ignore.
                        break;
                    default:
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        break;
                }
            }

            client.DefaultRequestHeaders.ExpectContinue = false;
            HttpResponseMessage response = null;

            response = await client.GetAsync(request.Url);
            TR result = default(TR);
  
            var rawContent = await new StreamReader(await response.Content.ReadAsStreamAsync()).ReadToEndAsync();

            string error = "";
            if (response.StatusCode == HttpStatusCode.OK)
                result = await ConvertHttpContentToType<TR>(rawContent, request.RequestFormat);
            else
            {
                error = rawContent;
            }

            // Depending on the format c
            var requestResponse = new HttpResponse<TR>(request.Url, response.StatusCode, result, error, rawContent);
            // Transfer the headers too
            foreach (var header in response.Headers)
                requestResponse.Headers.Add(header.Key, new HttpHeader {Key = header.Key, Value = new List<string>(header.Value)});

            return requestResponse;
        }

        public async Task<HttpResponse<TR>> PostAsync<TR, TC>(HttpRequest request, TC data) where TR : class
        {
            var clientHandler = new HttpClientHandler();

            clientHandler.AllowAutoRedirect = request.AllowAutoRedirect;

            var client = new HttpClient(clientHandler);

            var content = new StringContent(request.Serialize(data));

            foreach (HttpHeader h in request.Headers)
                switch (h.Key)
                {
                    case "Content-Type":
                        content.Headers.ContentType = new MediaTypeHeaderValue(h.Value[0]);
                        break;
                    default:
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        break;
                }

            if (request.Cookies.Count > 0)
            {
                var cookieContainer = new CookieContainer();
                foreach (HttpCookie c in request.Cookies)
                    cookieContainer.Add(new Uri(request.Url), new Cookie(c.Name, c.Value));
                clientHandler.CookieContainer = cookieContainer;
            }

            if (request.AuthenticationToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(request.AuthenticationToken.Scheme, request.AuthenticationToken.Value);
            }


            HttpResponseMessage response = await client.PostAsync(request.Url, content);
            TR result = default(TR);
            var rawContent = await new StreamReader(await response.Content.ReadAsStreamAsync()).ReadToEndAsync();

            string error = "";
            if (response.StatusCode == HttpStatusCode.OK)
                result = await ConvertHttpContentToType<TR>(rawContent, request.RequestFormat);
            else
            {
                error = rawContent;
            }
            return new HttpResponse<TR>(request.Url, response.StatusCode, result, error, rawContent);
        }

        public string GetDomain(Uri uri)
        {
            string host = uri.Host;

            // Split the host into multiple parts
            string[] parts = host.Split('.');
            string domain = "";
            int number = parts.Length;

            domain += string.Format("{0}.{1}", parts[number - 2], parts[number - 1]);
            if (number > 2 && parts[number - 1].Length == 2)
                domain = parts[number - 3] + "." + domain;

            return domain;
        }

        //public async Task<UriDetail> GetWebPageDetail(Uri uri)
        //{
        //    UriDetail detail = null;
        //    await Task.Run(async () =>
        //    {
        //        var uriDetail = new UriDetail {Uri = uri};

        //        // Fetch the html for the Uri
        //        HttpResponse<string> htmlResponse = await GetAsync<string>(new HttpRequest {Url = uri.AbsoluteUri});
        //        if (htmlResponse.StatusCode == HttpStatusCode.OK)
        //        {
        //            string html = htmlResponse.Content;
        //            // Parse the details out of the html
        //            uriDetail.Title = GetMatchText(html, @"<title>([\s\S]*)</title>");
        //            uriDetail.Description = GetMatchText(html, "<meta name=\"description\"(?:.*)content=\"(.*)\"");
        //            uriDetail.Keywords = GetMatchText(html, "<meta name=\"keywords\" content=\"(.*)\"");
        //            uriDetail.Html = html;
        //            uriDetail.ThumbnailUrl = GetWebPageThumbnailUrl(uri);
        //            uriDetail.SupportsFlash = !string.IsNullOrWhiteSpace(GetMatchText(html, @"(\.swf|flashplayer)"));

        //            detail = uriDetail;
        //        }
        //        else
        //            throw new IOException(htmlResponse.Error);
        //    });
        //    return detail;
        //}

        public string GetWebPageThumbnailUrl(Uri uri, ThumbnailSize size = ThumbnailSize.Large200x150)
        {
            string sizeCode = "";
            switch (size)
            {
                case ThumbnailSize.ExtraLarge320x240:
                    sizeCode = "xlg";
                    break;
                case ThumbnailSize.Large200x150:
                    sizeCode = "lg";
                    break;
                case ThumbnailSize.Micro75x56:
                    sizeCode = "mcr";
                    break;
                case ThumbnailSize.Small120x90:
                    sizeCode = "sm";
                    break;
                case ThumbnailSize.Tiny90x68:
                    sizeCode = "tny";
                    break;
                case ThumbnailSize.VerySmall100x75:
                    sizeCode = "vsm";
                    break;
            }
            return string.Format("http://images.shrinktheweb.com/xino.php?stwembed=1&stwu=adb83&stwinside=1&stwsize={1}&stwurl=http://{0}",
                uri.AbsoluteUri, sizeCode);
        }

        public async Task<WebSiteClassification> GetWebSiteClassification(HttpCookie sessionCookie, Uri uri)
        {
            WebSiteClassification classification = null;

            await Task.Run(async () =>
            {
                if (sessionCookie == null)
                    throw new ArgumentException("Session cookie is null and is required.");

                // Now make the call to get the classifiation.

                var request = new HttpRequest {Url = "http://global.sitesafety.trendmicro.com/result.php"};
                request.Headers.Add(new HttpHeader {Key = "Content-Type", Value = new List<string> {"application/x-www-form-urlencoded"}});
                request.Cookies.Add(sessionCookie);
                string postData = string.Format("urlname={0}&getinfo=Check+Now", uri.AbsoluteUri);
                HttpResponse<string> htmlResponse = await PostAsync<string, string>(request, postData);
                if (htmlResponse.StatusCode == HttpStatusCode.OK)
                {
                    MatchCollection matches = Regex.Matches(htmlResponse.Content, "<h5>(.*)</h5>");
                    if (matches.Count == 2)
                    {
                        classification = new WebSiteClassification
                        {
                            SecurityRisk = matches[0].Groups[1].Captures[0].Value,
                            Category = matches[1].Groups[1].Captures[0].Value
                        };
                    }
                }
            });


            return classification;
        }

        public async Task<HttpCookie> GetWebSiteClassificationSessionCookie()
        {
            HttpCookie sessionCookie = null;
            // Get Session Key
            HttpResponse<string> htmlResponse =
                await GetAsync<string>(new HttpRequest {Url = "http://global.sitesafety.trendmicro.com/index.php", AllowAutoRedirect = false});
            if (htmlResponse.StatusCode == HttpStatusCode.OK | htmlResponse.StatusCode == HttpStatusCode.Redirect)
            {
                // Obtain the session key from the request
                HttpHeader cookies = htmlResponse.Headers["Set-Cookie"];
                foreach (string cookie in cookies.Value)
                {
                    if (cookie.Contains("session_id"))
                    {
                        sessionCookie = new HttpCookie(cookie);
                    }
                }
            }
            else
                throw new IOException(htmlResponse.Error);

            return sessionCookie;
        }

        public async Task<TC> ConvertHttpContentToType<TC>(string content, HttpFormat format) where TC : class

        {
            object result = null;

            switch (format)
            {
                case HttpFormat.Text:
                    result = content;
                    break;
                case HttpFormat.XML:
                    result = XmlSerializer.Deserialize<TC>(content);
                    break;

                case HttpFormat.JSON:
                    result = JsonConvert.DeserializeObject<TC>(content);
                    break;
            }

            return (TC) result;
        }

        Dictionary<string, string> GetHeaderValues(string headerValue)
        {
            var values = new Dictionary<string, string>();
            string[] headerParts = headerValue.Split(';');
            foreach (string h in headerParts)
            {
                string[] keyValues = h.Split('=');
                values.Add(keyValues[0], keyValues[1]);
            }

            return values;
        }

        string GetMatchText(string text, string pattern)
        {
            Match match = Regex.Match(text, pattern);
            if (match.Success && match.Groups.Count == 2)
            {
                string value = match.Groups[1].Value;
                value = value.Replace(Environment.NewLine, "").Trim();
                return value;
            }
            return "";
        }
    }
}