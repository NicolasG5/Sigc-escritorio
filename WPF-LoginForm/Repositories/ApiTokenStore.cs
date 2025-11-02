namespace WPF_LoginForm.Repositories
{
    /// <summary>
    /// Singleton para almacenar el token de la API en memoria durante la sesión.
    /// </summary>
    public sealed class ApiTokenStore
    {
        private static readonly ApiTokenStore _instance = new ApiTokenStore();
        private string _token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE3NjIyNjAwNjAsInN1YiI6IjEifQ.-zIMOwoRPQVHajTwUK8CFpRmk8JBcgNat2j5Sgi08vk";

        private ApiTokenStore() { }

        public static ApiTokenStore Instance => _instance;

        public string Token
        {
            get => _token;
            set => _token = value;
        }

        public void Clear()
        {
            _token = null;
        }
    }
}
