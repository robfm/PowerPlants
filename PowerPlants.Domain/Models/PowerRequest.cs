using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlants.Domain.Models
{
    public class PowerRequest
    {
        public int Load { get; set; }

        public Dictionary<string, decimal> Fuels { get; set; }

        public IEnumerable<PowerPlant> Powerplants { get; set; }
    }
}
