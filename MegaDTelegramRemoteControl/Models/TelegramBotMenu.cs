using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models
{
    public class TelegramBotMenu
    {
        public string Text { get; set; }
        public List<ButtonItem> Buttons { get; set; }
    }

    public class ButtonItem
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public int Order { get; set; }
    }
}