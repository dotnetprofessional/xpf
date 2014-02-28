using System;
using System.Threading.Tasks;

namespace xpf.Http
{
    public interface IHttp
    {
        Task<HttpResponse<TR>> GetAsync<TR>(HttpRequest request);

        Task<HttpResponse<TR>> PostAsync<TR, T>(HttpRequest request, T data);

        string GetDomain(Uri uri);

        Task<UriDetail> GetWebPageDetail(Uri uri);

        string GetWebPageThumbnailUrl(Uri uri, ThumbnailSize size = ThumbnailSize.Large200x150);

        Task<WebSiteClassification> GetWebSiteClassification(HttpCookie sessionKey, Uri uri);

        Task<HttpCookie> GetWebSiteClassificationSessionCookie();

    }
}
