using Microsoft.AspNetCore.Authorization;
using OpenApiNet9.Explorer;
using OpenApiNet9.Todos;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(options =>
{
    options.AddOperationTransformer(((operation, context, cancellationToken) =>
    {
        return Task.CompletedTask;
    }));
});

builder.Services.AddAuthentication()
    .AddBearerToken();
builder.Services.AddAuthorizationBuilder()
    .AddDefaultPolicy("default", policy =>
    {
        policy.RequireAuthenticatedUser(); 
        policy.AddAuthenticationSchemes("Bearer");
    })
    .AddPolicy("assertion", p => p.RequireAssertion(context => true));

var app = builder.Build();

app.UseRouting();

app.Use((context, next) =>
{
    var endpoint = context.GetEndpoint();

    endpoint.Metadata.OfType<IAuthorizeData>();


    return next();
});

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapTodos();
app.MapExplorer();

app.MapScalarApiReference();

app.Run();