 using System.Collections.Generic;

namespace CyberChatbotFixed.Models
{
    public class QuizQuestion
    {
        public string QuestionText { get; set; } = "";
        public List<string> Options { get; set; } = new List<string>();
        public int CorrectOptionIndex { get; set; }
        public string Explanation { get; set; } = "";

    }
    public class QuizAnswerResult
    {
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; } = "";
    }
}
