using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using QaToDoApp.Data;
using QaToDoApp.Repository;

namespace QaToDoApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ToDoDbContext>(opt =>
                opt.UseInMemoryDatabase("ToDoList"));
            services.AddControllers().AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
            services.AddScoped<IToDoItemRepository, ToDoItemRepository>();
            services.AddAutoMapper(cfg => { }, typeof(MappingConfig));
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "QaToDoApp", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy("AllowOrigins", policyBuilder =>
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .WithExposedHeaders("Content-Disposition"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "QaToDoApp v1"));
            }

            app.UseHttpsRedirection();

            app.UseCors("AllowOrigins");

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}