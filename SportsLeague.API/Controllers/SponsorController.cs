using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SportsLeague.API.DTOs.Request;
using SportsLeague.API.DTOs.Response;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Services;

namespace SportsLeague.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SponsorController : ControllerBase
    {
        private readonly ISponsorService _sponsorService;
        private readonly IMapper _mapper;

        public SponsorController(ISponsorService sponsorService, IMapper mapper)
        {
            _sponsorService = sponsorService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SponsorResponseDTO>>> GetAll()
        {
            var sponsors = await _sponsorService.GetAllAsync();
            var dto = _mapper.Map<IEnumerable<SponsorResponseDTO>>(sponsors);
            return Ok(dto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SponsorResponseDTO>> GetById(int id)
        {
            var sponsor = await _sponsorService.GetByIdAsync(id);
            if (sponsor == null)
                return NotFound(new { message = $"Sponsor con ID {id} no encontrado" });

            return Ok(_mapper.Map<SponsorResponseDTO>(sponsor));
        }

        [HttpPost]
        public async Task<ActionResult<SponsorResponseDTO>> Create(SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                var created = await _sponsorService.CreateAsync(sponsor);
                var response = _mapper.Map<SponsorResponseDTO>(created);
                return CreatedAtAction(nameof(GetById), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, SponsorRequestDTO dto)
        {
            try
            {
                var sponsor = _mapper.Map<Sponsor>(dto);
                await _sponsorService.UpdateAsync(id, sponsor);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                await _sponsorService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        // Relationships
        [HttpGet("{id}/tournaments")]
        public async Task<ActionResult<IEnumerable<TournamentSponsorResponseDTO>>> GetTournaments(int id)
        {
            try
            {
                var list = await _sponsorService.GetTournamentsBySponsorAsync(id);
                var dto = _mapper.Map<IEnumerable<TournamentSponsorResponseDTO>>(list);
                return Ok(dto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/tournaments")]
        public async Task<ActionResult<TournamentSponsorResponseDTO>> AddToTournament(int id, TournamentSponsorRequestDTO dto)
        {
            try
            {
                var tsRequest = _mapper.Map<TournamentSponsor>(dto);
                var created = await _sponsorService.AddSponsorToTournamentAsync(id, tsRequest);
                var response = _mapper.Map<TournamentSponsorResponseDTO>(created);
                return CreatedAtAction(nameof(GetTournaments), new { id = id }, response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}/tournaments/{tid}")]
        public async Task<ActionResult> RemoveFromTournament(int id, int tid)
        {
            try
            {
                await _sponsorService.RemoveSponsorFromTournamentAsync(id, tid);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
