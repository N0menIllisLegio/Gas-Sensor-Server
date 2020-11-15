using System;
using System.Collections.Generic;

namespace Gss.Core.Entities
{
    public class Microcontroller
    {
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string IPAddress { get; set; }
        public DateTime LastResponseTime { get; set; }
        public bool Public { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        

        // ---- RELATIONSHIPS

        public User Owner { get; set; }

        public ICollection<Sensor> Sensors { get; set; }


        // ----- NOT IN DB

        public SensorData TryParseData(string data)
        {
            return null;
        }

        public void LeaveConversationRequest()
        {
            
        }

        public bool UserRequestedData { get; }
    }
}
