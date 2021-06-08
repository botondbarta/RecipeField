using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RecipeField.DAL;
using System.Linq;
using System.Net.Http;
using Hellang.Middleware.ProblemDetails;
using Hellang.Middleware.ProblemDetails.Mvc;
using Microsoft.AspNetCore.Http;
using RecipeField.DAL.Entities;
using RecipeField.BLL.ServiceInterfaces;
using RecipeField.BLL.Services;
using RecipeField.BLL.Config;
using RecipeField.BLL.Exceptions;

namespace RecipeField.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<RecipeFieldDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<RecipeFieldDbContext>();

            services.AddIdentityServer()
                .AddApiAuthorization<User, RecipeFieldDbContext>();

            services.AddAutoMapper(typeof(MapProfile));

            services.AddProblemDetails(ConfigureProblemDetails)
                .AddControllers()
                // Adds MVC conventions to work better with the ProblemDetails middleware.
                .AddProblemDetailsConventions()
                .AddJsonOptions(x => x.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddScoped<IRecipeService, RecipeService>()
                .AddScoped<IUserService, UserService>()
                .AddScoped<IIngredientService, IngredientService>()
                .AddScoped<ICommentService, CommentService>()
                .AddScoped<ICategoryService, CategoryService>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
            });
            services.AddControllersWithViews();
            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseProblemDetails();

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseIdentityServer();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }

        // borrowed from: https://github.com/khellang/Middleware
        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            options.MapToStatusCode<NotValidParametersException>(StatusCodes.Status400BadRequest);
            
            options.MapToStatusCode<DbEntityNotFoundException>(StatusCodes.Status404NotFound);
            
            options.MapToStatusCode<InvalidLoginException>(StatusCodes.Status406NotAcceptable);

            options.MapToStatusCode<InvalidRecipeModificationException>(StatusCodes.Status403Forbidden);

            options.MapToStatusCode<InvalidCommentModificationException>(StatusCodes.Status403Forbidden);

            options.MapToStatusCode<NotEmptyCategoryException>(StatusCodes.Status403Forbidden);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }
    }
}
