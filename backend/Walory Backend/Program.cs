using Application.CQRS.FriendsLogic;
using Application.Services;
using Domain;
using Infrastracture;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Walory_Backend;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.SignIn.RequireConfirmedEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<DataContext>()
.AddDefaultTokenProviders();


builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";
    options.AccessDeniedPath = "/auth/denied";
    options.Cookie.Name = "Walory";
    options.ExpireTimeSpan = TimeSpan.FromDays(7);
    options.SlidingExpiration = true;
});
builder.Services.AddAuthorization();
builder.Services.AddMediatR(cfg =>
cfg.RegisterServicesFromAssembly(typeof(SendFriendRequest.Handler).Assembly));
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost", "http://localhost:80", "http://localhost:5173", "http://20.67.251.224/","http://walory.northeurope.cloudapp.azure.com")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHostedService<NotificationCleanupService>();

ServicePointManager.ServerCertificateValidationCallback =
    delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }; // Ryzykowne 
builder.Services
    .AddFluentEmail(
        builder.Configuration["EmailSettings:FromEmail"],
        builder.Configuration["EmailSettings:FromName"])
    .AddSmtpSender(() => new SmtpClient(
        builder.Configuration["EmailSettings:SmtpHost"],
        int.Parse(builder.Configuration["EmailSettings:SmtpPort"]))
    {
        Credentials = new NetworkCredential(
            builder.Configuration["EmailSettings:SmtpUser"],
            builder.Configuration["EmailSettings:SmtpPass"]),
        EnableSsl = true
    }

    );

builder.Services.AddScoped<EmailService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment()) //Yes I want swager on production right now
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}

app.UseCors("AllowFrontend");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    db.Database.Migrate();

    await Seed.SeedData(db,userManager);
}

app.Run();

public partial class Program { }