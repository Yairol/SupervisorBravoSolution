using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Persistence.Helpers
{

        public class TemperatureAggregate
        {
            public Guid DixellId { get; set; }
            public string DixellName { get; set; } = string.Empty;
            public double? SetPoint { get; set; }
            public double? AvgTemperature { get; set; }
            public double? MinTemperature { get; set; }
            public double? MaxTemperature { get; set; }
            public double AvgControl { get; set; }
            public double AvgOffTime { get; set; }
            public double AvgDisconnectTime { get; set; }
            public TimeSpan ControlTime { get; set; }
            public TimeSpan DisconnectTime { get; set; }
            public TimeSpan OffTime { get; set; }
        }
    

}
