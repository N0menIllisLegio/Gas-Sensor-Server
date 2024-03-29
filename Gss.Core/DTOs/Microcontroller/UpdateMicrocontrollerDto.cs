﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Gss.Core.DTOs.Microcontroller
{
  public class UpdateMicrocontrollerDto
  {
    [Required]
    [MaxLength(200)]
    public string Name { get; set; }

    [Required]
    public bool Public { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    [Required]
    public string Password { get; set; }

    public List<Guid> SensorIDs { get; set; }
  }
}
