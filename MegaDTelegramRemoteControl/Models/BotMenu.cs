using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models
{
    public class BotMenu
    {
        public string Text { get; set; } = null!;
        public List<ButtonItem> Buttons { get; set; } = new();
    }

    public class ButtonItem
    {
        public string Name { get; set; } = null!;
        public string Id { get; set; } = null!;
        public int Order { get; set; }
    }
}