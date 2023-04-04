using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Configuration.AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSIDWebsite(action: o =>
{
    o.UseSqlServer(builder.Configuration.GetConnectionString("IdServer"),  o =>
    {
        o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
