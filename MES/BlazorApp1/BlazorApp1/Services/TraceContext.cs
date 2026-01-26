using System.Threading;

namespace BlazorApp1.Services;

public static class TraceContext
{
    private static readonly AsyncLocal<string?> CurrentTraceId = new();

    public static string? TraceId
    {
        get => CurrentTraceId.Value;
        set => CurrentTraceId.Value = value;
    }
}
