// APIC#/Models/RecommendationModels.cs
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RecommendationApi.Models
{
    public class PortfolioDataInput
    {
        public string UserId { get; set; }           // <--- NOME AJUSTADO: para ser mais genérico que 'ContaId'
        public string CurrentProfile { get; set; }   // <--- NOVO CAMPO: Perfil atual do investidor
        public List<AssetInputData> Assets { get; set; }
    }

    public class AssetInputData
    {
        public string AssetType { get; set; }
        public double FinancialValue { get; set; }
    }

    public class RecommendationOutput
    {
        // Renomeado para refletir o cálculo
        public string CalculatedProfile { get; set; }
        public PortfolioBreakdown PortfolioBreakdown { get; set; }
        public List<AssetRecommendation>? AssetRecommendations { get; set; } // Pode ser nulo
    }

    public class PortfolioBreakdown
    {
        [JsonPropertyName("RF")]
        public double RfPercentage { get; set; }

        [JsonPropertyName("RV")]
        public double RvPercentage { get; set; }

        [JsonPropertyName("OUTROS")]
        public double OutrosPercentage { get; set; }
    }

    public class AssetRecommendation
    {
        public string AssetName { get; set; }
        public int Score { get; set; }
    }

    // Modelo para deserializar a resposta bruta da API Python
    public class PythonApiResponse
    {
        // Renomeado para refletir o cálculo
        public string CalculatedProfile { get; set; } 
        public PortfolioBreakdown PortfolioBreakdown { get; set; }
        public List<AssetRecommendation>? AssetRecommendations { get; set; } // Pode ser nulo
    }
}