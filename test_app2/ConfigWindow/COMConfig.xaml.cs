using System;
using System.Windows;
using test_app2.ViewModels;
using test_app2.Config;
using TheRFramework.Utilities;

namespace test_app2.Config
{
    public partial class ConfigWindow : Window
    {
        public ConfigWindow()
        {
            InitializeComponent();
        }
        private void ConfigWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}
