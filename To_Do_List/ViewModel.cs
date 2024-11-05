using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using To_Do_List.Models;

namespace To_Do_List
{
    public class ViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;

        public ObservableCollection<Category> Categories { get; set; }
        public ObservableCollection<TaskItem> Tasks { get; set; }

        private Category _selectedCategory;
        public Category SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                _selectedCategory = value;
                OnPropertyChanged(nameof(SelectedCategory));
                LoadTasksForCategory();
            }
        }

        private TaskItem _selectedTask;
        public TaskItem SelectedTask
        {
            get => _selectedTask;
            set
            {
                _selectedTask = value;
                OnPropertyChanged(nameof(SelectedTask));
            }
        }

        public ICommand AddCategoryCommand { get; set; }
        public ICommand AddTaskCommand { get; set; }
        public ICommand CompleteTaskCommand { get; set; }
        public ICommand DeleteCategoryCommand { get; set; }
        public ICommand DeleteTaskCommand { get; set; }
        public ICommand EditCategoryCommand { get; set; }
        public ICommand EditTaskCommand { get; set; }
        public ICommand OpenTaskCommand { get; set; }

        public ViewModel()
        {
            _databaseService = new DatabaseService();
            Categories = new ObservableCollection<Category>(_databaseService.GetCategories());
            Tasks = new ObservableCollection<TaskItem>();

            AddCategoryCommand = new RelayCommand(AddCategory);
            AddTaskCommand = new RelayCommand(AddTask);
            CompleteTaskCommand = new RelayCommand<TaskItem>(CompleteTask); // Передаем задачу как параметр
            DeleteCategoryCommand = new RelayCommand(DeleteCategory);
            DeleteTaskCommand = new RelayCommand(DeleteTask);
            EditCategoryCommand = new RelayCommand(EditCategory);
            EditTaskCommand = new RelayCommand(EditTask);
            OpenTaskCommand = new RelayCommand<TaskItem>(OpenTask); // Добавляем команду "Открыть"
        }

        private void OpenTask(TaskItem task)
        {
            if (task != null)
            {
                // Открываем окно для просмотра задачи с кнопкой "Отмена"
                var viewTaskWindow = new AddTaskWindow(task, isReadOnly: true);
                viewTaskWindow.ShowDialog();
            }
        }

        private void LoadTasksForCategory()
        {
            if (SelectedCategory != null)
            {
                Tasks.Clear();
                var tasks = _databaseService.GetTasksForCategory(SelectedCategory.Id);
                foreach (var task in tasks)
                {
                    Tasks.Add(task);
                }
            }
        }

        private void AddCategory()
        {
            var addCategoryWindow = new AddCategoryWindow();
            if (addCategoryWindow.ShowDialog() == true)
            {
                var newCategory = new Category
                {
                    Name = addCategoryWindow.CategoryName
                };

                _databaseService.AddCategory(newCategory);
                Categories.Clear();
                foreach (var category in _databaseService.GetCategories())
                {
                    Categories.Add(category);
                }
            }
        }

        private void AddTask()
        {
            if (SelectedCategory != null)
            {
                var addTaskWindow = new AddTaskWindow();
                if (addTaskWindow.ShowDialog() == true)
                {
                    var newTask = new TaskItem
                    {
                        Title = addTaskWindow.TaskTitle,
                        Description = addTaskWindow.TaskDescription,
                        Priority = addTaskWindow.TaskPriority,
                        CreationDate = DateTime.Now,
                        IsCompleted = false,
                        Deadline = addTaskWindow.TaskDeadline,
                        CategoryId = SelectedCategory.Id
                    };

                    _databaseService.AddTask(newTask);
                    LoadTasksForCategory();
                }
            }
        }


        private void CompleteTask(TaskItem task)
        {
            if (task != null)
            {

                _databaseService.UpdateTask(task);
            }
        }

        /*
        private void CompleteTask()
        {
            if (SelectedTask != null)
            {
                SelectedTask.IsCompleted = true;
                _databaseService.UpdateTask(SelectedTask);
            }
        }
        */

        private void DeleteCategory()
        {
            if (SelectedCategory != null)
            {
                // Предупреждение пользователя
                var result = MessageBox.Show($"Are you sure you want to delete category '{SelectedCategory.Name}' and all its tasks?", "Delete Category", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.Yes)
                {
                    _databaseService.DeleteCategory(SelectedCategory.Id);
                    Categories.Remove(SelectedCategory);
                    Tasks.Clear(); // Очистить список задач
                }
            }
        }

        private void DeleteTask()
        {
            if (SelectedTask != null)
            {
                _databaseService.DeleteTask(SelectedTask.Id);
                Tasks.Remove(SelectedTask);
            }
        }

        private void EditCategory()
        {
            if (SelectedCategory != null)
            {
                var editCategoryWindow = new AddCategoryWindow
                {
                    Title = "Edit Category"
                };
                editCategoryWindow.CategoryNameTextBox.Text = SelectedCategory.Name;

                if (editCategoryWindow.ShowDialog() == true)
                {
                    SelectedCategory.Name = editCategoryWindow.CategoryName;
                    _databaseService.UpdateCategory(SelectedCategory);
                    LoadCategories();
                }
            }
        }

        private void EditTask()
        {
            if (SelectedTask != null)
            {
                var editTaskWindow = new AddTaskWindow
                {
                    Title = "Edit Task"
                };
                editTaskWindow.TaskTitleTextBox.Text = SelectedTask.Title;
                editTaskWindow.TaskDescriptionTextBox.Text = SelectedTask.Description;
                editTaskWindow.PriorityComboBox.SelectedIndex = (int)SelectedTask.Priority;

                if (editTaskWindow.ShowDialog() == true)
                {
                    SelectedTask.Title = editTaskWindow.TaskTitle;
                    SelectedTask.Description = editTaskWindow.TaskDescription;
                    SelectedTask.Priority = editTaskWindow.TaskPriority;
                    SelectedTask.Deadline = editTaskWindow.TaskDeadline;
                    _databaseService.UpdateTask(SelectedTask);
                    LoadTasksForCategory();
                }
            }
        }

        private void LoadCategories()
        {
            Categories.Clear();
            foreach (var category in _databaseService.GetCategories())
            {
                Categories.Add(category);
            }
        }
    }



}
