using BMW.ASP.Hubs;
using BMW.BLL;
using BMW.Dal;
using BMWDALInterfacesAndDTOs.Interfaces;
using FluentValidation;
using BMWDomain.interfaces;
using BMWDomain.Entities;



var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();


string? conString = builder.Configuration.GetConnectionString("DefaultConnection");

if (conString == null)
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found in configuration."); // to do fix the exception
}

builder.Services.AddScoped<IDiscussionThreadRepository, DiscussionThreadRepository>(_ => new DiscussionThreadRepository(conString));
builder.Services.AddScoped<IDiscussionThreadContainer, DiscussionThreadContainer>();
builder.Services.AddScoped<IUserRepository, UserRepository>(_ => new UserRepository(conString));
builder.Services.AddScoped<IUserContainer, UserContainer>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>(_ => new TopicRepository(conString));
builder.Services.AddScoped<ITopicContainer, TopicContainer>();
builder.Services.AddScoped<IValidator<DiscussionThread>, ThreadValidator>();
builder.Services.AddScoped<IValidator<Account>, UserValidator>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>(_ => new CommentRepository(conString));
builder.Services.AddScoped<IValidator<Comment>, CommentValidator>();
builder.Services.AddScoped<ICommentContainer, CommentContainer>();
builder.Services.AddScoped<IClubRepository, ClubRepository>(_ => new ClubRepository(conString));
builder.Services.AddScoped<IClubContainer, ClubContainer>();
builder.Services.AddScoped<IValidator<CreateClub>, ClubValidator>();
builder.Services.AddScoped<IClubRolesRepository, ClubRolesRepository>(_ => new ClubRolesRepository(conString));
builder.Services.AddScoped<IClubRolesContainer, ClubRolesContainer>();
builder.Services.AddScoped<IInviteCodeRepository, InviteCodeRepository>(_ => new InviteCodeRepository(conString));
builder.Services.AddScoped<IInviteCodeContainer, InviteCodeContainer>();
builder.Services.AddScoped<IValidator<InviteCode>, InvteCodeValidator>();
builder.Services.AddScoped<IChatRepository, ChatRepository>(_ => new ChatRepository(conString));
builder.Services.AddScoped<IValidator<Message>, MessageValidator>();
builder.Services.AddScoped<IChatContainer, ChatContainer>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();


app.MapHub<ChatHub>("/chatHub");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
