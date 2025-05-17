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
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DataContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddHostedService<NotificationCleanupService>();

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
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chatHub");
using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;

//Tutaj seed potem!

app.Run();
