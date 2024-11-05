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
        public DateTime? TaskDeadline { get; private set; }  // Свойство для хранения дэдлайна
        public bool IsReadOnly { get; set; }
        public bool IsPriorityEnabled => !IsReadOnly; // Управление доступностью ComboBox
        public Visibility AddButtonVisibility => IsReadOnly ? Visibility.Collapsed : Visibility.Visible;

        /*public AddTaskWindow()
        {
            InitializeComponent();
        }
        */

        public AddTaskWindow(TaskItem task = null, bool isReadOnly = false)
        {
            InitializeComponent();
            DataContext = this;

            if (task != null)
            {
                // Если мы редактируем задачу, заполняем поля
                TaskTitleTextBox.Text = task.Title;
                TaskDescriptionTextBox.Text = task.Description;
                PriorityComboBox.SelectedIndex = (int)task.Priority;

                if (task.Deadline.HasValue)
                {
                    DeadlineDatePicker.SelectedDate = task.Deadline.Value.Date;
                    DeadlineTimeTextBox.Text = task.Deadline.Value.ToString("HH:mm");
                }

                if (isReadOnly)
                {
                    // Блокируем элементы управления, если окно используется только для чтения
                    TaskTitleTextBox.IsReadOnly = true;
                    TaskDescriptionTextBox.IsReadOnly = true;
                    PriorityComboBox.IsEnabled = false;
                    DeadlineDatePicker.IsEnabled = false;
                    DeadlineTimeTextBox.IsReadOnly = true;
                    AddButton.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Проверка на заполненность обязательных полей
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

            // Обработка дэдлайна
            if (DeadlineDatePicker.SelectedDate.HasValue)
            {
                DateTime date = DeadlineDatePicker.SelectedDate.Value;
                TimeSpan time;
                if (TimeSpan.TryParse(DeadlineTimeTextBox.Text, out time))
                {
                    TaskDeadline = date + time;
                }
                else
                {
                    ShowValidationError("Invalid time format for deadline. Use HH:mm.");
                    return;
                }
            }
            else
            {
                TaskDeadline = null; // Дэдлайн не установлен
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
