using Blazored.LocalStorage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBlazoredLocalStorage();   // local storage
builder.Services.AddBlazoredLocalStorage(config =>
    config.JsonSerializerOptions.WriteIndented = true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpContextAccessor()
    .AddHttpClient("ServerAPI", client =>
    {
        client.BaseAddress = new Uri("https://localhost:44360/api/");
        client.DefaultRequestHeaders.Add("Accept", "application/json");
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();