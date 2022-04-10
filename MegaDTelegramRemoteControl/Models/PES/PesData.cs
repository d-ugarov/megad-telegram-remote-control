using System.Collections.Generic;

namespace MegaDTelegramRemoteControl.Models.PES;

public record PesData
{
    public PesDataBalanceDetails BalanceDetails { get; init; } = new();
    public PesDataIndicationInfo IndicationInfo { get; init; } = new();
    public PesDataLastPaymentInfo LastPaymentInfo { get; init; } = new();
}

public record PesDataBalanceDetails
{
    public decimal Balance { get; init; }
    public string BillDate { get; init; } = "";
    public long BillId { get; init; }
    public List<PesDutyParameters> DutyParameters { get; init; } = new();
    public bool EditAmount { get; init; }
    public decimal Fee { get; init; }
    public List<object> FeeDetails { get; init; } = new();
    public object? IsDistributionNeeded { get; init; }
    public object? PayUntil { get; init; }
    public List<PesDataSubServiceBalances> SubServiceBalances { get; init; } = new();
    public decimal ToPay { get; init; }
}

public record PesDataSubServiceBalances
{
    public decimal Amount { get; init; }
    public decimal AmountToPay { get; init; }
    public List<PesDutyParameters> DutyParameters { get; init; } = new();
    public bool EditAmount { get; init; }
    public decimal Fine { get; init; }
    public decimal FineToPay { get; init; }
    public bool IsDop { get; init; }
    public string SubServiceId { get; init; } = "";
    public string SubServiceName { get; init; } = "";
}

public record PesDataIndicationInfo
{
    public List<PesDisplayKey> AccountDisplayKey = new();
    public string declareType { get; init; } = "";
    public bool IsNeedDeclareIndications { get; init; }
    public int ProviderId { get; init; }
    public string ProviderName { get; init; } = "";
    public string ServiceName { get; init; } = "";
    public List<PesDataSubService> SubServices { get; init; } = new();
}

public record PesDataSubService
{
    public string Date { get; init; } = "";
    public List<PesDutyParameters> DutyParameters { get; init; } = new();
    public string IndicationTransferStatus { get; init; } = "";
    public string MeasurementUnit { get; init; } = "";
    public decimal MeterCapacity { get; init; }
    public string MeterId { get; init; } = "";
    public string MeterNumber { get; init; } = "";
    public int NumberOfDigitsRight { get; init; }
    public string Scale { get; init; } = "";
    public string SubServiceId { get; init; } = "";
    public string SubServiceName { get; init; } = "";
    public decimal Value { get; init; }
}

public record PesDataLastPaymentInfo
{
    public int PaymentsCount { get; init; }
    public int TotalSum { get; init; }
}