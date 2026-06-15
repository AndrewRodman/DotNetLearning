using TaskApp.Services;
using TaskWeb.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

// needed for WebSessionContext to read/write session
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

// your ISessionContext impl (replaces MAUI SessionContext)
builder.Services.AddScoped<ISessionContext, WebSessionContext>();

// read URL from config instead of hardcoding (see below)
var apiBaseUrl = builder.Configuration["ApiBaseUrl"]
    ?? throw new InvalidOperationException("ApiBaseUrl is not configured.");

builder.Services.AddHttpClient<ITaskApiService, TaskApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

var app = builder.Build();

// middleware pipeline - order matters
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();          // you had AddSession but not UseSession
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages().WithStaticAssets();

app.Run();
