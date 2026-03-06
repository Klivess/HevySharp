using HevySharp;

namespace HevySharpTests
{
    public class Authentication
    {
        [SetUp]
        public void Setup()
        {
            Assert.That(string.IsNullOrEmpty(Environment.GetEnvironmentVariable("HEVYAPIKEY")) == false);
        }

        [Test]
        public void LoginToHevy()
        {
            HevyAPI api = new HevyAPI();
            string apiKey = Environment.GetEnvironmentVariable("HEVYAPIKEY");
            Assert.That((api.AuthoriseHevy(apiKey).Result == true));
        }
    }
}
