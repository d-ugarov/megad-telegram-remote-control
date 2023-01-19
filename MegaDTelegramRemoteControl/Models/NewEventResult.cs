namespace MegaDTelegramRemoteControl.Models;

public record NewEventResult
{
    public static NewEventResult Default => new() {SendCustomCommand = false};
    public static NewEventResult DoNothing => new() {SendCustomCommand = true, Command = string.Empty};
    public static NewEventResult CustomCommand(string command) => new() {SendCustomCommand = true, Command = command};

    public bool SendCustomCommand { get; private init; }
    public string? Command { get; private init; }
}