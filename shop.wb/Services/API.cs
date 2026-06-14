using System.Net.Http.Headers;

namespace shop.wb.Services
{
    public class API
    {
        private readonly HttpClient http;
        private readonly IHttpContextAccessor context;

        public API(IHttpClientFactory factory, IHttpContextAccessor context)
        {
            this.http = factory.CreateClient("API");//localhost 7168
            this.context = context; //поле
        }

        public async Task<T?> GetAsync<T>(string url)
        {
            AddAuthHeader();
            return await this.http.GetFromJsonAsync<T>(url);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            AddAuthHeader();
            return await this.http.PostAsJsonAsync(url, data);
        }


        private void AddAuthHeader()
        {
            var token = this.context.HttpContext?.Session.GetString("jwt");//заголовк jwt
            if (!string.IsNullOrEmpty(token))
            {
                this.http.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", token);
            }
        }
    }
}
