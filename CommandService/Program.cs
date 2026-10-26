namespace CommandService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }

            app.MapPost("/api/commands/ping", async (
                CancellationToken cancellationToken) =>
            {
                Console.WriteLine("CommandSerivce reached...");
                return Results.Ok("Pong");

            }).ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .Produces<int>(StatusCodes.Status201Created);

            // app.UseHttpsRedirection();

            app.Run();
        }
    }
}
