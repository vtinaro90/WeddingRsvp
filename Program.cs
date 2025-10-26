using System.Net.Mail;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
       policy.WithOrigins("https://christina-vincent.com").AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

app.UseCors();

app.MapPost("/api/rsvp", async (RsvpData rsvp) =>
{
    var config = app.Services.GetRequiredService<IConfiguration>();
    var email = config["Rsvp:Email"];
    var password = config["Rsvp:Password"];

    try
    {
        var smtp = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new System.Net.NetworkCredential(email, password),
            EnableSsl = true,
        };

        string body = $@"
            New Wedding RSVP:
            Name: {rsvp.Name}
            Email: {rsvp.Email}
            Attending: {rsvp.Attending}
            Guests: {rsvp.Guests}
            Message: {rsvp.Message}
        ";

        await smtp.SendMailAsync(email, email, "New Wedding RSVP", body);

        return Results.Ok(new { message = "RSVP received!" });
    }
    catch (Exception ex)
    {
        return Results.Problem("Failed to send RSVP: " + ex.Message);
    }
});

app.Run();

record RsvpData(string Name, string Email, string Attending, int Guests, string Message);