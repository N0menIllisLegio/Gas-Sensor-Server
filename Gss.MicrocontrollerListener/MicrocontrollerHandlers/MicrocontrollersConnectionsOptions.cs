namespace Gss.MicrocontrollerListener.MicrocontrollerHandlers;

public sealed class MicrocontrollersConnectionsOptions
{
    public const string SectionName = "MicrocontrollersConnectionsOptions";

    public string IpAddress { get; set; } = null!;
    public int Port { get; set; }
    public int SendTimeout { get; set; }
    public int ReceiveTimeout { get; set; }
    public int ListenQueue { get; set; }
}
