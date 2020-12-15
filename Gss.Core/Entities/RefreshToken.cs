using System;

namespace Gss.Core.Entities
{
  public class RefreshToken
  {
    public Guid ID { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public virtual User User { get; set; }
  }
}
