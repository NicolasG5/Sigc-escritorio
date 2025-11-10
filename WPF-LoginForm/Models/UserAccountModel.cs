using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoginForm.Models
{
   public class UserAccountModel : INotifyPropertyChanged
    {
        private string _username;
        private string _displayName;
        private byte[] _profilePicture;
        private string _password;
        private bool _estado;

        public string Username 
        { 
            get => _username; 
            set 
            { 
                _username = value; 
                OnPropertyChanged(nameof(Username)); 
            } 
        }

        public string DisplayName 
        { 
            get => _displayName; 
            set 
            { 
                _displayName = value; 
                OnPropertyChanged(nameof(DisplayName)); 
            } 
        }

        public byte[] ProfilePicture 
        { 
            get => _profilePicture; 
            set 
            { 
                _profilePicture = value; 
                OnPropertyChanged(nameof(ProfilePicture)); 
            } 
        }

        public string Password 
        { 
            get => _password; 
            set 
            { 
                _password = value; 
                OnPropertyChanged(nameof(Password)); 
            } 
        }

        public bool Estado 
        { 
            get => _estado; 
            set 
            { 
                _estado = value; 
                OnPropertyChanged(nameof(Estado)); 
            } 
        }

        public bool Autenticar(string password)
        {
            return this.Password == password;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
