using System.Net;

namespace HevySharpTests;

public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Dictionary<string, (HttpStatusCode StatusCode, string Content)> _responses = new();
    private readonly List<HttpRequestMessage> _requests = [];

    public IReadOnlyList<HttpRequestMessage> Requests => _requests;

    public HttpRequestMessage? LastRequest => _requests.Count > 0 ? _requests[^1] : null;

    public void SetResponse(string urlContains, HttpStatusCode statusCode, string content)
    {
        _responses[urlContains] = (statusCode, content);
    }

    public void SetOkResponse(string urlContains, string content)
    {
        SetResponse(urlContains, HttpStatusCode.OK, content);
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _requests.Add(request);

        foreach (var (key, value) in _responses)
        {
            if (request.RequestUri?.ToString().Contains(key) == true)
            {
                return new HttpResponseMessage(value.StatusCode)
                {
                    Content = new StringContent(value.Content, System.Text.Encoding.UTF8, "application/json")
                };
            }
        }

        return new HttpResponseMessage(HttpStatusCode.NotFound)
        {
            Content = new StringContent("{}", System.Text.Encoding.UTF8, "application/json")
        };
    }

    public async Task<string?> GetLastRequestBody()
    {
        if (LastRequest?.Content is null) return null;
        return await LastRequest.Content.ReadAsStringAsync();
    }
}
