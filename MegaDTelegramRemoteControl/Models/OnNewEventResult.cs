namespace MegaDTelegramRemoteControl.Models
{
    public class OnNewEventResult
    {
        public static readonly OnNewEventResult Default = new OnNewEventResult {SendCustomCommand = false};
        
        public string Command { get; set; }
        public bool SendCustomCommand { get; set; }
    }
}