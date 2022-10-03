
using Library.Main.AuthorRepository;
using Library.Main.BillRepository;
using Library.Main.BookRepository;
using Library.Main.CustomerRepository;
using Library.Main.PublishingHouseRepository;
using Library.Models.Security;
using Library.Security.AccountRepository;
using Library.Shared.CategoryRepository;
using Library.Shared.DocumentRepository;
using Library.SQL.Context;
using Library.SQL.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
#region - Main -
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IBookRepository, BookRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IPublishingHouserepository, PublishingHouseRepository>();
builder.Services.AddScoped<IBillRepository, BillRepository>();
#endregion

#region - Shared -
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
#endregion

#region - Security -
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
#endregion

builder.Services.AddDbContext<LibraryDBContext>
                (options =>
                {
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
                });

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    //options.User.RequireUniqueEmail = true;
    options.Password.RequiredLength = 4;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
}).AddEntityFrameworkStores<LibraryDBContext>().AddDefaultTokenProviders();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v3.0", new OpenApiInfo()
    { Title = "Library API", Version = "v3.0" });
    //options.IncludeXmlComments("Library.API.xml");
    options.CustomSchemaIds(x => x.FullName);
    // Defining the security schema

    var securitySchema = new OpenApiSecurityScheme()
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    // Adding the bearer token authentaction option to the ui
    options.AddSecurityDefinition("Bearer", securitySchema);

    // use the token provided with the endpoints call
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<LibraryDBContext>();
    await context.Database.MigrateAsync();
    await SecurityDataSeed.InitializeAsync(services);
}
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v3.0/swagger.json", "Versioned API v3.0");
        c.DocExpansion(DocExpansion.None);
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseStaticFiles();

app.Run();
