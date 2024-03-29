﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Gss.Core.Helpers;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Entities
{
  public class User : IdentityUser<Guid>, IEntity
  {
    [ExpressionsBuilder]
    public string FirstName { get; set; }

    [ExpressionsBuilder]
    public string LastName { get; set; }
    public string AvatarPath { get; set; }

    [ExpressionsBuilder]
    public string Gender { get; set; }

    // [ExpressionsBuilder]
    public DateTimeOffset? Birthday { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    // [ExpressionsBuilder]
    public DateTimeOffset CreationDate { get; set; }

    // ------ RELATIONSHIPS

    public virtual ICollection<Microcontroller> Microcontrollers { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
  }
}
