namespace Gss.MicrocontrollerListener;

internal sealed class MicrocontrollersConnectionsOptions
{
    public const string SectionName = "MicrocontrollersConnectionsOptions";
    public int Port { get; set; }
    public int MessageTimespanSecondsDiff { get; set; }
}