using System;

namespace EmployeeManagement.Models
{
    public class ChatMessage
    {
        public string Sender { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string FormattedMessage => $"[{Timestamp:HH:mm}] {Sender}: {Text}";
    }
}