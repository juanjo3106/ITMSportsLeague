using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.Domain.Services
{
    public class SponsorService : ISponsorService
    {
        private readonly ISponsorRepository _sponsorRepository;
        private readonly ITournamentSponsorRepository _tournamentSponsorRepository;
        private readonly ITournamentRepository _tournamentRepository;
        private readonly ILogger<SponsorService> _logger;

        public SponsorService(
            ISponsorRepository sponsorRepository,
            ITournamentSponsorRepository tournamentSponsorRepository,
            ITournamentRepository tournamentRepository,
            ILogger<SponsorService> logger)
        {
            _sponsorRepository = sponsorRepository;
            _tournamentSponsorRepository = tournamentSponsorRepository;
            _tournamentRepository = tournamentRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Sponsor>> GetAllAsync()
        {
            _logger.LogInformation("Retrieving all sponsors");
            return await _sponsorRepository.GetAllAsync();
        }

        public async Task<Sponsor?> GetByIdAsync(int id)
        {
            _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
            var sponsor = await _sponsorRepository.GetByIdAsync(id);
            if (sponsor == null)
                _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);
            return sponsor;
        }

        public async Task<Sponsor> CreateAsync(Sponsor sponsor)
        {
            // Validaciones: nombre único y email válido
            var exists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
            if (exists)
                throw new InvalidOperationException($"Ya existe un sponsor con el nombre '{sponsor.Name}'");

            try
            {
                var mail = new System.Net.Mail.MailAddress(sponsor.ContactEmail);
            }
            catch
            {
                throw new InvalidOperationException("Email de contacto inválido");
            }

            _logger.LogInformation("Creating sponsor: {SponsorName}", sponsor.Name);
            return await _sponsorRepository.CreateAsync(sponsor);
        }

        public async Task UpdateAsync(int id, Sponsor sponsor)
        {
            var existing = await _sponsorRepository.GetByIdAsync(id);
            if (existing == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            if (existing.Name != sponsor.Name)
            {
                var nameExists = await _sponsorRepository.ExistsByNameAsync(sponsor.Name);
                if (nameExists)
                    throw new InvalidOperationException($"Ya existe un sponsor con el nombre '{sponsor.Name}'");
            }

            try
            {
                var mail = new System.Net.Mail.MailAddress(sponsor.ContactEmail);
            }
            catch
            {
                throw new InvalidOperationException("Email de contacto inválido");
            }

            existing.Name = sponsor.Name;
            existing.ContactEmail = sponsor.ContactEmail;
            existing.Phone = sponsor.Phone;
            existing.WebsiteUrl = sponsor.WebsiteUrl;
            existing.Category = sponsor.Category;

            _logger.LogInformation("Updating sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var exists = await _sponsorRepository.ExistsAsync(id);
            if (!exists)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {id}");

            _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
            await _sponsorRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<TournamentSponsor>> GetTournamentsBySponsorAsync(int sponsorId)
        {
            var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
            if (sponsor == null)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");

            return await _tournamentSponsorRepository.GetBySponsorIdAsync(sponsorId);
        }

        public async Task<TournamentSponsor> AddSponsorToTournamentAsync(int sponsorId, TournamentSponsor tsRequest)
        {
            // Validaciones
            var sponsorExists = await _sponsorRepository.ExistsAsync(sponsorId);
            if (!sponsorExists)
                throw new KeyNotFoundException($"No se encontró el sponsor con ID {sponsorId}");

            var tournamentExists = await _tournamentRepository.ExistsAsync(tsRequest.TournamentId);
            if (!tournamentExists)
                throw new KeyNotFoundException($"No se encontró el torneo con ID {tsRequest.TournamentId}");

            if (tsRequest.ContractAmount <= 0)
                throw new InvalidOperationException("ContractAmount debe ser mayor que 0");

            var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tsRequest.TournamentId, sponsorId);
            if (existing != null)
                throw new InvalidOperationException("La relación Sponsor-Tournament ya existe");

            var ts = new TournamentSponsor
            {
                TournamentId = tsRequest.TournamentId,
                SponsorId = sponsorId,
                ContractAmount = tsRequest.ContractAmount,
                JoinedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Adding sponsor {SponsorId} to tournament {TournamentId}", sponsorId, tsRequest.TournamentId);
            return await _tournamentSponsorRepository.CreateAsync(ts);
        }

        public async Task RemoveSponsorFromTournamentAsync(int sponsorId, int tournamentId)
        {
            var existing = await _tournamentSponsorRepository.GetByTournamentAndSponsorAsync(tournamentId, sponsorId);
            if (existing == null)
                throw new KeyNotFoundException("No se encontró la relación Sponsor-Tournament");

            _logger.LogInformation("Removing sponsor {SponsorId} from tournament {TournamentId}", sponsorId, tournamentId);
            await _tournamentSponsorRepository.DeleteAsync(existing.Id);
        }
    }
}
