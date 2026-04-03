using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SportsLeague.DataAccess.Reprositories
{
    public class TournamentTeamRepository : GenericRepository<TournamentTeam>, ITournamentTeamRepository
    {
        public TournamentTeamRepository(LeagueDbContext context) : base(context)
        {
        }

        public async Task<TournamentTeam?> GetByTournamentAndTeamAsync(
            int tournamentId, int teamId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(tt =>
                    tt.TournamentId == tournamentId && tt.TeamId == teamId);
        }

        public async Task<IEnumerable<TournamentTeam>> GetByTournamentAsync(
            int tournamentId)
        {
            return await _dbSet
                .Where(tt => tt.TournamentId == tournamentId)
                .Include(tt => tt.Team)
                .ToListAsync();
        }
    }

}
