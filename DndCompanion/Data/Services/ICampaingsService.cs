using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DndCompanion.Models;

namespace DndCompanion.Data.Services
{
    public interface ICampaingsService
    {
        Task<IEnumerable<CampaignModel>> GetAllCampaignsAsync();

        Task AddCampaignAsync(CampaignModel campaign);
    }
}