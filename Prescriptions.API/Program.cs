using Microsoft.EntityFrameworkCore;
using Prescriptions.API.Data;
using Prescriptions.API.Entities;
using Prescriptions.API.Services;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseInMemoryDatabase("PrescriptionsDev"));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("PrescriptionsDb")));
}

builder.Services.AddScoped<IPrescriptionService, PrescriptionService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Doctors.Any())
    {
        db.Doctors.Add(new Doctor { IdDoctor = 1, FirstName = "Gregory", LastName = "House", Email = "house@example.com" });
    }
    if (!db.Medicaments.Any())
    {
        db.Medicaments.Add(new Medicament { IdMedicament = 1, Name = "Paracetamol", Description = "Antipyretic", Type = "Analgesic" });
    }
    db.SaveChanges();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();