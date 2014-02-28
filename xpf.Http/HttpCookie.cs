using System;

namespace xpf.Http
{
    public class HttpCookie
    {
        public HttpCookie()
        {
        }

        public HttpCookie(string rawCookie)
        {
            var headerParts = rawCookie.Split(';');
            foreach (var h in headerParts)
            {
                var keyValues = h.Split('=');
                switch (keyValues[0].Trim())
                {
                    case "expires":
                        this.Expiry = DateTime.Parse(keyValues[1]);
                        break;
                    case "path":
                        this.Path = keyValues[1];
                        break;
                    case "domain":
                        this.Domain = keyValues[1];
                        break;
                    default:
                        this.Name = keyValues[0];
                        this.Value = keyValues[1];
                        break;
                }
            }
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public DateTime Expiry { get; set; }

        public bool IsExpired { get { return this.Expiry < DateTime.Now; } }

        public string Domain { get; set; }

        public string Path { get; set; }
    }
}