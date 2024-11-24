using ElectronNET.API;
using ElectronNET.API.Entities;
using Humanizer;
using NuGet.ContentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using SocketIOClient.Transport;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore;
using System;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Rewrite;

var builder = WebApplication.CreateBuilder(args);
//var builder = WebApplication.CreateBuilder(new WebApplicationOptions
//{
//    ContentRootPath = Directory.GetCurrentDirectory(),
//    WebRootPath = "wwwroot"
//});

// Add appsettings.json configuration file
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Access configuration and environment
var configuration = builder.Configuration;
var environment = builder.Environment;

var _Env = configuration["Environment"];
var _AppName = configuration["AppName"];

// Remove all CORS and security settings for testing purposes
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowCorsPolicy", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

// Add services to the container (includes MVC)
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllersWithViews();  // Enable MVC controllers and views
builder.Services.AddElectron();  // Add Electron support

// Use Electron with ASP.NET Core
builder.WebHost.UseElectron(args);


// Specify the URLs for both HTTP and HTTPS
builder.WebHost.UseUrls("http://localhost:5001");  // Update to use port 5001

// Build the app
var app = builder.Build();

System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

// Enable CORS with the previously defined policy
app.UseCors("AllowCorsPolicy");  // Apply CORS policy

// /////////////////////////////////////////////////////////////////////////////
// Author: william.sergio2@redross.org
// /////////////////////////////////////////////////////////////////////////////
// Default Content Type Provider: The FileExtensionContentTypeProvider
// already knows how to handle common file types like .css and .js.
// By creating a default provider and then modifying its mappings,
// ***YOU EXTEND THE DEFAULT BEHAVIOR INSTEAD OF REPLACING IT.***
// Custom Mappings: You only need to add or modify mappings for the 
// file types that require special handling (e.g., .apk, .ipa, .plist).
var defaultFileProvider = new FileExtensionContentTypeProvider();
// Add custom MIME type mappings
defaultFileProvider.Mappings[".apk"] = "application/vnd.android.package-archive";
defaultFileProvider.Mappings[".ipa"] = "application/octet-stream";
defaultFileProvider.Mappings[".plist"] = "text/xml";
// Video file types
defaultFileProvider.Mappings[".mp4"] = "video/mp4";
defaultFileProvider.Mappings[".avi"] = "video/x-msvideo";
defaultFileProvider.Mappings[".mov"] = "video/quicktime";
defaultFileProvider.Mappings[".wmv"] = "video/x-ms-wmv";
defaultFileProvider.Mappings[".flv"] = "video/x-flv";
defaultFileProvider.Mappings[".mkv"] = "video/x-matroska";
defaultFileProvider.Mappings[".webm"] = "video/webm";
// Audio file types
defaultFileProvider.Mappings[".mp3"] = "audio/mpeg";
defaultFileProvider.Mappings[".wav"] = "audio/wav";
defaultFileProvider.Mappings[".ogg"] = "audio/ogg";
defaultFileProvider.Mappings[".flac"] = "audio/flac";
// Document file types
defaultFileProvider.Mappings[".pdf"] = "application/pdf";
defaultFileProvider.Mappings[".doc"] = "application/msword";
defaultFileProvider.Mappings[".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
defaultFileProvider.Mappings[".xls"] = "application/vnd.ms-excel";
defaultFileProvider.Mappings[".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
defaultFileProvider.Mappings[".ppt"] = "application/vnd.ms-powerpoint";
defaultFileProvider.Mappings[".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
// Image file types
defaultFileProvider.Mappings[".jpg"] = "image/jpeg";
defaultFileProvider.Mappings[".jpeg"] = "image/jpeg";
defaultFileProvider.Mappings[".png"] = "image/png";
defaultFileProvider.Mappings[".gif"] = "image/gif";
defaultFileProvider.Mappings[".bmp"] = "image/bmp";
defaultFileProvider.Mappings[".svg"] = "image/svg+xml";
// Archive file types
defaultFileProvider.Mappings[".zip"] = "application/zip";
defaultFileProvider.Mappings[".rar"] = "application/x-rar-compressed";
defaultFileProvider.Mappings[".7z"] = "application/x-7z-compressed";
defaultFileProvider.Mappings[".tar"] = "application/x-tar";
defaultFileProvider.Mappings[".gz"] = "application/gzip";
// Other common file types
defaultFileProvider.Mappings[".json"] = "application/json";
defaultFileProvider.Mappings[".xml"] = "application/xml";
defaultFileProvider.Mappings[".csv"] = "text/csv";
defaultFileProvider.Mappings[".txt"] = "text/plain";
// //////////////////////////////////////////////////////////////////////
// Use static files to serve resources (CSS, JS, images)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = "",
    ContentTypeProvider = defaultFileProvider,
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24; // 1 day
        ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = "public,max-age=" + durationInSeconds;
    }
});

