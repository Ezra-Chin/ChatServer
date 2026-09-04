using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using ServerInterface;

namespace PollingClient.src
{
    public partial class LoginView : UserControl 
    {
        ServerInterface.ServerInterface server= new ServerInterface.ServerInterface();
        public LoginView()
        {
            InitializeComponent();

        }

        private void SignIn_Click(object sender, RoutedEventArgs e) 
        {
            string userId = UserIdTextBox.Text.Trim();

            if (string.IsNullOrEmpty(userId))
            {
                MessageBox.Show("Please enter a valid user ID.");
                return;
            }

            try
            {
                serverInter
            } catch (Exception ex) {
                MessageBox.Show("An error occurred while signing in."); return;
            }


        }
    }
}
