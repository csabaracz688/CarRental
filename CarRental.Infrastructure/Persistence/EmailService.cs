using CarRental.Application.Common.Interfaces;

namespace CarRental.Infrastructure.Services;

public class EmailService : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var folder = Path.Combine(Directory.GetCurrentDirectory(), "MockEmails");

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        var fileName = $"email_{DateTime.Now:yyyyMMdd_HHmmss}.html";
        var path = Path.Combine(folder, fileName);

        var content = $@"
            <h1>MOCK EMAIL</h1>
            <p><strong>To:</strong> {to}</p>
            <p><strong>Subject:</strong> {subject}</p>
            <hr />
            {body}
        ";

        await File.WriteAllTextAsync(path, content);

        Console.WriteLine($"Mock email saved: {path}");
    }
}