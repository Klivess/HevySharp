using HevySharp;
using HevySharp.Schemas;

namespace HevySharpTests;

public class RoutineFolderTests
{
    private MockHttpMessageHandler handler = null!;
    private HevyAPI api = null!;

    [TearDown]
    public void TearDown() => handler?.Dispose();

    [SetUp]
    public async Task SetUp()
    {
        handler = new MockHttpMessageHandler();
        handler.SetOkResponse("/v1/user/info", """{"username":"test"}""");
        api = new HevyAPI(new HttpClient(handler));
        await api.AuthoriseHevy("key");
    }

    [Test]
    public async Task GetRoutineFolders_DeserializesPaginatedResponse()
    {
        handler.SetOkResponse("/v1/routine_folders?page=", """
        {
            "page": 1,
            "page_count": 1,
            "routine_folders": [
                { "id": "fld_001", "title": "Powerlifting Programs" }
            ]
        }
        """);

        var result = await api.GetRoutineFolders();

        Assert.That(result, Is.Not.Null);
        Assert.That(result!.RoutineFolders, Has.Count.EqualTo(1));
        Assert.That(result.RoutineFolders![0].Id, Is.EqualTo("fld_001"));
        Assert.That(result.RoutineFolders[0].Title, Is.EqualTo("Powerlifting Programs"));
    }

    [Test]
    public async Task GetRoutineFolder_DeserializesFolder()
    {
        handler.SetOkResponse("/v1/routine_folders/fld_001", """
        { "id": "fld_001", "title": "Powerlifting Programs" }
        """);

        var folder = await api.GetRoutineFolder("fld_001");

        Assert.That(folder, Is.Not.Null);
        Assert.That(folder!.Id, Is.EqualTo("fld_001"));
        Assert.That(folder.Title, Is.EqualTo("Powerlifting Programs"));
    }

    [Test]
    public async Task CreateRoutineFolder_WrapsBodyInRoutineFolderKey()
    {
        handler.SetOkResponse("/v1/routine_folders", """
        { "id": "fld_002", "title": "Hypertrophy" }
        """);

        var folder = new HevyRoutineFolder { Title = "Hypertrophy" };

        await api.CreateRoutineFolder(folder);

        var body = await handler.GetLastRequestBody();
        Assert.That(body, Is.Not.Null);
        Assert.That(body, Does.Contain("\"routine_folder\""));
        Assert.That(body, Does.Contain("\"title\""));
    }
}
