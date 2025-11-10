using System;
using System.Collections.ObjectModel;
using System.Security;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;

namespace WPF_LoginForm.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        public ObservableCollection<UserModel> Usuarios { get; set; } = new ObservableCollection<UserModel>();

        private string _username;
        private SecureString _password;
        private string _errorMessage;
        private bool _isViewVisible = true;
        private IUserRepository userRepository;

        public string Email
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Email));
            }
        }

        public SecureString Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }

        public bool IsViewVisible
        {
            get => _isViewVisible;
            set
            {
                _isViewVisible = value;
                OnPropertyChanged(nameof(IsViewVisible));
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RecoverPasswordCommand { get; }
        public ICommand ShowPasswordCommand { get; }
        public ICommand RememberPasswordCommand { get; }

        public LoginViewModel()
        {
            userRepository = new UserRepository(); // Usa la implementación que consume la API
            LoginCommand = new ViewModelCommand(ExecuteLoginCommand, CanExecuteLoginCommand);
            RecoverPasswordCommand = new ViewModelCommand(p => ExecuteRecoverPassCommand("", ""));
        }

        private bool CanExecuteLoginCommand(object obj)
        {
            return !string.IsNullOrWhiteSpace(Email) && Email.Length >= 3 && Password != null && Password.Length >= 3;
        }

        private async void ExecuteLoginCommand(object obj)
        {
            try
            {
                var plainPassword = new System.Net.NetworkCredential(string.Empty, Password).Password;
                System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Attempting login for: {Email}");

                var loginResponse = await userRepository.AuthenticateUserAsync(Email, plainPassword);

                if (loginResponse != null && loginResponse.Success && !string.IsNullOrEmpty(loginResponse.Token))
                {
                    System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Login successful. Token: {loginResponse.Token?.Substring(0, 20)}...");
                    System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Username from response: {loginResponse.Username}");

                    Thread.CurrentPrincipal = new System.Security.Principal.GenericPrincipal(
                        new System.Security.Principal.GenericIdentity(loginResponse.Username), null);

                    System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Thread.CurrentPrincipal.Identity.Name: {Thread.CurrentPrincipal.Identity.Name}");

                    ApiTokenStore.Instance.Token = loginResponse.Token;

                    // Obtener usuarios tras login exitoso
                    var usuarios = await userRepository.GetAllAsync();
                    Usuarios.Clear();
                    foreach (var usuario in usuarios)
                        Usuarios.Add(usuario);

                    System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Setting IsViewVisible to false");
                    IsViewVisible = false;
                    return;
                }
                ErrorMessage = loginResponse?.ErrorMessage ?? "Email o contraseña incorrectos, o usuario inactivo.";
                System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Login failed: {ErrorMessage}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[ExecuteLoginCommand] Exception: {ex.Message}");
                ErrorMessage = $"Error de conexión: {ex.Message}";
            }
        }

        private void ExecuteRecoverPassCommand(string username, string email)
        {
            throw new NotImplementedException();
        }
    }
}
