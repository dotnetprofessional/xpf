using System;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace xpf.Http
{
    public class UriDetail
    {
        string _title;
        string _description;
        string _keywords;
        bool _supportsFlash;

        public UriDetail(string url, string htmlContent)
        {
            this.Url = url;
            this.Html = htmlContent;

            // Reset other fields to empty strings rather than have them as nulls
            _title = string.Empty;
            _description = string.Empty;
            _keywords = string.Empty;
        }

        public string Title
        {
            get
            {
                if (string.IsNullOrEmpty(_title))
                    _title= GetMatchText(this.Html, @"<title>([\s\S]*)</title>");

                return _title; 
                
            }
        }

        public string Description
        {
            get
            {
                if(string.IsNullOrEmpty(_description))
                    _description = GetMatchText(this.Html, "<meta name=\"description\"(?:.*)content=\"(.*)\"");

                return _description;
            }
        }

        public string Url { get; private set; }

        public string Html { get; private set; }

        public string Keywords
        {
            get
            {
                if(string.IsNullOrEmpty(_keywords))
                    _keywords = GetMatchText(this.Html, "<meta name=\"keywords\" content=\"(.*)\"");

                return _keywords;
            }
        }

        bool _supportsFlashSet = false;
        public bool SupportsFlash
        {
            get
            {
                if(!_supportsFlashSet)
                    _supportsFlash = !string.IsNullOrWhiteSpace(GetMatchText(this.Html, @"(\.swf|flashplayer)"));

                return _supportsFlash;
            }
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