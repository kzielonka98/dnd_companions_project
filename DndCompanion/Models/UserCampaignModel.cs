using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DndCompanion.Models
{
    public class UserCampaignModel
    {
        public string UserId { get; set; } = null!;

        public UserModel User { get; set; } = null!;

        public int CampaignId { get; set; }

        public CampaignModel Campaign { get; set; } = null!;

        public bool IsOwner { get; set; }
    }
}
