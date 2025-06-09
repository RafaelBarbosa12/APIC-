using RecommendationApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Adicionar serviços ao contêiner de injeção de dependência.
builder.Services.AddControllers();

// Registrar o RecommendationService para injeção de dependência
builder.Services.AddScoped<RecommendationService>();

// Se RecommendationService usa HttpClient, você pode usar AddHttpClient
builder.Services.AddHttpClient<RecommendationService>();

// Configuração para documentação Swagger/OpenAPI (opcional)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar o pipeline de requisições HTTP.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// app.UseAuthorization();

app.MapControllers();

app.Run();
