using System.Windows;
using System.Windows.Controls;

namespace Day15
{
    public partial class EditDialog : Window
    {
        public string FullName { get; private set; }
        public string Position { get; private set; }

        public EditDialog()
        {
            InitializeComponent();
        }

        public EditDialog(string fullName, string position)
        {
            InitializeComponent();
            txtFullName.Text = fullName;
            cbPosition.Text = position;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Введите ФИО");
                return;
            }

            var selected = cbPosition.SelectedItem as ComboBoxItem;
            if (selected == null)
            {
                MessageBox.Show("Выберите должность");
                return;
            }

            FullName = txtFullName.Text.Trim();
            Position = selected.Content.ToString();
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}