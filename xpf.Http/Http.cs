using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace xpf.Http
{
    public class Http : IHttp
    {
        public async Task<HttpResponse<TR>> GetAsync<TR>(HttpRequest request)
        {
            var clientHandler = new System.Net.Http.HttpClientHandler();

            clientHandler.AllowAutoRedirect = request.AllowAutoRedirect;

            var client = new System.Net.Http.HttpClient(clientHandler);

            if (request.AuthenticationToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(request.AuthenticationToken.Scheme, request.AuthenticationToken.Value);
            }

            foreach (var h in request.Headers)
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
            System.Net.Http.HttpResponseMessage response = null;

            response = await client.GetAsync(request.Url);
            var result = default(TR);
            var error = "";
            if (response.StatusCode == HttpStatusCode.OK)
                result = await this.ConvertContentToType<TR>(response.Content, request.RequestFormat);
            else
            {
                error = await this.ConvertContentToType<string>(response.Content, HttpFormat.Text);
            }

            // Depending on the format c
            var requestResponse = new HttpResponse<TR>(response.StatusCode, result, error);
            // Transfer the headers too
            foreach (var header in response.Headers)
                requestResponse.Headers.Add(header.Key, new HttpHeader { Key = header.Key, Value = new List<string>(header.Value) });

            return requestResponse;
        }

        public async Task<HttpResponse<TR>> PostAsync<TR, TC>(HttpRequest request, TC data)
        {
            var clientHandler = new System.Net.Http.HttpClientHandler();

            clientHandler.AllowAutoRedirect = request.AllowAutoRedirect;

            var client = new System.Net.Http.HttpClient(clientHandler);

            var content = new System.Net.Http.StringContent(request.Serialize(data));

            foreach (var h in request.Headers)
                switch (h.Key)
                {
                    case "Content-Type":
                        content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(h.Value[0]);
                        break;
                    default:
                        client.DefaultRequestHeaders.Add(h.Key, h.Value);
                        break;
                }

            if (request.Cookies.Count > 0)
            {
                var cookieContainer = new CookieContainer();
                foreach (var c in request.Cookies)
                    cookieContainer.Add(new Uri(request.Url), new Cookie(c.Name, c.Value));
                clientHandler.CookieContainer = cookieContainer;
            }

            if (request.AuthenticationToken != null)
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(request.AuthenticationToken.Scheme, request.AuthenticationToken.Value);
            }


            var response = await client.PostAsync(request.Url, content);
            var result = default(TR);
            var error = "";
            if (response.StatusCode == HttpStatusCode.OK)
                result = await this.ConvertContentToType<TR>(response.Content, request.RequestFormat);
            else
            {
                error = await this.ConvertContentToType<string>(response.Content, HttpFormat.Text);
            }
            return new HttpResponse<TR>(response.StatusCode, result, error);
        }

        public string GetDomain(Uri uri)
        {
            var host = uri.Host;

            // Split the host into multiple parts
            var parts = host.Split('.');
            var domain = "";
            var number = parts.Length;

            domain += string.Format("{0}.{1}", parts[number - 2], parts[number - 1]);
            if (number > 2 && parts[number - 1].Length == 2)
                domain = parts[number - 3] + "." + domain;

            return domain;
        }

        private async Task<TC> ConvertContentToType<TC>(System.Net.Http.HttpContent content, HttpFormat format)
        {
            return await this.ConvertHttpStreamToType<TC>(await content.ReadAsStreamAsync(), format);
        }

        public async Task<TC> ConvertHttpStreamToType<TC>(Stream stream, HttpFormat format)
        {
            object result = null;

            switch (format)
            {
                case HttpFormat.Text:
                    result = await new StreamReader(stream).ReadToEndAsync();
                    break;
                case HttpFormat.XML:
                    using (stream)
                    {
                        result = XmlSerializer.DeserializeFromStream<TC>(stream);
                    }
                    break;

                case HttpFormat.JSON:
                    using (stream)
                    {
                        result = JsonConvert.DeserializeObject<TC>(await new StreamReader(stream).ReadToEndAsync());
                    }
                    break;
            }

            return (TC)result;
        }

        public async Task<UriDetail> GetWebPageDetail(Uri uri)
        {
            UriDetail detail = null;
            await Task.Run(async () =>
            {
                var uriDetail = new UriDetail { Uri = uri };

                // Fetch the html for the Uri
                var htmlResponse = await this.GetAsync<string>(new HttpRequest { Url = uri.AbsoluteUri });
                if (htmlResponse.StatusCode == HttpStatusCode.OK)
                {
                    var html = htmlResponse.Content;
                    // Parse the details out of the html
                    uriDetail.Title = this.GetMatchText(html, @"<title>([\s\S]*)</title>");
                    uriDetail.Description = this.GetMatchText(html, "<meta name=\"description\"(?:.*)content=\"(.*)\"");
                    uriDetail.Keywords = this.GetMatchText(html, "<meta name=\"keywords\" content=\"(.*)\"");
                    uriDetail.Html = html;
                    uriDetail.ThumbnailUrl = this.GetWebPageThumbnailUrl(uri);
                    uriDetail.SupportsFlash = !string.IsNullOrWhiteSpace(this.GetMatchText(html, @"(\.swf|flashplayer)"));

                    detail = uriDetail;
                }
                else
                    throw new IOException(htmlResponse.Error);
            });
            return detail;
        }

        public string GetWebPageThumbnailUrl(Uri uri, ThumbnailSize size = ThumbnailSize.Large200x150)
        {
            var sizeCode = "";
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

                var request = new HttpRequest { Url = "http://global.sitesafety.trendmicro.com/result.php" };
                request.Headers.Add(new HttpHeader { Key = "Content-Type", Value = new List<string> { "application/x-www-form-urlencoded" } });
                request.Cookies.Add(sessionCookie);
                var postData = string.Format("urlname={0}&getinfo=Check+Now", uri.AbsoluteUri);
                var htmlResponse = await this.PostAsync<string, string>(request, postData);
                if (htmlResponse.StatusCode == HttpStatusCode.OK)
                {
                    var matches = Regex.Matches(htmlResponse.Content, "<h5>(.*)</h5>");
                    if (matches.Count == 2)
                    {
                        classification = new WebSiteClassification { SecurityRisk = matches[0].Groups[1].Captures[0].Value, Category = matches[1].Groups[1].Captures[0].Value };
                    }
                }
            });


            return classification;
        }

        public async Task<HttpCookie> GetWebSiteClassificationSessionCookie()
        {
            HttpCookie sessionCookie = null;
            // Get Session Key
            var htmlResponse = await this.GetAsync<string>(new HttpRequest { Url = "http://global.sitesafety.trendmicro.com/index.php", AllowAutoRedirect = false });
            if (htmlResponse.StatusCode == HttpStatusCode.OK | htmlResponse.StatusCode == HttpStatusCode.Redirect)
            {
                // Obtain the session key from the request
                var cookies = htmlResponse.Headers["Set-Cookie"];
                foreach (var cookie in cookies.Value)
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

        private Dictionary<string, string> GetHeaderValues(string headerValue)
        {
            var values = new Dictionary<string, string>();
            var headerParts = headerValue.Split(';');
            foreach (var h in headerParts)
            {
                var keyValues = h.Split('=');
                values.Add(keyValues[0], keyValues[1]);
            }

            return values;
        }

        private string GetMatchText(string text, string pattern)
        {
            var match = Regex.Match(text, pattern);
            if (match.Success && match.Groups.Count == 2)
            {
                var value = match.Groups[1].Value;
                value = value.Replace(Environment.NewLine, "").Trim();
                return value;
            }
            else
                return "";
        }
    }
}
