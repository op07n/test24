using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace N2N_Desktop
{
    class ShowMessageCommand : ICommand
    {
        public void Execute(object parameter)
        {
            MainWindow.mainw.Visibility = Visibility.Visible;
            MainWindow.mainw.WindowState = WindowState.Normal;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
}
