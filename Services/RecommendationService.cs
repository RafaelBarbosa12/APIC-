// APIC#/Services/RecommendationService.cs
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RecommendationApi.Models;
using System.Linq;

namespace RecommendationApi.Services
{
    public class RecommendationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<RecommendationService> _logger;
        private readonly string _pythonApiBaseUrl;

        public RecommendationService(HttpClient httpClient, IConfiguration configuration, ILogger<RecommendationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _pythonApiBaseUrl = configuration["PythonApi:BaseUrl"] ?? "http://10.254.17.97:5000";
        }

        public async Task<RecommendationOutput> GetRecommendationAsync(PortfolioDataInput portfolioInput)
        {
            try
            {
                // Converte a entrada para o formato esperado pela API Python
                var pythonApiRequest = new
                {
                    userId = portfolioInput.UserId,                // <--- ENVIANDO UserId
                    currentProfile = portfolioInput.CurrentProfile, // <--- ENVIANDO CurrentProfile
                    assets = portfolioInput.Assets.Select(a => new
                    {
                        assetType = a.AssetType,
                        financialValue = a.FinancialValue
                    }).ToList()
                };

                var json = JsonSerializer.Serialize(pythonApiRequest, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Para garantir camelCase no JSON
                });

                var content = new StringContent(json, Encoding.UTF8, "application/json");

                _logger.LogInformation("Enviando requisição POST para API Python: {Url}", $"{_pythonApiBaseUrl}/api/recommendation/profile");
                _logger.LogDebug("Corpo da requisição para Python: {JsonBody}", json);

                var response = await _httpClient.PostAsync($"{_pythonApiBaseUrl}/api/recommendation/profile", content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Erro na API Python. Status: {StatusCode}, Content: {Content}",
                        response.StatusCode, errorContent);
                    throw new InvalidOperationException($"Erro na API Python: {response.StatusCode} - {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation("Resposta recebida da API Python: {Response}", responseContent);

                var pythonResponse = JsonSerializer.Deserialize<PythonApiResponse>(responseContent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Para deserializar camelCase do JSON
                });

                // Mapeia a resposta da API Python para o Output da API C#
                return new RecommendationOutput
                {
                    CalculatedProfile = pythonResponse.CalculatedProfile, // <--- CAMPO RENOMEADO
                    PortfolioBreakdown = pythonResponse.PortfolioBreakdown,
                    AssetRecommendations = pythonResponse.AssetRecommendations // Se nulo, será nulo no C# também
                };
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Erro de conexão com a API Python");
                throw new InvalidOperationException("Erro de conexão com o serviço de recomendação", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Erro ao processar resposta JSON da API Python: {Message}. Raw response: {RawResponse}", ex.Message, ex.Message);
                throw new InvalidOperationException($"Erro ao processar resposta do serviço de recomendação: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao obter recomendação");
                throw;
            }
        }

        public async Task<bool> CheckHealthAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{_pythonApiBaseUrl}/health");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}