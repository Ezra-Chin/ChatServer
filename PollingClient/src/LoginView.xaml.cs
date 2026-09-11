using System;
using System.Windows;
using System.Windows.Controls;

namespace PollingClient.src
{
    public partial class LoginView : Page
    {
        ChatContract.IPollingChatService foob;
        
        public LoginView(ChatContract.IPollingChatService chatContract)
        {
            InitializeComponent();
            foob = chatContract;
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
                bool result = foob.SignIn(userId);
                if (!result)
                {
                    MessageBox.Show("There's already another session with that user ID ");
                }
                else
                {
                    NavigationService.Navigate(new ChannelListPage(userId, foob));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(), 
                    "An error occurred while signing in."
                    ); 
                
                return;
            }
        }
    }
}