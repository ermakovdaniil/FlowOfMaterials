﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using PlenkaAPI.Data;

using PlenkaWpf.VM;

using MessageBox = HandyControl.Controls.MessageBox;


namespace PlenkaWpf.View
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginControl : UserControl, IСhangeableControl
    {
        public LoginControl()
        {
            InitializeComponent();
        }

        public WindowState PreferedWindowState { get; set; } = WindowState.Normal;
        public string WindowTitle { get; set; } = "Авторизация";
        public double? PreferedHeight { get; set; } = 390;
        public double? PreferedWidth { get; set; } = 270;
        public event IСhangeableControl.ChangingRequestHandler ChangingRequest;

        public void OnChangingRequest(UserControl newControl)
        {
            ChangingRequest?.Invoke(this, newControl);
        }


        private void Button_Click(object sender, RoutedEventArgs e) //todo убрать заглушку
        {
            var con = DbContextSingleton.GetInstance();
            var userName = UserNameTextbox.Text;
            var password = PasswordTextBox.Password;

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Введите имя пользователя и пароль!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                var user = con.Users.First(u => u.UserName == userName && u.UserPassword == password);
                if (user.UserType.UserTypeName == "Администратор")
                {
                    OnChangingRequest(new MainAdminControl());
                }

                if (user.UserType.UserTypeName == "Исследователь")
                {
                    OnChangingRequest(new ResearcherControl());
                }
            }
            catch (Exception exception){

                MessageBox.Show("Неверное имя пользователя или пароль! Повторите попытку.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            OnChangingRequest(new ResearcherControl());
        }
    }
}
