namespace MegaDTelegramRemoteControl.Models.PES;

public record PesDutyParameters
{
    public string? FieldCode { get; init; }
    public string? FieldName { get; init; }
    public string? FieldType { get; init; }
    public string? FieldValue { get; init; }
}