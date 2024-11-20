namespace Gss.MicrocontrollerListener.Email;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string Address { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string SmtpServer { get; set; } = null!;
    public int SmtpPort { get; set; }
    public bool SmtpUseSsl { get; set; }
}
