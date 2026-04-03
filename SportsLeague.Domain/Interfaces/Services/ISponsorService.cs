using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface ISponsorService
    {
        Task<IEnumerable<Sponsor>> GetAllAsync();
        Task<Sponsor?> GetByIdAsync(int id);
        Task<Sponsor> CreateAsync(Sponsor sponsor);
        Task UpdateAsync(int id, Sponsor sponsor);
        Task DeleteAsync(int id);

        Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId);
        Task<TournamentSponsor> AddSponsorToTournamentAsync(int sponsorId, TournamentSponsor tsRequest);
        Task RemoveSponsorFromTournamentAsync(int sponsorId, int tournamentId);
    }
}
