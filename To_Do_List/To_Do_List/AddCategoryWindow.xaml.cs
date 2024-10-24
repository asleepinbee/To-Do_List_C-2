using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace To_Do_List
{
    public partial class AddCategoryWindow : Window
    {
        public string CategoryName { get; private set; }

        public AddCategoryWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(CategoryNameTextBox.Text))
            {
                // Если поле пустое, показываем предупреждение
                ShowValidationError("Category name cannot be empty.");
                return;
            }
            CategoryName = CategoryNameTextBox.Text;
            DialogResult = true;
            this.Close();
        }

        private void ShowValidationError(string message)
        {
            var errorWindow = new ValidationErrorWindow(message);
            errorWindow.ShowDialog();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            this.Close();
        }
    }

}
