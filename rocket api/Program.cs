using Microsoft.AspNetCore.Builder;
using rocket_api.telegram;
using System;
using System.Threading.Tasks;
using TL;
using WTelegram;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});
//builder.Services.AddHostedService<rocket_api.telegram.TelegramService>();

var app = builder.Build();
//Messages_Dialogs? CachedDialogs = null;
//DateTime LastDialogsUpdate = DateTime.MinValue;


// Configure the HTTP request pipeline.

app.UseCors("AllowAll");

//var client = await Telegram_login.Main();

//Console.WriteLine(client.User);
//app.UseWebSockets();

app.Map("/ws", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = 400;
        return;
    }

    var socket = await context.WebSockets.AcceptWebSocketAsync();
    await WebSocketHandler.AddClient(socket);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.MapGet("/contacts", async () =>
//{
//    CachedDialogs = await client.Messages_GetAllDialogs();
//    LastDialogsUpdate = DateTime.UtcNow;
//    var dialogs = await client.Messages_GetAllDialogs();
//    var response = await Get_Chat_ID.init.Get_all(dialogs,client);
//    return response;
//});
app.MapGet("/see", () =>
{
    return "hi";
});
//app.MapPost("/send", async (UserSendMessage req) =>
//{
//    var response = await send_message.init.Send(client, req.id,req.message,req.type,req.accessHash);
//    return response;
//});
//app.MapPost("/userprofileimage", async (UserAvatar req) =>
//{
//    if (CachedDialogs == null) CachedDialogs = await client.Messages_GetAllDialogs();
//    var response = await Profileimage.GetProfileImageBase64(CachedDialogs, client, req.id, req.accessHash);
//    return response;
//});
//app.MapPost("/getusermessage", async (UserGetMessage req) =>
//{
//    if (CachedDialogs == null) CachedDialogs = await client.Messages_GetAllDialogs();
//    var response = await GetMessageList.GetUserChatHistory(CachedDialogs, client, req.id, req.accessHash, req.limit);
//    return response;
//});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
//app.UseRouting();
app.Run();


public class UserGetMessage
{
    public long id { get; set; }
    public long accessHash { get; set; }
    public int limit { get; set; }
}
public class UserSendMessage
{//WTelegram.Client client, long id , string message , string type, long? accessHash
    public long id { get; set; }
    public string message { get; set; } = "";
    public string type { get; set; } = "";
    public long? accessHash { get; set; }
}
public class UserAvatar
{//WTelegram.Client client, long id , string message , string type, long? accessHash
    public long id { get; set; }
    public long accessHash { get; set; }
}