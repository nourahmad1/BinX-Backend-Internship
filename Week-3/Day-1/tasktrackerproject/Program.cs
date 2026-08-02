using TaskTrackerApi.Data;

var builder = WebApplication.CreateBuilder(args);


// Add MVC Controllers
builder.Services.AddControllers();


// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI();
}


// Disable for now because we use HTTP
// app.UseHttpsRedirection();



app.MapControllers();



app.Run();