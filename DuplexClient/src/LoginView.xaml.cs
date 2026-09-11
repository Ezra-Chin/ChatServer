using System;
using System.Windows;
using System.Windows.Controls;

namespace DuplexClient.src
{
    public partial class LoginView : Page
    {
        ChatContract.IChatService foob;
        ChatCallbackImpl callback;

        public LoginView(ChatContract.IChatService chatContract, ChatCallbackImpl callback)
        {
            InitializeComponent();
            foob = chatContract;
            this.callback = callback; 
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
                    NavigationService.Navigate(new ChannelListPage(userId, foob, callback));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    ex.ToString(), 
                    "An error occurred while signing in."); 
                    
                return;
            }
        }
    }
}
