namespace MegaDTelegramRemoteControl.Models.PES;

public record PesDisplayKey
{
    public string? FieldName { get; init; }
    public string? FieldCode { get; init; }
    public string? FieldValue { get; init; }
    public string? FieldType { get; init; }
}