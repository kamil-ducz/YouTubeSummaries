using YouTubeSummaries;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddCors();

builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient<ISummaryService, SummaryService>();
builder.Services.AddHttpClient<ITranscriptService, ExternalTranscriptService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(
    options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()
    );

app.UseAuthorization();

app.MapControllers();

app.Run();
