using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Request
{
    public class UpdateTournamentStatusDTO
    {
        public TournamentStatus Status { get; set; }
    }

}
