namespace Application.Contracts.Infrastructure;

public interface IAuditTrailLog
{
    public DateTime Timestamp { get; set; }
}