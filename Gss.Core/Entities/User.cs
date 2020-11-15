using System;
using System.Collections.Generic;

namespace Gss.Core.Entities
{
    public class User
    {
        public Guid ID { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }


        // ------ RELATIONSHIPS

        public ICollection<Microcontroller> Microcontrollers { get; set; }
    }
}
