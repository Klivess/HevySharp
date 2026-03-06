namespace HevySharp
{
    public class HevyAPI
    {
        private string baseURL = "https://api.hevyapp.com";
        private string apiKey = "";
        private HttpClient client;
        private bool IsAuthorised = false;
        public HevyAPI()
        {
            client = new();
        }

        public async Task<bool> AuthoriseHevy(string apiKey)
        {
            client.DefaultRequestHeaders.Add("api-key", apiKey);
            var response = await client.GetAsync(baseURL + "/v1/user/info");
            if (response.IsSuccessStatusCode)
            {
                IsAuthorised = true;
                return true;
            }
            return false;
        }

    }
}
