using PowerPlants.Domain.Models;
using PowerPlants.Domain.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerPlants.Business.Services
{
    public class PowerLoadService : IPowerLoadService
    {
        /// <summary>
        /// Method for selecting available plants considering fuel price and CO emissions cost
        /// </summary>
        /// <param name="powerRequest"></param>
        /// <returns></returns>
        public Task<List<RequestResult>> GetPowerPlants(PowerRequest powerRequest)
        {
            var results = new List<RequestResult>();

            try
            {
                // Grouping all the plants by type, sorting each group from higher to lower efficiency and from higher to lower maximum power.
                var groups = powerRequest.Powerplants.GroupBy(g => g.Type).ToDictionary(g => g.Key, g => g.OrderByDescending(a => a.Efficiency).ThenByDescending(a => a.Pmax).ToList());

                decimal load = powerRequest.Load;

                var plantsSortedByPrice = new List<PowerPlant>();

                var windPerc = default(decimal);
                if (powerRequest.Fuels.Keys.Any(a=>a.Contains("wind")))
                    windPerc = powerRequest.Fuels.First(a => a.Key.Contains("wind")).Value;

                if(groups.Keys.Any(a=>a.Contains("windturbine")))
                plantsSortedByPrice.AddRange(groups["windturbine"]);

                if (groups.Keys.Any(a => a.Contains("gasfired")))
                    plantsSortedByPrice.AddRange(groups["gasfired"]);

                if (groups.Keys.Any(a => a.Contains("turbojet")))
                    plantsSortedByPrice.AddRange(groups["turbojet"]);

                for (int i = 0; i < plantsSortedByPrice.Count && load > 0; i++)
                {
                    var plant = plantsSortedByPrice[i];

                    //In order to get the exact amount of capacity and use every plant, the Pmin for the next plant will be considered.
                    var nextPlant = new PowerPlant();
                    if (i + 1 < plantsSortedByPrice.Count)
                        nextPlant = plantsSortedByPrice[i + 1];

                    var pmax = plant.Pmax;
                    var pmin = plant.Pmin;

                    if (plant.Type == "windturbine")
                    {
                        pmax = pmax * windPerc / 100;
                        pmin = pmin * windPerc / 100;
                    }

                    var power = default(decimal);

                    if (pmax < 1)
                        continue;

                    if (pmax < load)
                    {
                        if (nextPlant.Pmin > (load - pmax))
                            power = load - nextPlant.Pmin;
                        else
                            power = pmax;

                        load -= power;
                    }
                    else
                    {
                        power = load;
                        load = default(decimal);
                    }

                    results.Add(new RequestResult { p = power, name = plant.Name });
                }
            }
            catch
            {
                throw;
            }

            return Task.FromResult(results);
        }
    }
}