// ///////////////////////////////////////////////////////////////////////////
// Author: william.sergio2@redcross.org
// InfoSec requirement
// ///////////////////////////////////////////////////////////////////////////
// This code simplifies the management of Content-Security-Policy (CSP) strings
// by programmatically generating the policy based on a predefined list of
// allowed domains. By consolidating the domains into an array, users can
// easily add or remove domains from the CSP without manually modifying the
// intricate syntax of the CSP string. This approach reduces the risk of
// syntax errors and ensures consistency across different directives.
// Additionally, it dynamically includes a nonce for scripts, further enhancing
// security by allowing only scripts with the matching nonce to execute. This
// makes CSP management more flexible and less error-prone, especially in
// complex applications.
// ///////////////////////////////////////////////////////////////////////////
app.Use(async (context, next) =>
{
    string scriptNonce;
    using (var rng = RandomNumberGenerator.Create())
    {
        var nonceBytes = new byte[32];
        rng.GetBytes(nonceBytes);
        scriptNonce = Convert.ToBase64String(nonceBytes);
    }

    context.Items["ScriptNonce"] = scriptNonce;

    // Add domains here specific for your application!!!
    string[] domains = new string[]
    {
        "https://www.googletagmanager.com",
        "https://www.google-analytics.com",
        "https://fonts.googleapis.com",
        "https://fonts.gstatic.com",
        "https://cdn.datatables.net",
        "https://cdnjs.cloudflare.com",
        "https://cdn.jsdelivr.net",
        "https://stackpath.bootstrapcdn.com",
        "https://code.jquery.com",
        "https://ajax.aspnetcdn.com",
        "https://www.adobe.com",
        "https://adobe.com",
        "https://ainetprofit.com",
        "https://ainetprofits.com",
        "https://sergioapps.com",
        "https://maxlifespan.com",
        "https://mdhealthbuzz.com",
        "https://software-rus.com",
        "https://stationbreaktv.com",
        "https://localhost:*",
        "wss://localhost:*"
    };

    string scriptSrcDomains = string.Join(" ", domains);

    string csp = $"default-src 'self'; " +
                 $"script-src 'self' 'nonce-{scriptNonce}' 'strict-dynamic' {scriptSrcDomains}; " +
                 $"font-src 'self' https://fonts.gstatic.com {scriptSrcDomains}; " +
                 $"img-src 'self' data: blob: {scriptSrcDomains}; " +
                 $"object-src 'none'; " +
                 $"media-src 'self' blob: {scriptSrcDomains}; " +
                 $"connect-src {scriptSrcDomains} ws://localhost:* https://localhost:* http://localhost:*; " +
                 $"style-src 'self' 'unsafe-inline' {scriptSrcDomains}; " +
                 $"frame-ancestors 'self'; " +
                 $"form-action 'self' {scriptSrcDomains} https://localhost:5001;";

    context.Response.Headers["Content-Security-Policy"] = "";

    await next();
});

// Enable developer exception page during development for better debugging
if (app.Environment.IsDevelopment())
{
    // Use Developer Exception Page for detailed error insights during development
    app.UseDeveloperExceptionPage();
}
else
{
    // Handle errors globally and return a generic error response
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// //////////////////////////////////////////////
// Author: william.sergio2@redcross.org
// CRITICAL .NET 7 & 8 BUG FIX !!!
// THIS IS A WELL-KNOWN BUG IN .NET 7
// FIXES BUG: https://localhost:5001/index.html
// MUST GO BEFORE app.UseRouting();
// //////////////////////////////////////////////
var rewrite = new RewriteOptions()
    .AddRedirect("^index\\.html$", "Home/Index");
app.UseRewriter(rewrite);
// //////////////////////////////////////////////







// Setup routing
app.UseRouting();  // Enable routing in the application

// Set up routes (adjust if you need MVC or custom routes)
app.UseEndpoints(endpoints =>
{
    // Default route(s) for MVC.
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Company}/{action=Index}/{id?}");
});

// Disable HTTPS redirection (for testing)
app.Use(async (context, next) =>
{
    // Force the use of HTTP scheme for all requests (unsafe, testing only)
    context.Request.Scheme = "http";
    await next();
});

// Only create the Electron window if Electron is active
if (HybridSupport.IsElectronActive)
{
    // Bootstrap Electron window setup
    await ElectronBootstrap();
}

// ElectronBootstrap method to configure and start Electron
async Task ElectronBootstrap()
{
    var appInstance = Electron.App;

    // Append command line switches
    appInstance.CommandLine.AppendSwitch("ignore-certificate-errors");
    appInstance.CommandLine.AppendSwitch("allow-insecure-localhost", "true");

    var preloadPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "wwwroot", "js", "preload.js").Replace("\\", "/");

    // Check if the file exists
    //if (!File.Exists(preloadPath))
    //{
    //    Console.WriteLine($"Preload script not found at {preloadPath}");
    //    return; // Exit if the file doesn't exist
    //}

    // Define the destination path by replacing the initial part of the path
    //var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "obj/Host/bin/wwwroot/js/preload.js");

    // Ensure the destination directory exists
    //Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));

    // Copy the file
    //File.Copy(preloadPath, destinationPath, overwrite: true);
    //Console.WriteLine($"Preload script copied to {destinationPath}");

    var menuManager = new MenuManager();
    var browserWindow = await menuManager.CreateWindow(new BrowserWindowOptions
    {
        Width = 1600,
        MinWidth = 800,
        Height = 1100,
        MinHeight = 500,
        HasShadow = false,
        Center = true,
        AutoHideMenuBar = false, // Make sure this is set to false to keep menu visible
        Frame = true,
        Transparent = false,
        Icon = "wwwroot/img/ai.ico",
        WebPreferences = new WebPreferences
        {
            NodeIntegration = false, // Keep this false for security
            ContextIsolation = true,
            Preload = preloadPath
        }
    });

    // Optionally, show the window when it's ready to be displayed
    browserWindow.OnReadyToShow += () => browserWindow.Show();

    // Open Developer Tools (Inspector)
    // browserWindow.WebContents.OpenDevTools();

    // Clear cache to prevent loading issues from previous sessions
    await browserWindow.WebContents.Session.ClearCacheAsync();

    // Set the title for the Electron window
    browserWindow.SetTitle("Net Profit");

    // Load the correct URL from ASP.NET Core (on port 5001)
    browserWindow.LoadURL("http://localhost:5001");  // Ensure the window loads from port 5001
}

// Start the app and wait for it to shut down
await app.StartAsync();  // Start the application
app.WaitForShutdown();  // Wait for the app to shut down when it's finished
