using Congratulator.Api.Abstractions;
using Congratulator.Api.BackgroundServices;
using Congratulator.Api.Data;
using Congratulator.Api.Notifications;
using Congratulator.Api.Options;
using Congratulator.Api.Repositories;
using Congratulator.Api.Services;
using Congratulator.Api.Storage;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Конфигурация (appsettings.json -> строго типизированные классы опций) ---
builder.Services.Configure<BirthdaySettings>(builder.Configuration.GetSection("Birthdays"));
builder.Services.Configure<NotificationSettings>(builder.Configuration.GetSection("Notifications"));

// --- Веб-слой ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Поздравлятор API",
        Version = "v1",
        Description = "API для ведения списка дней рождения друзей/коллег.",
    });
});

builder.Services.AddCors(options =>
{
    var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                          ?? new[] { "http://localhost:5173" };

    options.AddPolicy("Spa", policy => policy
        .WithOrigins(allowedOrigins)
        .AllowAnyHeader()
        .AllowAnyMethod());
});

// --- Данные ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// --- Время. TimeProvider.System инжектируется как абстракция вместо DateTime.Now,
//     чтобы расчёт "сегодня" был тестируемым (можно подменить FakeTimeProvider в тестах). ---
builder.Services.AddSingleton(TimeProvider.System);

// --- Прикладные сервисы (Dependency Inversion: контроллеры зависят от интерфейсов) ---
builder.Services.AddScoped<IPersonRepository, EfPersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddSingleton<IPhotoStorage, LocalPhotoStorage>();
builder.Services.AddScoped<IBirthdayNotificationService, BirthdayNotificationService>();

// Выбор транспорта рассылки: если SMTP не настроен — используем безопасную заглушку,
// чтобы проект запускался "из коробки" без реального почтового сервера.
var smtpHost = builder.Configuration["Notifications:Smtp:Host"];
if (!string.IsNullOrWhiteSpace(smtpHost))
{
    builder.Services.AddScoped<IMessageSender, SmtpMessageSender>();
}
else
{
    builder.Services.AddScoped<IMessageSender, ConsoleMessageSender>();
}

builder.Services.AddHostedService<BirthdayReminderHostedService>();

var app = builder.Build();

// --- Гарантируем существование БД и папки для фотографий ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

var uploadsPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "uploads");
Directory.CreateDirectory(uploadsPath);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "Поздравлятор API v1"));
}

// Глобальный обработчик ошибок: при любом необработанном исключении возвращаем JSON,
// а не HTML-страницу ошибки, которую SPA не умеет отображать.
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    ctx.Response.StatusCode = 500;
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { message = "Внутренняя ошибка сервера." });
}));

app.UseCors("Spa");
app.UseStaticFiles(); // отдаёт wwwroot/uploads/* как /uploads/*
app.UseAuthorization();
app.MapControllers();

app.Run();
