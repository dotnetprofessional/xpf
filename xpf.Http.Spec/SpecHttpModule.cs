using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nancy;

namespace xpf.Http.Spec
{
    public class SpecHttpModule : NancyModule
    {
        public SpecHttpModule()
        {
            Get["/When_requesting_a_valid_url_that_is_not_a_html_page_as_text"] = _ => "this is simple text";
        }
    }
}
