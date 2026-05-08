using System.Windows;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Views
{
    public partial class EmployeeEditWindow : Window
    {
        public EmployeeEditWindow(EmployeeEditViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            viewModel.OnSave = employee =>
            {
                DialogResult = true;
                Close();
            };

            viewModel.OnCancel = () =>
            {
                DialogResult = false;
                Close();
            };
        }
    }
}