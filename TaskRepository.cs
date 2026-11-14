using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using CyberChatbotFixed.Models;

namespace CyberChatbotFixed.Data
{
    public class TaskRepository
    {
        public int AddTask(CyberTask task)
        {
            string sql = @"INSERT INTO cyber_tasks (title, description, reminder_date, is_completed)
VALUES (@title, @description, @reminder, @completed);";
            using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@title", task.Title);
                command.Parameters.AddWithValue("@description", task.Description);
                command.Parameters.AddWithValue("@reminder", task.ReminderDate.HasValue ? (object)task.ReminderDate.Value : DBNull.Value);
                command.Parameters.AddWithValue("@completed", task.IsCompleted);
                connection.Open();
                command.ExecuteNonQuery();
                return Convert.ToInt32(command.LastInsertedId);
            }
        }

        public List<CyberTask> GetAllTasks()
        {
            List<CyberTask> tasks = new List<CyberTask>();
            string sql = @"SELECT Id AS task_id, title, description, reminder_date, is_completed FROM cyber_tasks ORDER BY is_completed, task_id DESC;";
            using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
            using (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                connection.Open();
                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tasks.Add(new CyberTask
                        {
                            TaskId = Convert.ToInt32(reader["task_id"]),
                            Title = Convert.ToString(reader["title"]) ?? "",
                            Description = Convert.ToString(reader["description"]) ?? "",
                            ReminderDate = reader["reminder_date"] == DBNull.Value ? (DateTime?)null: Convert.ToDateTime(reader["reminder_date"]),
                            IsCompleted = Convert.ToBoolean(reader["is_completed"])
                        });
                    }
                }
            }
            return tasks;
        }

        public void MarkTaskCompleted(int taskId)
        {
            string sql = @"UPDATE cyber_tasks SET is_completed = TRUE WHERE task_id = @id;";
            using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
           using   (MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", taskId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void DeleteTask(int taskId)
        {
            string sql = "DELETE FROM cyber_tasks WHERE task_id = @id;";
            using (MySqlConnection connection = new MySqlConnection(DatabaseConfig.ConnectionString))
             using(MySqlCommand command = new MySqlCommand(sql, connection))
            {
                command.Parameters.AddWithValue("@id", taskId);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
}