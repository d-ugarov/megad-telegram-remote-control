using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Infrastructure.Configurations
{
    public class TelegramConfig
    {
        public string BotAccessToken { get; set; }
        
        public HashSet<int> AllowedUsers { get; set; } = new HashSet<int>();
        
        public HashSet<int> DebugLogUsers { get; set; } = new HashSet<int>();
    }
}