using Application.Models;

namespace Application.Contracts.Infrastructure;

public interface IAuditTrailProvider<T>
{
    void AddLog(T auditTrailLog);
    IEnumerable<T> QueryAuditLogs(string filter = "*", AuditTrailPaging auditTrailPaging = null);
    long Count(string filter);
}