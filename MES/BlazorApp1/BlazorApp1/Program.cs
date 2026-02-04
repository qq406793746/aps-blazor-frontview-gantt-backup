using BlazorApp1.Data;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Syncfusion.Blazor;
using Syncfusion.Licensing;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddAuthorizationCore();
builder.Services.AddSingleton<WeatherForecastService>();
builder.Services.AddSingleton<AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.Configure<ApsApiOptions>(builder.Configuration.GetSection("ApsApi"));
builder.Services.AddTransient<TraceIdHandler>();
builder.Services.AddHttpClient<IApsApiService, ApsApiService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApsApiOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        client.BaseAddress = new Uri(options.BaseUrl);
    }
})
.AddHttpMessageHandler<TraceIdHandler>();
builder.Services.AddHttpClient<IGanttApiClient, GanttApiClient>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApsApiOptions>>().Value;
    if (!string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        client.BaseAddress = new Uri(options.BaseUrl);
    }
})
.AddHttpMessageHandler<TraceIdHandler>();

// 从配置中获取数据库连接字符串，并使用默认值作为备用
var connectionString = builder.Configuration.GetConnectionString("MESProjectManagement") 
    ?? @"Data Source=LAPTOP-TVH0HQJJ\SYSTEM3R;Persist Security Info=True;User ID=Sys3RClient;Password=Power3Rmad;Encrypt=True;Trust Server Certificate=True";

builder.Services.AddScoped<ProjectManagementService>(_ => new ProjectManagementService(connectionString));
builder.Services.AddScoped<BOMManagementService>(_ => new BOMManagementService(connectionString));
builder.Services.AddScoped<ProcessManagementService>(_ => new ProcessManagementService(connectionString));
builder.Services.AddScoped<EquipmentManagementService>(_ => new EquipmentManagementService(connectionString));

var syncfusionLicenseKey = builder.Configuration["Syncfusion:LicenseKey"];
if (!string.IsNullOrWhiteSpace(syncfusionLicenseKey))
{
    SyncfusionLicenseProvider.RegisterLicense(syncfusionLicenseKey);
}

var app = builder.Build();

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



