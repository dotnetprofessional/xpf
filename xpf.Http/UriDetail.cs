using System;

namespace xpf.Http
{
    public class UriDetail
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string ThumbnailUrl { get; set; }

        public Uri Uri { get; set; }

        public string Html { get; set; }

        public string Keywords { get; set; }

        public bool SupportsFlash { get; set; }
    }
}