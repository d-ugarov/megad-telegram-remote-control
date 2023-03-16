namespace MegaDTelegramRemoteControl.Models;

public record NewEventResult
{
    public static NewEventResult Default => new() {SendOk200 = false};
    public static NewEventResult DoNothing => new() {SendOk200 = true};
    public static NewEventResult CustomCommand(string command) => new() {SendOk200 = true, SendOk200Data = command};

    public bool SendOk200 { get; private init; }
    public string? SendOk200Data { get; private init; }
}