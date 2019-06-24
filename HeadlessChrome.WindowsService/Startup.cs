using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace HeadlessChrome.WindowsService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.Run(async (context) =>
            {
                Process process = null;
                context.Response.ContentType = "text/plain";
                try
                {
                    var url = context.Request.Query["u"];
                    if (string.IsNullOrEmpty(url))
                        throw new ApplicationException("missing parameter - u");
                    if (!int.TryParse(context.Request.Query["w"].FirstOrDefault(), out var width))
                        width = 1024;
                    if (!int.TryParse(context.Request.Query["h"].FirstOrDefault(), out var height))
                        height = 768;
                    var tempFile = Path.GetTempFileName() + ".png";
                    var startInfo = new ProcessStartInfo()
                    {
                        CreateNoWindow = true,
                        FileName = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe",
                        Arguments =
                            $"--headless --disable-gpu --window-size={width},{height} --screenshot={tempFile} {url}",
                        UseShellExecute = false,
                        RedirectStandardError = true
                    };

                    process = Process.Start(startInfo);
                    process.WaitForExit(5000);

                    if (File.Exists(tempFile))
                    {
                        var img = File.ReadAllBytes(tempFile);
                        File.Delete(tempFile);
                        context.Response.ContentType = "image/png";
                        await context.Response.Body.WriteAsync(img, 0, img.Length);
                    }
                    else await context.Response.WriteAsync("Failed");
                }
                catch (Exception ex)
                {
                    if (process != null)
                    {
                        try
                        {
                            //若Process仍存在，清除之
                            Process.GetProcessById(process.Id);
                            process.Kill();
                        }
                        catch
                        {
                            // ignored
                        }
                    }
                    await context.Response.WriteAsync(ex.Message);
                }
            });
        }
    }
}