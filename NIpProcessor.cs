using System;
using System.Text.RegularExpressions;

namespace CyberChatbotFixed.Services
{
    public enum UserIntent { Unknown, AddTask, AddTaskWithReminder, StartQuiz, ShowTasks, ShowActivityLog, CybersecurityQuestion }
    public class IntentResult
    {
        public UserIntent Intent { get; set; }
        public string TaskTitle { get; set; } = "";
        public DateTime? ReminderDate { get; set; }
    }
    public class NlpProcessor
    {
        public IntentResult DetectIntent(string rawInput)
        {
            string input = rawInput.Trim().ToLower();
            if (input.Contains("what have you done for me") || input.Contains("show activity log") || input.Contains("show my activity") || input.Contains("recent actions"))
                return new IntentResult { Intent = UserIntent.ShowActivityLog };
            if (input.Contains("start quiz") || input.Contains("play quiz") || input.Contains("cyber quiz") || input.Contains("test my knowledge"))
                return new IntentResult { Intent = UserIntent.StartQuiz };
            if (input.Contains("show tasks") || input.Contains("show my tasks") || input.Contains("view tasks"))
                return new IntentResult { Intent = UserIntent.ShowTasks };
            if (input.Contains("remind me to") || input.Contains("set a reminder") || input.Contains("add a reminder"))
                return new IntentResult { Intent = UserIntent.AddTaskWithReminder, TaskTitle = ExtractTaskTitle(input), ReminderDate = ExtractReminderDate(input) };
            if (input.Contains("add task") || input.Contains("add a task") || input.Contains("create task") || input.Contains("create a task"))
                return new IntentResult { Intent = UserIntent.AddTask, TaskTitle = ExtractTaskTitle(input) };
            return new IntentResult { Intent = UserIntent.CybersecurityQuestion };
        }
        public string ExtractTaskTitle(string input)
        {
            string title = Regex.Replace(input,
            @"^(please\s+)?(can\s+you\s+)?(remind\s+me\s+to\s*|set\s+a\s+reminder\s*(to|for)?\s*|add\s+a\s+reminder\s*(to|for)?\s*|add\s+a\s+task\s*(to)?\s*|add\s+task\s*(to)?\s*|create\s+a\s+task\s*(to)?\s*|create\s+task\s*(to)?\s*)",
            "", RegexOptions.IgnoreCase);
            title = Regex.Replace(title, @"\b(tomorrow|in\s+\d+\s+days?)\b\.?$", "", RegexOptions.IgnoreCase);
            title = title.Trim().TrimEnd('.');
            if (title.Length > 0) title = char.ToUpper(title[0]) + title.Substring(1);
            return title;
        }
        public DateTime? ExtractReminderDate(string input)
        {
            if (input.Contains("tomorrow")) return DateTime.Today.AddDays(1);
            Match m = Regex.Match(input, @"in\s+(\d+)\s+days?", RegexOptions.IgnoreCase);
            if (m.Success) return DateTime.Today.AddDays(Convert.ToInt32(m.Groups[1].Value));
            return null;
        }
    }
}