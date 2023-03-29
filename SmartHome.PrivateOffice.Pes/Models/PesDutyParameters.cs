namespace SmartHome.PrivateOffice.Pes.Models;

internal record PesDutyParameters
{
    public string? FieldCode { get; init; }
    public string? FieldName { get; init; }
    public string? FieldType { get; init; }
    public string? FieldValue { get; init; }
}