using SupervisorBravo.Domain.Entities.Common;
using SupervisorBravo.Domain.Entities.Dixell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupervisorBravo.Domain.Entities.System
{
    public class CustomAlarm : Entity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DixellBase DixellBase { get; set; }
        public bool AlarmEnabled { get; set; }
        public bool IsGreaterThan { get; set; }
        public bool IsLessThan { get; set; }
        public double Value { get; set; }

        public CustomAlarm(string name, string description, DixellBase dixellBase, bool alarmEnabled, bool isGreaterThan, bool isLessThan, double value)
        {
            Name = name;
            Description = description;
            DixellBase = dixellBase;
            AlarmEnabled = alarmEnabled;
            IsGreaterThan = isGreaterThan;
            IsLessThan = isLessThan;
            Value = value;
        }

        public CustomAlarm()
        {
            Name = string.Empty;
            Description = string.Empty;
            
        }
    }
}
