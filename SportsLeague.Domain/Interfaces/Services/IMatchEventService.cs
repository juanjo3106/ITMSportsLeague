using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Services
{
    public interface IMatchEventService
    {
        #region Match Results
        Task<MatchResult> RegisterResultAsync(int matchId, MatchResult result);
        Task<MatchResult?> GetResultByMatchAsync(int matchId);
        #endregion

        #region Goals
        Task<Goal> RegisterGoalAsync(int matchId, Goal goal);
        Task<IEnumerable<Goal>> GetGoalsByMatchAsync(int matchId);
        Task DeleteGoalAsync(int goalId);
        #endregion

        #region Cards
        Task<Card> RegisterCardAsync(int matchId, Card card);
        Task<IEnumerable<Card>> GetCardsByMatchAsync(int matchId);
        Task DeleteCardAsync(int cardId);
        #endregion
    }

}
