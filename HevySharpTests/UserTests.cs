using HevySharp;

namespace HevySharpTests;

public class UserTests
{
    private MockHttpMessageHandler handler = null!;
    private HevyAPI api = null!;

    [TearDown]
    public void TearDown() => handler?.Dispose();

    [SetUp]
    public async Task SetUp()
    {
        handler = new MockHttpMessageHandler();
        handler.SetOkResponse("/v1/user/info", """
        {
            "username": "iron_lifter",
            "created_at": "2022-01-15T00:00:00Z",
            "weight_unit": "kg",
            "distance_unit": "km"
        }
        """);
        api = new HevyAPI(new HttpClient(handler));
        await api.AuthoriseHevy("key");
    }

    [Test]
    public async Task GetUserInfo_DeserializesCorrectly()
    {
        var user = await api.GetUserInfo();

        Assert.That(user, Is.Not.Null);
        Assert.That(user!.Username, Is.EqualTo("iron_lifter"));
        Assert.That(user.CreatedAt, Is.EqualTo("2022-01-15T00:00:00Z"));
        Assert.That(user.WeightUnit, Is.EqualTo("kg"));
        Assert.That(user.DistanceUnit, Is.EqualTo("km"));
    }
}
