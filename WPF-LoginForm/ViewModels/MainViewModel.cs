using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;
using WPF_LoginForm.Views;
using System.Security.Principal;

namespace WPF_LoginForm.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        //Fields
        private UserAccountModel _currentUserAccount;
        private ViewModelBase _currentChildView;
        private string _caption;
        private IconChar _icon;

        private IUserRepository userRepository;

        //Properties
        public UserAccountModel CurrentUserAccount
        {
            get
            {
                return _currentUserAccount;
            }

            set
            {
                _currentUserAccount = value;
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }
        public ViewModelBase CurrentChildView
        {
            get
            {
                return CurrentChildView1;
            }
            set
            {
                CurrentChildView1 = value;
                OnPropertyChanged(nameof(CurrentChildView));
            }
        }
        public string Caption
        {
            get
            {
                return Caption1;
            }
            set
            {
                Caption1 = value;
                OnPropertyChanged(nameof(Caption));
            }
        }
        public IconChar Icon
        {
            get
            {
                return Icon1;
            }
            set
            {
                Icon1 = value;
                OnPropertyChanged(nameof(Icon));
            }
        }



        //--> Commands
        public ICommand ShowHomeViewCommand { get; }
        
        public ICommand ShowCustomerCommand { get; }

        public ICommand ShowCustomerView2Command { get; }

        public ICommand ShowCustomerView3Command { get; }

        public ICommand ShowCustomerView4Command { get; }

        public ICommand ShowCustomerView5Command { get; }
        public ICommand ShowAtencionCommand { get; }
        public ICommand ShowSeguimientoCommand { get; }
        public ICommand ShowCitaCommand { get; }
        public ICommand ShowTratamientoCommand { get; }
        public ICommand ShowMedicacionCommand { get; }
        public ICommand ShowReportesCommand { get; }
        public ICommand ShowControlSolicitudesCommand { get; }
        public ICommand ShowGestionUsuariosCommand { get; } // NUEVO COMANDO


        public ICommand ShowAgregarViewCommand { get; }

        public ICommand LogoutCommand { get; }






        public ViewModelBase CurrentChildView1 { get => _currentChildView; set => _currentChildView = value; }
        public string Caption1 { get => _caption; set => _caption = value; }
        public IconChar Icon1 { get => _icon; set => _icon = value; }

        public MainViewModel()
        {
            userRepository = new UserRepository();
            CurrentUserAccount = new UserAccountModel();
            
            System.Diagnostics.Debug.WriteLine($"[MainViewModel Constructor] Initialized");
            
            //Initialize commands
            ShowHomeViewCommand = new ViewModelCommand(ExecuteShowHomeViewCommand);

            ShowCustomerCommand = new ViewModelCommand(ExecuteShowCustomerCommand);


            ShowCustomerView2Command = new ViewModelCommand(ExecuteShowCustomerView2Command);
            ShowCitaCommand = new ViewModelCommand(ExecuteShowCitaCommand);
            ShowCustomerView3Command = new ViewModelCommand(ExecuteShowCustomerView3Command);
            ShowControlSolicitudesCommand = new ViewModelCommand(ExecuteShowControlSolicitudesCommand);
            ShowAtencionCommand = new ViewModelCommand(ExecuteShowAtencionCommand);
            ShowSeguimientoCommand = new ViewModelCommand(ExecuteShowSeguimientoCommand);
            ShowTratamientoCommand = new ViewModelCommand(ExecuteShowTratamientoCommand);
            ShowMedicacionCommand = new ViewModelCommand(ExecuteShowMedicacionCommand);
            ShowReportesCommand = new ViewModelCommand(ExecuteShowReportesCommand);
            ShowGestionUsuariosCommand = new ViewModelCommand(ExecuteShowGestionUsuariosCommand); // NUEVO


            ShowCustomerView4Command = new ViewModelCommand(ExecuteShowCustomerView4Command);
            ShowCustomerView5Command = new ViewModelCommand(ExecuteShowCustomerView5Command);

            ShowAgregarViewCommand = new ViewModelCommand(ExecuteShowShowAgregarViewCommandCommand);

            LogoutCommand = new ViewModelCommand(ExecuteLogoutCommand);

        



            //Default view
            ExecuteShowHomeViewCommand(null);
            LoadCurrentUserData();
        }

        private void ExecuteShowReportesCommand(object obj)
        {
            CurrentChildView = new ReportesG();
            Caption = "Seguimiento";
            Icon = IconChar.Hammer;
        }

        // NUEVO: Metodo para mostrar Gestion de Usuarios
        private void ExecuteShowGestionUsuariosCommand(object obj)
        {
            CurrentChildView = new ViewModels.GestionUsurarios();
            Caption = "Gestion de Usuarios";
            Icon = IconChar.Users;
        }

        private void ExecuteShowMedicacionCommand(object obj)
        {
            CurrentChildView = new MedicacioG();
            Caption = "Seguimiento";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowTratamientoCommand(object obj)
        {
            CurrentChildView = new TratamientoG();
            Caption = "Seguimiento";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowSeguimientoCommand(object obj)
        {
            CurrentChildView = new Seguimiento();
            Caption = "Seguimiento";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowCitaCommand(object obj)
        {
            CurrentChildView = new Cita();
            Caption = "Cita";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowAtencionCommand(object obj)
        {
            
            CurrentChildView = new Atencion();
            Caption = "Atencion";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowShowAgregarViewCommandCommand(object obj)
        {
            CurrentChildView = new AgregarViewModel();
            Caption = "Inspeccion";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowControlSolicitudesCommand(object obj)
        {
            CurrentChildView = new ControlSolicitudes();
            Caption = "Gestion de solicitud";
            Icon = IconChar.Envelope;
        }


        private void ExecuteShowCustomerCommand(object obj)
        {
            CurrentChildView = new Customer();
            Caption = "Solicitud";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowCustomerView5Command(object obj)
        {
            CurrentChildView = new CustomerView5();
            Caption = "Customers5";
            Icon = IconChar.Hammer;
        }


        private void ExecuteShowCustomerView4Command(object obj)
        {
            CurrentChildView = new CustomerView4();
            Caption = "Customers4";
            Icon = IconChar.Hammer;
        }

        private void ExecuteShowCustomerView3Command(object obj)
        {
            CurrentChildView = new CustomerView3();
            Caption = "Customers3";
            Icon = IconChar.IceCream;
        }

        private void ExecuteShowCustomerView2Command(object obj)
        {
            CurrentChildView = new CustomerView2();
            Caption = "Customers2";
            Icon = IconChar.ArrowDownUpAcrossLine;


        }

        //private void ExecuteShowCustomerCommand(object obj)
        //{
        //    CurrentChildView = new CustomerViewModel();
        //    Caption = "Customers";
        //    Icon = IconChar.UserGroup;
           
        //}

        private void ExecuteShowHomeViewCommand(object obj)
        {
            CurrentChildView = new HomeViewModel();
            Caption = "Dashboard";
            Icon = IconChar.Home;
        }
       

        private void ExecuteLogoutCommand(object obj)
        {
            // reset principal
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(string.Empty), null);

            // show a fresh login window that reuses the same transition logic
            var loginView = new LoginView();
            loginView.IsVisibleChanged += (s, ev) =>
            {
                if (loginView.IsVisible == false && loginView.IsLoaded)
                {
                    var mainView = new MainView();
                    mainView.Show();
                    loginView.Close();
                }
            };
            loginView.Show();

            // close current main window
            var currentMain = Application.Current.Windows.OfType<MainView>().FirstOrDefault();
            if (currentMain != null)
            {
                currentMain.Close();
            }
        }
        
        private async void LoadCurrentUserData()
        {
            try
            {
                var username = Thread.CurrentPrincipal?.Identity?.Name;
                
                System.Diagnostics.Debug.WriteLine($"[LoadCurrentUserData] Username from Principal: '{username}'");
                
                if (string.IsNullOrEmpty(username))
                {
                    CurrentUserAccount.DisplayName = "Usuario no identificado";
                    CurrentUserAccount.Username = "";
                    System.Diagnostics.Debug.WriteLine("[LoadCurrentUserData] Username is null or empty");
                    OnPropertyChanged(nameof(CurrentUserAccount));
                    return;
                }

                System.Diagnostics.Debug.WriteLine($"[LoadCurrentUserData] Calling GetByUsernameAsync for: {username}");
                var user = await userRepository.GetByUsernameAsync(username);
                
                if (user != null)
                {
                    System.Diagnostics.Debug.WriteLine($"[LoadCurrentUserData] User found: {user.Name} {user.LastName}");
                    CurrentUserAccount.Username = user.Username;
                    CurrentUserAccount.DisplayName = $"{user.Name} {user.LastName}";
                    CurrentUserAccount.ProfilePicture = null;
                    
                    System.Diagnostics.Debug.WriteLine($"[LoadCurrentUserData] DisplayName set to: {CurrentUserAccount.DisplayName}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("[LoadCurrentUserData] User not found from API");
                    CurrentUserAccount.DisplayName = "Usuario no encontrado";
                    CurrentUserAccount.Username = username;
                }
                
                // Forzar actualización de la UI
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[LoadCurrentUserData] Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"[LoadCurrentUserData] StackTrace: {ex.StackTrace}");
                CurrentUserAccount.DisplayName = "Error al cargar usuario";
                CurrentUserAccount.Username = Thread.CurrentPrincipal?.Identity?.Name ?? "";
                OnPropertyChanged(nameof(CurrentUserAccount));
            }
        }

        
    }
    
}
