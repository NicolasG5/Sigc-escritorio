using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WPF_LoginForm.Services
{
    public class ApiServiceBase
    {
        protected readonly HttpClient _httpClient;
        protected readonly string _baseUrl;
        protected readonly string _jwtToken;

        public ApiServiceBase()
        {
            _baseUrl = ConfigurationManager.AppSettings["ApiBaseUrl"];
            _jwtToken = ConfigurationManager.AppSettings["ApiJwtToken"];

            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(_baseUrl);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _jwtToken);
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        // Método helper para manejar errores
        protected void HandleApiError(Exception ex, string operation)
        {
            System.Diagnostics.Debug.WriteLine($"Error en API {operation}: {ex.Message}");
            // Aquí podrías agregar logging más avanzado
        }
    }
}