using System.Diagnostics;

namespace AuraPlus.Web.Tracing;

public static class Tracing
{
    public static readonly ActivitySource Source = new("AuraPlus.Api");
}
