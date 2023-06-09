﻿using System.Collections.ObjectModel;
using System.Windows;

using PlenkaAPI.Data;
using PlenkaAPI.Models;

using PlenkaWpf.Utils;
using PlenkaWpf.View;

using MessageBox = HandyControl.Controls.MessageBox;


namespace PlenkaWpf.VM
{
    internal class UserExplorerControlVM : ViewModelBase

    {
    #region Functions

    #region Constructors

        public UserExplorerControlVM()
        {
            _db = DbContextSingleton.GetInstance();
            Users = _db.Users.Local.ToObservableCollection();
            UserTypes = _db.UserTypes.Local.ToObservableCollection();
        }

    #endregion

    #endregion


    #region Properties

        private readonly MembraneContext _db;
        public User SelectedUser { get; set; }
        public ObservableCollection<User> Users { get; set; }
        public ObservableCollection<UserType> UserTypes { get; set; }

    #endregion


    #region Commands

        private RelayCommand _addNewUser;

        /// <summary>
        ///     Команда, открывающая окно создания пользователя
        /// </summary>
        public RelayCommand AddNewUser
        {
            get
            {
                return _addNewUser ??= new RelayCommand(o =>
                {
                    ShowChildWindow(new UserEditWindow(new User()));
                });
            }
        }

        private RelayCommand _editUser;

        /// <summary>
        ///     Команда, открывающая окно редактирования пользователя
        /// </summary>
        public RelayCommand EditUser
        {
            get
            {
                return _editUser ??= new RelayCommand(o =>
                {
                    ShowChildWindow(new UserEditWindow(SelectedUser));
                }, _ => SelectedUser != null);
            }
        }

        private RelayCommand _deleteUser;

        /// <summary>
        ///     Команда, удаляющая пользователя
        /// </summary>
        public RelayCommand DeleteUser
        {
            get
            {
                return _deleteUser ??= new RelayCommand(o =>
                {
                    if (MessageBox.Show($"Вы действительно хотите удалить пользователя {SelectedUser.UserName}?",
                                        "Удаление пользователя", MessageBoxButton.YesNo, MessageBoxImage.Warning) ==
                        MessageBoxResult.Yes)
                    {
                        _db.Users.Remove(SelectedUser);
                        _db.SaveChanges();
                    }
                }, _ => SelectedUser != null);
            }
        }

    #endregion
    }
}
