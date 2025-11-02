using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPF_LoginForm.Models;
using WPF_LoginForm.Repositories;
using WPF_LoginForm.Views;

namespace WPF_LoginForm.ViewModels
{
    public class AgregarViewModel : ViewModelBase
    {
        private int selectedIndex;
        private int id;
        private string _Name;
        private String _LastName;
        private string _errorMessage;
        private string document;
        private string address;
        private string gender;
        private bool _isViewVisible = true;

        private ICommand newClientCommand;
        private ICommand addClientCommand;
        private ICommand delClientCommand;
        private ICommand updateClientCommand;


        private IUserRepository userRepository;
        public AgregarViewModel()
        {
            userRepository = new UserRepository();

        }
        //region Propiedades
    }   
}
