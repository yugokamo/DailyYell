using System;

namespace DailyYell.Unity.ValueObjects
{
    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;
    }
}
