using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Entities
{
  public class User : IdentityUser<Guid>
  {
    // ------ RELATIONSHIPS

    public ICollection<Microcontroller> Microcontrollers { get; set; }
  }
}
