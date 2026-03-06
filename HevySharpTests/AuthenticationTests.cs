using HevySharp;

namespace HevySharpTests;

public class AuthenticationTests
{
    [Test]
    public async Task AuthoriseHevy_WithValidKey_ReturnsTrue()
    {
        var handler = new MockHttpMessageHandler();
        handler.SetOkResponse("/v1/user/info", """{"username":"test"}""");
        var api = new HevyAPI(new HttpClient(handler));

        var result = await api.AuthoriseHevy("valid-key");

        Assert.That(result, Is.True);
        Assert.That(api.IsAuthorised, Is.True);
    }

    [Test]
    public async Task AuthoriseHevy_WithInvalidKey_ReturnsFalse()
    {
        var handler = new MockHttpMessageHandler();
        handler.SetResponse("/v1/user/info", System.Net.HttpStatusCode.Unauthorized, "{}");
        var api = new HevyAPI(new HttpClient(handler));

        var result = await api.AuthoriseHevy("bad-key");

        Assert.That(result, Is.False);
        Assert.That(api.IsAuthorised, Is.False);
    }

    [Test]
    public async Task AuthoriseHevy_SetsApiKeyHeader()
    {
        var handler = new MockHttpMessageHandler();
        handler.SetOkResponse("/v1/user/info", """{"username":"test"}""");
        var api = new HevyAPI(new HttpClient(handler));

        await api.AuthoriseHevy("my-secret-key");

        var request = handler.LastRequest;
        Assert.That(request, Is.Not.Null);
        Assert.That(request!.Headers.Contains("api-key"), Is.True);
        Assert.That(request.Headers.GetValues("api-key").First(), Is.EqualTo("my-secret-key"));
    }

    [Test]
    public void MethodsThrow_WhenNotAuthorised()
    {
        var api = new HevyAPI(new HttpClient(new MockHttpMessageHandler()));

        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetUserInfo());
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetWorkouts());
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetWorkout("id"));
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetWorkoutCount());
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetRoutines());
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetRoutineFolders());
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetExerciseTemplates());
        Assert.ThrowsAsync<InvalidOperationException>(() => api.GetExerciseHistory("id"));
    }
}
