using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Net;
using System.Net.Http.Headers;
using static System.Net.WebRequestMethods;

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
            await this.context.HttpContext!.Session.LoadAsync();
            //var token = this.context.HttpContext.Session.GetString("jwt");

            //Console.WriteLine($"=== GetAsync: token is {(token == null ? "NULL" : "present, length=" + token.Length)}");
            //Console.WriteLine($"=== Auth header: {this.http.DefaultRequestHeaders.Authorization?.ToString() ?? "NOT SET"}");

            AddAuthHeader();

           // Console.WriteLine($"=== Auth header AFTER: {this.http.DefaultRequestHeaders.Authorization?.ToString() ?? "NOT SET"}");

            return await this.http.GetFromJsonAsync<T>(url);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T data)
        {
            AddAuthHeader();
            var response = await this.http.PostAsJsonAsync(url, data);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await this.context.HttpContext!.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                this.context.HttpContext!.Session.Clear();
            }

            return response;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            AddAuthHeader();
            return await this.http.DeleteAsync(url);
        }
        private void AddAuthHeader()
        {
            var token = this.context.HttpContext?.Session.GetString("jwt");//заголовк jwt

            if (!string.IsNullOrEmpty(token))

            {

                this.http.DefaultRequestHeaders.Authorization = null;

                this.http.DefaultRequestHeaders.Authorization =

                    new AuthenticationHeaderValue("Bearer", token);

            }

        }
        /////доделай log out  и истичние cookee
        //// User.Identity?.IsAuthenticated == true warn!!!
    }
}


