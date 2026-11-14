using CyberChatbotFixed.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CyberChatbotFixed.Services
{
    public class ActivityLogger
    {
        private readonly List<ActivityEntry> entries = new List<ActivityEntry>();
        public void Log(string description) { entries.Add(new ActivityEntry { Time = DateTime.Now, Description = description }); }
        public List<ActivityEntry> GetRecent(int count) { return entries.OrderByDescending(x => x.Time).Take(count).ToList(); }
        public string GetSummary(int count)
        {
            var recent = GetRecent(count);
            if (recent.Count == 0) return "No activity recorded yet. Try adding a task or starting the quiz.";
            string summary = "Here's a summary of recent actions:";
            int i = 1;
            foreach (var e in recent.OrderBy(x => x.Time)) { summary += $"\n{i}. {e.Description}"; i++; }
            return summary;
        }
    }
}