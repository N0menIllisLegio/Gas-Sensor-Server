using System;
using Gss.Core.Interfaces;

namespace Gss.Core.Entities
{
  public class RefreshToken : IEntity
  {
    public Guid Id { get; set; }
    public string Token { get; set; }
    public DateTime ExpirationDate { get; set; }
    public virtual User User { get; set; }
  }
}
