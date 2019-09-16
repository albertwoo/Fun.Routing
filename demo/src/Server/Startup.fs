module Server.Startup

open System
open System.IO
open Microsoft.Net.Http.Headers
open Microsoft.AspNetCore
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Rewrite
open Microsoft.AspNetCore.ResponseCompression
open Microsoft.Extensions.DependencyInjection
open Microsoft.Extensions.Primitives
open Microsoft.Extensions.Configuration
open Giraffe


let publicPath = 
#if DEBUG
  Path.GetFullPath "../Client/public"
#else
  "wwwroot"
#endif

let staticFileOptions =
  StaticFileOptions(
    OnPrepareResponse = fun ctx ->
      ctx.Context.Response.Headers.[HeaderNames.CacheControl] <- StringValues (sprintf "public,max-age=%d" (60*60*24*30))
  )

let configureServices (config: IConfiguration) (services : IServiceCollection) =
  services
    .AddGiraffe()
    .AddResponseCompression(fun c ->
      c.Providers.Add<BrotliCompressionProvider>()
      c.Providers.Add<GzipCompressionProvider>()
      c.EnableForHttps <- true
      c.MimeTypes <-
        [|
           "text/plain"
           "text/css"
           "text/html"
           "text/xml"
           "text/json"
           "application/javascript"
           "image/png"
           "image/jpeg"
           "image/svg+xml"
           "font/woff2"
        |])
    .AddResponseCaching(fun c -> c.UseCaseSensitivePaths <- true)
  |> ignore


let configureApp (app : IApplicationBuilder) =
  app
     .UseResponseCompression()
     //.UseDefaultFiles() // disable it for SSR support
     .UseStaticFiles(staticFileOptions)
     .UseResponseCaching()
     .UseGiraffe Routes.mainRoutes


[<EntryPoint>]
let main args =
  let config =
    ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json", optional = false, reloadOnChange = true)
        .AddJsonFile("appsettings.Development.json", optional = true, reloadOnChange = true)
        .AddJsonFile("appsettings.Production.json", optional = true, reloadOnChange = true)
        .AddCommandLine(args)
        .Build()
  WebHost
    .CreateDefaultBuilder()
    .CaptureStartupErrors(true)
    .UseConfiguration(config)
    .UseWebRoot(publicPath)
    .Configure(Action<IApplicationBuilder> configureApp)
    .ConfigureServices(configureServices config)
    .UseIISIntegration()
    .Build()
    .Run()
  1
