using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF_LoginForm.Models
{
   public class UserAccountModel
    {
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public byte[] ProfilePicture { get; set; }
        public string Password { get; set; } // Added Password property
        public bool Estado { get; set; } // Added Estado property

        // Added Autenticar method
        public bool Autenticar(string password)
        {
            // Here you would add your own logic to check if the password is correct
            return this.Password == password;
        }
    }
}
