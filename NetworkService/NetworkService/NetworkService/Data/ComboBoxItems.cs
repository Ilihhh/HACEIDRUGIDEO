using System.Collections.Generic;

namespace NetworkService.Data
{
    public class ComboBoxItems
    {
        public static Dictionary<string, string> entityTypes = new Dictionary<string, string>()
        {
            {"Cable Sensor" , "pack://application:,,,/NetworkService;component/Images/CableSensor.png"},
            {"Digital Manometer", "pack://application:,,,/NetworkService;component/Images/DigitalManometer.png" }
        };
    }
}
