using PowerPlants.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PowerPlants.Domain.Services.Interfaces
{
    public interface IPowerLoadService
    {
        Task<List<RequestResult>> GetPowerPlants(PowerRequest powerRequest);
    }
}
