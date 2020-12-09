using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Entities
{
  public class User : IdentityUser<Guid>
  {
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AvatarPath { get; set; }

    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreationDate { get; set; }

    // ------ RELATIONSHIPS

    public ICollection<Microcontroller> Microcontrollers { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; }
  }
}
