using System;
using System.Collections.Generic;
using System.Data.SQLite;
using To_Do_List.Models;

public class DatabaseService
{
    private const string ConnectionString = "Data Source=tasks.db;Version=3;";

    public DatabaseService()
    {
        InitializeDatabase();
    }

    public void InitializeDatabase()
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            string createCategoryTable = @"CREATE TABLE IF NOT EXISTS Categories (
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        Name TEXT NOT NULL)";
            string createTaskTable = @"CREATE TABLE IF NOT EXISTS Tasks (
                                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                    Title TEXT NOT NULL,
                                    Description TEXT,
                                    IsCompleted INTEGER,
                                    CreationDate DATETIME,
                                    Deadline DATETIME,  -- Новый столбец для дэдлайна
                                    Priority INTEGER,
                                    CategoryId INTEGER,
                                    FOREIGN KEY(CategoryId) REFERENCES Categories(Id))";
            var command = new SQLiteCommand(createCategoryTable, connection);
            command.ExecuteNonQuery();
            command.CommandText = createTaskTable;
            command.ExecuteNonQuery();
        }
    }

    public List<Category> GetCategories()
    {
        var categories = new List<Category>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("SELECT * FROM Categories", connection);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                categories.Add(new Category
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1)
                });
            }
        }
        return categories;
    }

    public List<TaskItem> GetTasksForCategory(int categoryId)
    {
        var tasks = new List<TaskItem>();
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("SELECT * FROM Tasks WHERE CategoryId = @CategoryId", connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                tasks.Add(new TaskItem
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Description = reader.GetString(2),
                    IsCompleted = reader.GetInt32(3) == 1,
                    CreationDate = reader.GetDateTime(4),
                    Deadline = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5), // Получаем дэдлайн, если он установлен
                    Priority = (PriorityLevel)reader.GetInt32(6),
                    CategoryId = reader.GetInt32(7)
                });
            }
        }
        return tasks;
    }


    public void AddCategory(Category category)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("INSERT INTO Categories (Name) VALUES (@Name)", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.ExecuteNonQuery();
        }
    }

    public void AddTask(TaskItem task)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("INSERT INTO Tasks (Title, Description, IsCompleted, CreationDate, Deadline, Priority, CategoryId) VALUES (@Title, @Description, @IsCompleted, @CreationDate, @Deadline, @Priority, @CategoryId)", connection);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@CreationDate", task.CreationDate);
            command.Parameters.AddWithValue("@Deadline", task.Deadline ?? (object)DBNull.Value);  // Учитываем, что дэдлайн может быть пустым
            command.Parameters.AddWithValue("@Priority", (int)task.Priority);
            command.Parameters.AddWithValue("@CategoryId", task.CategoryId);
            command.ExecuteNonQuery();
        }
    }

    public void UpdateTask(TaskItem task)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("UPDATE Tasks SET Title = @Title, Description = @Description, Priority = @Priority, IsCompleted = @IsCompleted, Deadline = @Deadline WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@Priority", (int)task.Priority);
            command.Parameters.AddWithValue("@IsCompleted", task.IsCompleted ? 1 : 0);
            command.Parameters.AddWithValue("@Deadline", task.Deadline ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Id", task.Id);
            command.ExecuteNonQuery();
        }
    }

    public void UpdateCategory(Category category)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("UPDATE Categories SET Name = @Name WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Name", category.Name);
            command.Parameters.AddWithValue("@Id", category.Id);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteCategory(int categoryId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("DELETE FROM Tasks WHERE CategoryId = @CategoryId", connection);
            command.Parameters.AddWithValue("@CategoryId", categoryId);
            command.ExecuteNonQuery();

            command.CommandText = "DELETE FROM Categories WHERE Id = @Id";
            command.Parameters.AddWithValue("@Id", categoryId);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteTask(int taskId)
    {
        using (var connection = new SQLiteConnection(ConnectionString))
        {
            connection.Open();
            var command = new SQLiteCommand("DELETE FROM Tasks WHERE Id = @Id", connection);
            command.Parameters.AddWithValue("@Id", taskId);
            command.ExecuteNonQuery();
        }
    }
}
