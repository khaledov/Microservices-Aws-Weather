using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloudWeather.DataLoader.Models
{
    internal class TemperatureModel
    {
        public DateTime CreatedOn { get; set; }
        public decimal TempHigh { get; set; }
        public decimal TempLow { get; set; }
        public string? ZipCode { get; set; }
    }
}
