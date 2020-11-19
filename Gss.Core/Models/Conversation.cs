using System;
using Gss.Core.Entities;
using Gss.Core.Enums;

namespace Gss.Core.Models
{
  class Conversation
  {
    public DateTime RequestLeavedTime { get; set; }
    public SensorData SensorData { get; set; }
    public DateTime ResponseReceivedTime { get; set; }
    public ConversationStatus Status { get; set; }
  }
}
