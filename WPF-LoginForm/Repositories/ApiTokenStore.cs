namespace WPF_LoginForm.Repositories
{
    /// <summary>
    /// Singleton para almacenar el token de la API en memoria durante la sesión.
    /// </summary>
    public sealed class ApiTokenStore
    {
        private static readonly ApiTokenStore _instance = new ApiTokenStore();
        private string _token = null; // No hardcodear tokens por seguridad

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
