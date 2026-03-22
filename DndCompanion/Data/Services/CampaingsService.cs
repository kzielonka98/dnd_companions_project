using DndCompanion.Models;
using DndCompanion.Models.SystemMessages.CampaingInvitation;
using Microsoft.EntityFrameworkCore;

namespace DndCompanion.Data.Services
{
    public class CampaingsService : ICampaingsService
    {
        private readonly DndCompanionContext _context;

        public CampaingsService(DndCompanionContext context)
        {
            _context = context;
        }

        public async Task AddCampaignAsync(CampaignModel campaign, UserModel user)
        {
            UserCampaignModel userCampaign = new()
            {
                UserId = user.Id,
                User = user,
                CampaignId = campaign.Id,
                Campaign = campaign,
                IsOwner = true,
            };
            _context.UserCampaigns.Add(userCampaign);
            _context.Campaigns.Add(campaign);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCampaignAsync(int id)
        {
            var campaign = await _context.Campaigns.FindAsync(id);
            if (campaign != null)
            {
                _context.UserCampaigns.RemoveRange(
                    _context.UserCampaigns.Where(uc => uc.CampaignId == id)
                );
                _context.Campaigns.Remove(campaign);
                await _context.SaveChangesAsync();
            }
        }

        public async Task AddPlayerToCampaignAsync(CampaignModel campaign, UserModel user)
        {
            UserCampaignModel userCampaign = new()
            {
                UserId = user.Id,
                User = user,
                CampaignId = campaign.Id,
                Campaign = campaign,
                IsOwner = false,
            };
            _context.UserCampaigns.Add(userCampaign);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteInvitationAsync(int invitationId)
        {
            var invitation = await _context.CampaignInvitations.FindAsync(invitationId);
            if (invitation != null)
            {
                _context.CampaignInvitations.Remove(invitation);
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateCampaingInvitationAsync(
            CampaignModel campaign,
            UserModel sender,
            UserModel receiver,
            string content
        )
        {
            content ??=
                $"You have been invited by {sender.UserName} to join the campaign {campaign.Name}.";

            CampaignInvitationModel invitation = new()
            {
                CampaignId = campaign.Id,
                Campaign = campaign,
                SenderId = sender.Id,
                Sender = sender,
                ReceiverId = receiver.Id,
                Receiver = receiver,
                Content = content,
            };
            _context.CampaignInvitations.Add(invitation);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserFromCampaignAsync(int campaignId, string userId)
        {
            var userCampaign = await _context
                .UserCampaigns.Where(uc => uc.CampaignId == campaignId && uc.UserId == userId)
                .FirstOrDefaultAsync();
            if (userCampaign != null)
            {
                _context.UserCampaigns.Remove(userCampaign);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<CampaignModel>> GetAllPublicCampaignsAsync()
        {
            var campaigns = await _context
                .Campaigns.Where(c => c.Public)
                .Include(c => c.Users)
                .Select(c => c)
                .ToListAsync();
            return campaigns;
        }

        public async Task<IEnumerable<CampaignModel>> GetOwnedCampaignsByUserAsync(UserModel user)
        {
            var campaigns = await _context
                .Users.Where(u => u.Id == user.Id)
                .SelectMany(u => u.Campaigns)
                .Where(c => c.UsersCampaigns.Any(uc => uc.UserId == user.Id && uc.IsOwner))
                .Include(c => c.Users)
                .ToListAsync();
            return campaigns;
        }

        public async Task<IEnumerable<CampaignModel>> GetJoinedCampaignsByUserAsync(UserModel user)
        {
            var campaigns = await _context
                .Users.Where(u => u.Id == user.Id)
                .SelectMany(u => u.Campaigns)
                .Where(c => c.UsersCampaigns.Any(uc => uc.UserId == user.Id && !uc.IsOwner))
                .Include(c => c.Users)
                .ToListAsync();
            return campaigns;
        }

        public async Task<CampaignModel> GetCampaignByIdAsync(int id)
        {
            var campaign = await _context
                .Campaigns.Where(c => c.Id == id)
                .Include(c => c.Users)
                .Include(c => c.Characters)
                .FirstOrDefaultAsync();
            return campaign;
        }

        public async Task<
            IEnumerable<CampaignInvitationModel>
        > GetReceivedCampaignInvitationsForUserAsync(UserModel user)
        {
            var invitations = await _context
                .CampaignInvitations.Where(ci => ci.ReceiverId == user.Id)
                .Include(ci => ci.Campaign)
                .Include(ci => ci.Sender)
                .ToListAsync();

            return invitations;
        }

        public async Task<
            IEnumerable<CampaignInvitationModel>
        > GetSentCampaignInvitationsForUserAsync(UserModel user)
        {
            var invitations = await _context
                .CampaignInvitations.Where(ci => ci.SenderId == user.Id)
                .Include(ci => ci.Campaign)
                .Include(ci => ci.Receiver)
                .ToListAsync();

            return invitations;
        }

        public async Task<IEnumerable<CampaignModel>> GetCampaignsByUserInvitationsAsync(
            UserModel user
        )
        {
            var campaigns = await _context
                .CampaignInvitations.Where(ci => ci.ReceiverId == user.Id)
                .Include(ci => ci.Sender)
                .Select(ci => ci.Campaign)
                .ToListAsync();
            return campaigns;
        }

        public Task AssignCharacterToCampaignAsync(CampaignModel campaign, CharacterModel character)
        {
            CharacterCampaignModel characterCampaign = new()
            {
                CampaignId = campaign.Id,
                Campaign = campaign,
                CharacterId = character.Id,
                Character = character,
            };
            _context.CharacterCampaigns.Add(characterCampaign);
            return _context.SaveChangesAsync();
        }

        public async Task UnassignCharacterFromCampaignAsync(int campaignId, int characterId)
        {
            CharacterCampaignModel CharacterCampaign = await _context
                .CharacterCampaigns
                .Where(cc => cc.CampaignId == campaignId &&
                 cc.CharacterId == characterId)
                .FirstOrDefaultAsync();
            if (CharacterCampaign != null)
            {
                _context.CharacterCampaigns.Remove(CharacterCampaign);
                await _context.SaveChangesAsync();
            }
        }
    }
}
