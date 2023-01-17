namespace MegaDTelegramRemoteControl.Models;

public class NewEventResult
{
    public static readonly NewEventResult Default = new() {SendCustomCommand = false};

    public string? Command { get; set; }
    public bool SendCustomCommand { get; set; }
}