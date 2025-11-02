using Newtonsoft.Json;

namespace WPF_LoginForm.Models
{
    public class LoginResponse
    {
        [JsonProperty("access_token")]
        public string Token { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        public string Username { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
    }
}
