using CyberChatbotFixed.Data;
using CyberChatbotFixed.Models;
using CyberChatbotFixed.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace CyberChatbotFixed
{
    public class MainForm : Form
    {
        private RichTextBox chatBox;
        private TextBox inputBox;
        private Button sendButton;

        private QuizManager quizManager = new QuizManager();
        private NlpProcessor nlp = new NlpProcessor();
        private ActivityLogger logger = new ActivityLogger();
        private TaskRepository taskRepo = new TaskRepository();
        private bool waitingForQuizAnswer = false;

        public MainForm()
        {
            Text = "Cybersecurity Chatbot";
            Width = 700;
            Height = 500;

            chatBox = new RichTextBox();
            chatBox.Dock = DockStyle.Top;
            chatBox.Height = 380;
            chatBox.ReadOnly = true;
            Controls.Add(chatBox);

            inputBox = new TextBox();
            inputBox.Dock = DockStyle.Bottom;
            inputBox.Height = 30;
            Controls.Add(inputBox);

            sendButton = new Button();
            sendButton.Text = "Send";
            sendButton.Dock = DockStyle.Bottom;
            sendButton.Height = 30;
            sendButton.Click += SendButton_Click;
            Controls.Add(sendButton);

            AddBotMessage("Hello! I am your Cybersecurity Chatbot.");
            AddBotMessage("Try: what is phishing | start quiz | add task - change my password | show tasks | show activity log");
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string input = inputBox.Text.Trim();
            if (string.IsNullOrEmpty(input)) return;
            AddUserMessage(input);
            inputBox.Clear();

            if (waitingForQuizAnswer)
            {
                HandleQuizAnswer(input);
                return;
            }

            IntentResult result = nlp.DetectIntent(input);

            try
            {
                switch (result.Intent)
                {
                    case UserIntent.StartQuiz:
                        quizManager.StartQuiz();
                        waitingForQuizAnswer = true;
                        logger.Log("Started quiz");
                        ShowCurrentQuestion();
                        break;

                    case UserIntent.AddTask:
                        if (string.IsNullOrEmpty(result.TaskTitle))
                        {
                            AddBotMessage("What task would you like to add? Example: add task - change my password");
                        }
                        else
                        {
                            taskRepo.AddTask(new CyberTask { Title = result.TaskTitle, Description = result.TaskTitle, IsCompleted = false });
                            logger.Log("Added task: " + result.TaskTitle);
                            AddBotMessage("Task added: " + result.TaskTitle);
                        }
                        break;

                    case UserIntent.AddTaskWithReminder:
                        taskRepo.AddTask(new CyberTask { Title = result.TaskTitle, Description = result.TaskTitle, IsCompleted = false, ReminderDate = result.ReminderDate });
                        logger.Log("Added task with reminder: " + result.TaskTitle);
                        AddBotMessage("Task added with reminder: " + result.TaskTitle);
                        break;

                    case UserIntent.ShowTasks:
                        var tasks = taskRepo.GetAllTasks();
                        if (tasks.Count == 0)
                            AddBotMessage("You have no tasks yet. Try: add task - change my password");
                        else
                        {
                            AddBotMessage("Here are your tasks:");
                            foreach (var t in tasks)
                                AddBotMessage("- " + t.Title + (t.IsCompleted ? " (Done)" : ""));
                        }
                        break;

                    case UserIntent.ShowActivityLog:
                        AddBotMessage(logger.GetSummary(10));
                        break;

                    default:
                        AddBotMessage(GetCyberAnswer(input));
                        break;
                }
            }
            catch (Exception ex)
            {
                AddBotMessage("Database is not available right now (MySQL is not running). Start MySQL in XAMPP and try again.");
                AddBotMessage("Error: " + ex.Message);
            }
        }

        private void HandleQuizAnswer(string input)
        {
            int selected = -1;
            input = input.Trim().ToLower();
            if (input == "a" || input == "1") selected = 0;
            else if (input == "b" || input == "2") selected = 1;
            else if (input == "c" || input == "3") selected = 2;
            else if (input == "d" || input == "4") selected = 3;

            if (selected < 0)
            {
                AddBotMessage("Please answer with 1, 2, 3 or 4 (or a, b, c, d).");
                return;
            }

            QuizAnswerResult answer = quizManager.CheckAnswer(selected);
            AddBotMessage(answer.Feedback);
            logger.Log("Answered quiz question: " + (answer.IsCorrect ? "Correct" : "Incorrect"));

            if (quizManager.MoveNext())
            {
                ShowCurrentQuestion();
            }
            else
            {
                waitingForQuizAnswer = false;
                AddBotMessage("Quiz finished! Your score: " + quizManager.Score + " / " + quizManager.TotalQuestions);
            }
        }

        private void ShowCurrentQuestion()
        {
            QuizQuestion q = quizManager.CurrentQuestion;
            AddBotMessage("Question " + (quizManager.CurrentIndex + 1) + ": " + q.QuestionText);
            for (int i = 0; i < q.Options.Count; i++)
                AddBotMessage((i + 1) + ". " + q.Options[i]);
            AddBotMessage("Type 1, 2, 3 or 4 to answer.");
        }

        private string GetCyberAnswer(string input)
        {
            string lower = input.ToLower();
            if (lower.Contains("phishing")) return "Phishing - Don't click suspicious links!";
            if (lower.Contains("password")) return "Use a strong, unique password and turn on 2FA.";
            if (lower.Contains("hello") || lower.Contains("hi")) return "Hi! Ask me anything about Cybersecurity!";
            return "I can help with phishing, passwords, quizzes, tasks and activity logs. Try: start quiz";
        }

        private void AddUserMessage(string msg) { chatBox.AppendText("\nYou: " + msg + "\n"); }
        private void AddBotMessage(string msg) { chatBox.AppendText("Bot: " + msg + "\n"); }
    }
}