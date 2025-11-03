using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WPF_LoginForm.Views;

namespace WPF_LoginForm
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected void ApplicationStart(object sender, StartupEventArgs e)
        {
            var loginView = new LoginView();
            loginView.Show();
            loginView.IsVisibleChanged += (s, ev) =>
              {
                  if (loginView.IsVisible == false && loginView.IsLoaded)
                  {
                      System.Diagnostics.Debug.WriteLine("[App] LoginView hidden, creating MainView");
                      System.Diagnostics.Debug.WriteLine($"[App] Thread.CurrentPrincipal.Identity.Name: '{System.Threading.Thread.CurrentPrincipal?.Identity?.Name}'");
                      
                      var mainView = new MainView();
                      mainView.Show();
                      loginView.Close();
                  }
              };
        }
    }
}
