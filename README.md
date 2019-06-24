# Headless Chrome

* [使用 ASP.NET Core 快速打造 Windows Service - 以 Headless Chrome 網頁抓圖為例](https://blog.darkthread.net/blog/aspnetcore-run-as-service/)
* [Getting Started with Headless Chrome  |  Web  |  Google Developers](https://developers.google.com/web/updates/2017/04/headless-chrome)

```bash
$ dotnet publish HeadlessChrome.WindowsService.csproj -c Release -f netcoreapp2.2 -r win-x64 -o C:\Publish\HeadlessChrome.WindowsService
$ sc create WebSnapshotService binPath="C:\Publish\HeadlessChrome.WindowsService\HeadlessChrome.WindowsService.exe"
$ sc start WebSnapshotService
```