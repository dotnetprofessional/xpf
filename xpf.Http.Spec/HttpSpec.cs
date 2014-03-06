using System.Net;
using System.Runtime.CompilerServices;
using FluentAssertions;
using Machine.Specifications;
using Nancy.Helpers;

namespace xpf.Http.Spec
{
    [Subject("")]
    public class HttpSpec
    {
        public static NancyTesting NancyTesting;

        Establish context = () =>
        {
            NancyTesting = new NancyTesting();
            NancyTesting.Start();
        };
        Cleanup cleanup = () => NancyTesting.Stop();

        [Subject("")]
        public class When_requesting_a_valid_url_that_is_not_a_html_page_as_text
        {
            static HttpResponse<string> Result;
            static string Url;

            Establish context = () =>
            {
                Url = "/When_requesting_a_valid_url_that_is_not_a_html_page_as_text";
                var http = new Http(NancyTesting.Server.Handler);
                Result = http.GetAsync<string>(new HttpRequest { Url = Url}).Result;
            };

            It should_have_status_code_of_OK = () => Result.StatusCode.Should().Be(HttpStatusCode.OK);
            It should_not_have_an_error_set = () => Result.Error.Should().BeEmpty();
            It should_return_the_contents_of_the_Url = () => Result.Content.Should().Be("this is simple text");
            It should_not_have_any_headers = () => Result.Headers.Count.Should().Be(0);
            It should_have_the_correct_Url_in_response = () => Result.Url.Should().BeEquivalentTo(Url);

            It should_have_a_valid_details = () => Result.Detail.Should().NotBeNull();
            It should_have_correct_Url_in_detail = () => Result.Detail.Url.Should().BeEquivalentTo(Url);
            It should_have_no_title_in_detail = () => Result.Detail.Title.Should().BeEmpty();
            It should_have_no_description_in_detail = () => Result.Detail.Description.Should().BeEmpty();
            It should_have_no_keywords_in_detail = () => Result.Detail.Keywords.Should().BeEmpty();
            It should_not_support_flash_in_detail = () => Result.Detail.SupportsFlash.Should().BeFalse();
        }

        [Subject("")]
        public class When_requesting_a_url_that_does_not_exist
        {
            static HttpResponse<string> Result;
            static string Url;

            Establish context = () =>
            {
                Url = "/I_Dont_Exist";
                var http = new Http(NancyTesting.Server.Handler);
                Result = http.GetAsync<string>(new HttpRequest { Url = Url }).Result;
            };

            It should_have_status_code_of_NotFound = () => Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            It should_have_an_error_set = () => Result.Error.Should().NotBeNullOrEmpty();
            It should_return_the_contents_of_the_Url_as_null = () => Result.Content.Should().BeNull();
            It should_not_have_any_headers = () => Result.Headers.Count.Should().Be(0);
            It should_have_the_correct_Url_in_response = () => Result.Url.Should().BeEquivalentTo(Url);

            It should_have_a_valid_details = () => Result.Detail.Should().NotBeNull();
            It should_have_correct_Url_in_detail = () => Result.Detail.Url.Should().BeEquivalentTo(Url);
            It should_have_404_in_title_in_detail = () => Result.Detail.Title.Should().Be("404");
            It should_have_no_description_in_detail = () => Result.Detail.Description.Should().BeEmpty();
            It should_have_no_keywords_in_detail = () => Result.Detail.Keywords.Should().BeEmpty();
            It should_not_support_flash_in_detail = () => Result.Detail.SupportsFlash.Should().BeFalse();
        }
    }
}
