﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Gss.Core.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Gss.Core.Entities
{
  public class User : IdentityUser<Guid>, IEntity
  {
    [NotMapped]
    public Guid ID { get => Id; set => Id = value; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string AvatarPath { get; set; }

    public string Gender { get; set; }
    public DateTime? Birthday { get; set; }

    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTime CreationDate { get; set; }

    // ------ RELATIONSHIPS

    public virtual ICollection<Microcontroller> Microcontrollers { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }
  }
}
