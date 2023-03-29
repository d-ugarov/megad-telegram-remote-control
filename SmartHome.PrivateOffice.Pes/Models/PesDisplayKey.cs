namespace SmartHome.PrivateOffice.Pes.Models;

internal record PesDisplayKey
{
    public string? FieldName { get; init; }
    public string? FieldCode { get; init; }
    public string? FieldValue { get; init; }
    public string? FieldType { get; init; }
}