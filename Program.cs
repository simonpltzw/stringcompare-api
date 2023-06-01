using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using StringCompare.API.Models;
using StringCompare.API.Validators;
using Swashbuckle.AspNetCore.Annotations;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.Seq("https://upload.seq.simon-paltzow.de")
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c => 
    {
        c.SwaggerDoc("1.0.0", new Microsoft.OpenApi.Models.OpenApiInfo
        {
            Version = "1.0.0",
            Title = "StringCompare-API",
            Description = "WebAPI for comparing texts using <a href=\"https://github.com/google/diff-match-patch\" target=\"_blank\">google diff-match-patch</a>",
        });

        c.EnableAnnotations();
    });

    builder.Services.AddScoped<IValidator<DiffRequest>, DiffRequestValidator>();

    builder.Services.AddHealthChecks();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {

    }

    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("1.0.0/swagger.json", "StringCompare-API"));

    app.UseHttpsRedirection();

    app.UseHealthChecks("/health");

    app.MapPost("/findDifferences",
        [SwaggerOperation(Summary = "Find differences between two texts.", Description = "Categorizes differences between two texts in EQUAL, INSERT and DELETE.", OperationId = "FindDifferences")]
        [SwaggerResponse(200, "Differences were found.", typeof(List<DiffResponseElement>))]
        [SwaggerResponse(400, "Failed to validate the json input.")]
        async (IValidator<DiffRequest> validator, [FromBody, SwaggerParameter("The text payload.", Required = true)] DiffRequest request) =>
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var diffResult = new DiffMatchPatch.diff_match_patch().diff_main(request.Text1!, request.Text2!);
            List<DiffResponseElement> diffResponse = new();
            diffResult.ForEach(x =>
            {
                diffResponse.Add(new DiffResponseElement
                {
                    Operation = x.operation.ToString(),
                    Text = x.text
                });
            });
            return Results.Ok(diffResponse);
        }
    );

    app.Run();
}
catch (Exception)
{
    throw;
}
finally
{
    // Important to call at exit so that batched events are flushed.
    Log.CloseAndFlush();
}