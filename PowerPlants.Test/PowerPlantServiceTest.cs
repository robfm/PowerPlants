using PowerPlants.Business.Services;
using PowerPlants.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace PowerPlants.Test
{
    public class PowerPlantServiceTest
    {
        private readonly PowerLoadService _powerloadService;
        public PowerPlantServiceTest()
        {
            _powerloadService = new PowerLoadService();
        }

        [Fact]
        public void RemovesPlantsfromResultsHavingNoPower()
        {
            // Arrange
            var request = GetRequest(210,0);

            // Act
            var result = _powerloadService.GetPowerPlants(request).Result;

            // Assert
            Assert.False(result.Any(a=>a.p == 0));
        }

        [Fact]
        public void CheckIfResultContainsMoreEfficientPlants()
        {
            // Arrange
            var request = GetRequest(480, 60);

            // Act
            var result = _powerloadService.GetPowerPlants(request).Result;
            var sortedPlants = request.Powerplants.OrderByDescending(a => a.Efficiency).ThenByDescending(a=>a.Pmax).ToList();

            bool IsSorted = true;
            for (int i = 0; i < result.Count && IsSorted; i++)
            {
                int index = sortedPlants.FindIndex(a => a.Name == result[i].name);

                if(index != i)
                    IsSorted = false;
            }

            //Assert
            Assert.True(IsSorted);
        }

        private PowerRequest GetRequest(int load, int windPercent) 
        {
            var request = new PowerRequest();
            var rng = new Random();
            var powerPlants = new List<PowerPlant>();
            decimal[] efficiency = new[] { 0.53M, 0.37M };
            var fuels = new Dictionary<string, decimal>();

            request.Load = load;
            
            powerPlants.AddRange(Enumerable.Range(1, 2).Select(index => new PowerPlant
            {
                Efficiency = 1,
                Name = "windPark" + rng.Next(0, 99),
                Pmax = rng.Next(50, 150),
                Pmin = 0,
                Type = "windturbine"
            }));

            powerPlants.AddRange(Enumerable.Range(1, 2).Select(index => new PowerPlant
            {
                Efficiency = efficiency[rng.Next(efficiency.Length)],
                Name = "gasfired" + rng.Next(0, 99),
                Pmax = rng.Next(200, 400),
                Pmin = rng.Next(40, 100),
                Type = "gasfired"
            }));

            powerPlants.Add(new PowerPlant
            {
                Efficiency = 0.3M,
                Name = "turbojet1",
                Pmax = 17,
                Pmin = 0,
                Type = "turbojet"
            });
            request.Powerplants = powerPlants;            

            fuels.Add("wind(%)", windPercent);
            fuels.Add("kerosine(euro/MWh)", 50.8M);
            fuels.Add("co2(euro/ton)", 20);
            fuels.Add("gas(euro / MWh)", 13.4M);

            request.Fuels = fuels;

            return request;
        }
    }
}
