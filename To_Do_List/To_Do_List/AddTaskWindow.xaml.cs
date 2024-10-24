using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using To_Do_List.Models;

namespace To_Do_List
{
    public partial class AddTaskWindow : Window
    {
        public string TaskTitle { get; private set; }
        public string TaskDescription { get; private set; }
        public PriorityLevel TaskPriority { get; private set; }

        public AddTaskWindow()
        {
            InitializeComponent();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {

            if (string.IsNullOrWhiteSpace(TaskTitleTextBox.Text))
            {
                ShowValidationError("Task title cannot be empty.");
                return;
            }

            if (string.IsNullOrWhiteSpace(TaskDescriptionTextBox.Text))
            {
                ShowValidationError("Task description cannot be empty.");
                return;
            }

            TaskTitle = TaskTitleTextBox.Text;
            TaskDescription = TaskDescriptionTextBox.Text;

            // Преобразуем выбранный элемент ComboBox в PriorityLevel
            switch (PriorityComboBox.SelectedIndex)
            {
                case 0:
                    TaskPriority = PriorityLevel.Low;
                    break;
                case 1:
                    TaskPriority = PriorityLevel.Medium;
                    break;
                case 2:
                    TaskPriority = PriorityLevel.High;
                    break;
            }

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
