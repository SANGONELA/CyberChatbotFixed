using System;
using System.Drawing;
using System.Windows.Forms;

namespace CyberChatbotFixed
{
    public partial class Form1 : Form
    {
        RichTextBox chatDisplay;
        TextBox inputBox;
        Button sendButton;

        public Form1()
        {
            InitializeComponent();
            this.Text = "Cybersecurity Awareness Chatbot";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterScreen;

            chatDisplay = new RichTextBox();
            chatDisplay.Location = new Point(10, 10);
            chatDisplay.Size = new Size(760, 450);
            chatDisplay.ReadOnly = true;
            chatDisplay.Font = new Font("Consolas", 11);
            chatDisplay.BackColor = Color.Black;
            chatDisplay.ForeColor = Color.Lime;
            this.Controls.Add(chatDisplay);

            inputBox = new TextBox();
            inputBox.Location = new Point(10, 475);
            inputBox.Size = new Size(600, 30);
            inputBox.Font = new Font("Segoe UI", 11);
            this.Controls.Add(inputBox);

            sendButton = new Button();
            sendButton.Location = new Point(620, 475);
            sendButton.Size = new Size(150, 30);
            sendButton.Text = "Send";
            sendButton.Click += SendButton_Click;
            this.Controls.Add(sendButton);

            inputBox.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    SendButton_Click(s, e);
                    e.SuppressKeyPress = true;
                }
            };

            chatDisplay.AppendText("Bot: Hello! I am your Cybersecurity Chatbot.\n");
            chatDisplay.AppendText("Bot: Ask me about phishing, passwords, malware, safe browsing.\n\n");
        }

        private void SendButton_Click(object sender, EventArgs e)
        {
            string user = inputBox.Text.Trim();
            if (user == "") return;

            chatDisplay.SelectionColor = Color.Cyan;
            chatDisplay.AppendText("\nYou: " + user + "\n");
            inputBox.Clear();

            string lower = user.ToLower();
            string bot = "Bot: Try asking about phishing, passwords, malware, safe browsing, social engineering.";

            if (lower.Contains("phish")) bot = "Bot: Phishing - Don't click suspicious links! Check sender email, hover over links, never share passwords via email.";
            else if (lower.Contains("password")) bot = "Bot: Use 12+ chars, mix letters/numbers/symbols, use password manager, enable 2FA.";
            else if (lower.Contains("malware") || lower.Contains("virus")) bot = "Bot: Keep antivirus updated, don't download from untrusted sites, keep Windows updated.";
            else if (lower.Contains("brows") || lower.Contains("safe")) bot = "Bot: Use HTTPS, avoid public WiFi for banking, keep browser updated.";
            else if (lower.Contains("social")) bot = "Bot: Social Engineering - Attackers trick you. Verify identity by phone, don't share OTP.";
            else if (lower.Contains("hi") || lower.Contains("hello")) bot = "Bot: Hi! Ask me anything about Cybersecurity!";
            else if (lower.Contains("thank")) bot = "Bot: You're welcome! Stay safe online!";

            chatDisplay.SelectionColor = Color.Lime;
            chatDisplay.AppendText(bot + "\n\n");
            chatDisplay.ScrollToCaret();
        }

        private void Form1_Load(object sender, EventArgs e) { }
    }
} 