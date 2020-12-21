﻿using System;

namespace Gss.Core.DTOs
{
  public record TokenDto
  {
    public string AccessToken { get; init; }
    public DateTime AccessTokenExpiration { get; init; }
    public string RefreshToken { get; init; }
    public Guid UserID { get; init; }
  }
}
