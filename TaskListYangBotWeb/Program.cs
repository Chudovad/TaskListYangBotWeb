using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net;
using System.Text;
using TaskListYangBotWeb.Data;
using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Handlers;
using TaskListYangBotWeb.Handlers.Callbacks;
using TaskListYangBotWeb.Handlers.Commands;
using TaskListYangBotWeb.Handlers.KeyboardCommands;
using TaskListYangBotWeb.Handlers.Replies;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services;
using TaskListYangBotWeb.Services.Interfaces;

namespace TaskListYangBotWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ��������� �������� ApplicationContext � �������� ������� � ����������
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            SetStaticFields(builder);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<TelegramBotService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFavoriteTaskRepository, FavoriteTaskRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();
            builder.Services.AddScoped<IUserWebRepository, UserWebRepository>();
            builder.Services.AddScoped<IRoleRepository, RoleRepository>();
            builder.Services.AddScoped<IFavoriteEnvironmentRepository, FavoriteEnvironmentRepository>();

            builder.Services.AddScoped<ICommandExecutor, CommandExecutor>();
            builder.Services.AddScoped<BaseHandler, StartCommand>();
            builder.Services.AddScoped<BaseHandler, HelpCommand>();
            builder.Services.AddScoped<BaseHandler, AtWorkCommand>();
            builder.Services.AddScoped<BaseHandler, TasksSortingCommand>();
            builder.Services.AddScoped<BaseHandler, NormaCommand>();
            builder.Services.AddScoped<BaseHandler, YangCommand>();
            builder.Services.AddScoped<BaseHandler, YangOnCommand>();
            builder.Services.AddScoped<BaseHandler, YangOnFavoriteCommand>();
            builder.Services.AddScoped<BaseHandler, FavoriteTaskCommand>();
            builder.Services.AddScoped<BaseHandler, CreateLinkCommand>();
            builder.Services.AddScoped<BaseHandler, AdminCommand>();
            builder.Services.AddScoped<BaseHandler, FavoriteEnvironmentsCommand>();
            builder.Services.AddScoped<BaseHandler, YangOnEnvironmentCommand>();
            builder.Services.AddScoped<BaseHandler, YangOnFavTaskEnvCommand>();

            builder.Services.AddScoped<BaseHandler, TypeSortingCallback>();
            builder.Services.AddScoped<BaseHandler, ExitTaskCallback>();
            builder.Services.AddScoped<BaseHandler, TakeTaskCallback>();
            builder.Services.AddScoped<BaseHandler, AddToFavoriteTaskCallback>();
            builder.Services.AddScoped<BaseHandler, DeleteFavoriteTaskCallback>();
            builder.Services.AddScoped<BaseHandler, AddToFavoriteEnvironmentCallback>();
            builder.Services.AddScoped<BaseHandler, DeleteFavoriteEnvironmentCallback>();

            builder.Services.AddScoped<BaseHandler, CompleteYangOnKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, CompleteYangKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, GetTwentyTaskKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, DefaultCommand>();

            builder.Services.AddScoped<BaseHandler, AddToFavoriteTaskReply>();
            builder.Services.AddScoped<BaseHandler, CheckTokenReply>();
            builder.Services.AddScoped<BaseHandler, AddToFavoriteEnvironmentReply>();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration).CreateLogger();
            builder.Host.UseSerilog();

            builder.Services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Cookies["AuthToken"];
                            if (!string.IsNullOrEmpty(accessToken))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        },
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                    };
                });
            builder.Services.AddMvc();
            builder.Services.AddControllers();

            builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"./keys/"));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseSerilogRequestLogging();

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseStatusCodePages(async context =>
            {
                var request = context.HttpContext.Request;
                var response = context.HttpContext.Response;

                if (response.StatusCode == (int)HttpStatusCode.Unauthorized)
                    response.Redirect("/Login");
            });
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }

        private static void SetStaticFields(WebApplicationBuilder builder)
        {
            StaticFields.passwordEncryption = builder.Configuration.GetValue<string>("PasswordEncryption");
            StaticFields.linkTask = builder.Configuration.GetValue<string>("UrlLinkTask");
            StaticFields.linkManual = builder.Configuration.GetValue<string>("UrlLinkManual");
            StaticFields.urlTaskList = builder.Configuration.GetValue<string>("UrlTaskList");
            StaticFields.urlTakeTask = builder.Configuration.GetValue<string>("UrlTakeTask");
            StaticFields.urlLeaveTask = builder.Configuration.GetValue<string>("UrlLeaveTask");
            StaticFields.urlCheckToken = builder.Configuration.GetValue<string>("UrlCheckToken");
            StaticFields.urlTaskTitle = builder.Configuration.GetValue<string>("UrlTaskTitle");
            StaticFields.urlCheckNorm = builder.Configuration.GetValue<string>("UrlCheckNorm");
        }
    }
}