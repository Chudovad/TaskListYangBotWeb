using Microsoft.EntityFrameworkCore;
using Serilog;
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

            // добавляем контекст ApplicationContext в качестве сервиса в приложение
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            SetStaticFields(builder);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<TelegramBotService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFavoriteTaskRepository, FavoriteTaskRepository>();
            builder.Services.AddScoped<IMessageRepository, MessageRepository>();

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

            builder.Services.AddScoped<BaseHandler, AddToFavoriteReply>();

            builder.Services.AddScoped<BaseHandler, TypeSortingCallback>();
            builder.Services.AddScoped<BaseHandler, ExitTaskCallback>();
            builder.Services.AddScoped<BaseHandler, TakeTaskCallback>();
            builder.Services.AddScoped<BaseHandler, AddToFavoriteCallback>();
            builder.Services.AddScoped<BaseHandler, DeleteFavoriteTaskCallback>();

            builder.Services.AddScoped<BaseHandler, CompleteYangOnKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, CompleteYangKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, GetTwentyTaskKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, DefaultCommand>();
            builder.Services.AddScoped<BaseHandler, CheckTokenReply>();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration).CreateLogger();
            builder.Host.UseSerilog();

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