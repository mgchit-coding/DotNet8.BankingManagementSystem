var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddRefitService<IStateApi>(builder.Configuration);
builder.Services.AddRefitService<IUserApi>(builder.Configuration);
builder.Services.AddRefitService<ITownshipApi>(builder.Configuration);
builder.Services.AddRefitService<IAccountApi>(builder.Configuration);
builder.Services.AddRefitService<IAdminUser>(builder.Configuration);
builder.Services.AddRefitService<ITransactionApi>(builder.Configuration);
builder.Services.AddScoped<InjectService>();

await builder.Build().RunAsync();