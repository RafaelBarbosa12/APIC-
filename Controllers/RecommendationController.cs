// APIC#/Controllers/RecommendationController.cs
using Microsoft.AspNetCore.Mvc;
using RecommendationApi.Models;
using RecommendationApi.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging; // Adicione para logging

namespace RecommendationApi.Controllers
{
    [ApiController]
    [Route("api/recommendation")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;
        private readonly ILogger<RecommendationController> _logger; // Adicione logger

        public RecommendationController(RecommendationService recommendationService, ILogger<RecommendationController> logger)
        {
            _recommendationService = recommendationService;
            _logger = logger; // Inicialize logger
        }

        [HttpPost("profile")]
        public async Task<ActionResult<RecommendationOutput>> GetProfileRecommendationAsync([FromBody] PortfolioDataInput portfolioInput)
        {
            // Validação: Garante que os campos essenciais são fornecidos
            if (portfolioInput == null || portfolioInput.Assets == null || !portfolioInput.Assets.Any() || 
                string.IsNullOrEmpty(portfolioInput.UserId) || string.IsNullOrEmpty(portfolioInput.CurrentProfile))
            {
                _logger.LogWarning("Requisição inválida para /api/recommendation/profile. Faltam userId, currentProfile ou assets.");
                return BadRequest("UserId, CurrentProfile e Assets são obrigatórios.");
            }

            try
            {
                _logger.LogInformation("Requisição recebida para /api/recommendation/profile para UserId: {UserId}", portfolioInput.UserId);
                var recommendation = await _recommendationService.GetRecommendationAsync(portfolioInput);
                return Ok(recommendation);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Erro na lógica de negócio ao processar recomendação para UserId: {UserId}", portfolioInput.UserId);
                return StatusCode(500, $"Erro interno ao processar a recomendação: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado no controller ao processar recomendação para UserId: {UserId}", portfolioInput.UserId);
                return StatusCode(500, "Ocorreu um erro inesperado ao processar a solicitação.");
            }
        }

        // Remova ou comente outros endpoints se eles não forem mais relevantes para o fluxo principal
        // Ex: [HttpGet("assets/{contaId}")] e [HttpGet("health")]
        // Se você ainda quiser um health check, mova-o para um controller separado ou mantenha-o aqui se preferir.
    }
}