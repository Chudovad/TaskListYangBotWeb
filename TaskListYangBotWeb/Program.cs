using TaskListYangBotWeb.Data.Interfaces;
using TaskListYangBotWeb.Data.Repository;
using TaskListYangBotWeb.Handlers.Callbacks;
using TaskListYangBotWeb.Handlers.Commands;
using TaskListYangBotWeb.Handlers.KeyboardCommands;
using TaskListYangBotWeb.Handlers.Replies;
using TaskListYangBotWeb.Handlers;
using TaskListYangBotWeb.Helper;
using TaskListYangBotWeb.Services.Interfaces;
using TaskListYangBotWeb.Services;
using Microsoft.EntityFrameworkCore;
using TaskListYangBotWeb.Data;
using Microsoft.Extensions.Configuration;
using TaskListYangTgBot;

namespace TaskListYangBotWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // добавляем контекст ApplicationContext в качестве сервиса в приложение
            builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            StaticFields.passwordEncryption = builder.Configuration.GetValue<string>("PasswordEncryption");
            StaticFields.linkTask = builder.Configuration.GetValue<string>("UrlLinkTask");
            StaticFields.linkManual = builder.Configuration.GetValue<string>("UrlLinkManual");

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddSingleton<TelegramBotService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();

            builder.Services.AddScoped<ICommandExecutor, CommandExecutor>();
            builder.Services.AddScoped<BaseHandler, StartCommand>();

            builder.Services.AddScoped<BaseHandler, HelpCommand>();

            builder.Services.AddScoped<BaseHandler, MsgAllUsersReply>();

            builder.Services.AddScoped<BaseHandler, GrantAccessUserCallback>();
            builder.Services.AddScoped<BaseHandler, BusKeyboardCommand>();
            builder.Services.AddScoped<BaseHandler, DefaultHandler>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}