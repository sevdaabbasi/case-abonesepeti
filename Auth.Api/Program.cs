using Auth.Api.Repositories;
using Auth.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Mongo ayarlarını oku validate et
var mongoSettings = builder.Configuration.GetSection("MongoSettings").Get<MongoSettings>();
if (mongoSettings == null)
{
    throw new InvalidOperationException("MongoSettings yapılandırması bulunamadı!");
}

// DI
builder.Services.AddSingleton(mongoSettings);
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<AuthService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting(); 
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();

app.Run();