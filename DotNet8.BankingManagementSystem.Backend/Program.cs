var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        config =>
        {
            config
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
            //.AllowCredentials();
        });
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(
    opt => { opt.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")); }, ServiceLifetime.Transient,
    ServiceLifetime.Transient);

#region Register Services

builder.Services.AddScoped<StateService>();
builder.Services.AddScoped<TownshipService>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<AccountService>();
builder.Services.AddScoped<AdminUserService>();
builder.Services.AddScoped<TransactionService>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

#endregion


//builder.Services
//    .AddRefitClient<IStateApi>()
//    .ConfigureHttpClient(c => c.BaseAddress = new Uri(builder.Configuration.GetSection("ApiUrl").Value!));

//builder.Services.AddRefitService<IStateApi>(builder.Configuration);
//builder.Services.AddRefitService<ITownshipApi>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseSession();
app.UseCors("AllowAll");

app.MapControllers();

app.Run();