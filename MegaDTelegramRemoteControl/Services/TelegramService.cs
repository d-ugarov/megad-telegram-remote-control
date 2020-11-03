using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MegaDTelegramRemoteControl.Services
{
    public class TelegramService : ITelegramService
    {
        private readonly TelegramConfig telegramConfig;
        private TelegramBotClient bot;
        
        private readonly ITelegramLogic telegramLogic;
        private readonly ILogger<TelegramService> logger;
        
        public TelegramService(TelegramConfig telegramConfig, ITelegramLogic telegramLogic, ILogger<TelegramService> logger)
        {
            this.telegramConfig = telegramConfig;
            this.telegramLogic = telegramLogic;
            this.logger = logger;
        }

        public async Task InitBotAsync()
        {
            bot = new TelegramBotClient(telegramConfig.BotAccessToken);
            bot.OnMessage += OnMessageReceived;
            bot.OnCallbackQuery += OnCallbackQueryReceived;
            bot.StartReceiving();
            
            await bot.SetMyCommandsAsync(new List<BotCommand>
                                         {
                                             new BotCommand
                                             {
                                                 Command = "dashboard",
                                                 Description = "Dashboard"
                                             }
                                         });
                
            logger.LogTrace("TelegramBot started");
        }

        public Task SendDebugTextMessageAsync(string message)
        {
            return InvokeOperations.InvokeOperationAsync(async () =>
            {
                foreach (var chatId in telegramConfig.DebugLogUsers)
                {
                    await bot.SendTextMessageAsync(chatId, message);
                }
            });
        }
        
        private async void OnMessageReceived(object sender, MessageEventArgs e)
        {
            await InvokeOperations.InvokeOperationAsync(async () =>
            {
                if (e.Message == null)
                    return;

                var allowed = IsUserAllowed(e.Message.From);
                logger.LogTrace($"[OnMessageReceived] " +
                                $"From: {e.Message.From.Username} ({e.Message.From.Id}), " +
                                $"allowed: {allowed}, " +
                                $"message: {e.Message.Text}");
                
                if (!allowed)
                    return;
                
                await ProcessMessageAsync(e.Message);
            });
        }

        private async Task ProcessMessageAsync(Message message)
        {
            switch (message.Text?.ToLowerInvariant())
            {
                case "dashboard":
                default:
                {
                    var mainMenu = await telegramLogic.ProcessTelegramActionAsync();
                    mainMenu.EnsureSuccess();

                    var (text, inlineKeyboard) = GetFormattedAnswer(mainMenu.Data);
                    await SendMessageAsync(text, inlineKeyboard, message.Chat);
                    break;
                }
            }
        }
        
        private async void OnCallbackQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            await InvokeOperations.InvokeOperationAsync(async () =>
            {
                if (e.CallbackQuery == null)
                    return;

                var allowed = IsUserAllowed(e.CallbackQuery.From);
                logger.LogTrace($"[OnCallbackQueryReceived] " +
                                $"From: {e.CallbackQuery.From.Username} ({e.CallbackQuery.From.Id}), " +
                                $"allowed: {allowed}, " +
                                $"message: {e.CallbackQuery.Data}");
                
                if (!allowed)
                    return;

                await bot.AnswerCallbackQueryAsync(e.CallbackQuery.Id);
                
                await ProcessCallbackQueryAsync(e.CallbackQuery);
            });
        }

        private async Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            var menu = await telegramLogic.ProcessTelegramActionAsync(callbackQuery.Data);
            menu.EnsureSuccess();
            
            var chatId = new ChatId(callbackQuery.Message.Chat.Id);
            var (text, inlineKeyboard) = GetFormattedAnswer(menu.Data);
            
            await EditMessageAsync(text, inlineKeyboard, chatId, callbackQuery.Message);
        }
        
        private static (string text, InlineKeyboardMarkup inlineKeyboard) GetFormattedAnswer(TelegramBotMenu menu)
        {
            var buttons = menu.Buttons
                              .GroupBy(x => x.Order, (k, v) => new
                                                               {
                                                                   Order = k,
                                                                   Row = v.Select(x => InlineKeyboardButton.WithCallbackData(x.Name, x.Id))
                                                               })
                              .OrderBy(x => x.Order)
                              .Select(x => x.Row)
                              .ToList();

            var inlineKeyboard = new InlineKeyboardMarkup(buttons);

            return (menu.Text, inlineKeyboard);
        }
        
        private Task SendMessageAsync(string text, IReplyMarkup inlineKeyboard, Chat chat)
        {
            return bot.SendTextMessageAsync(chat.Id, text, replyMarkup: inlineKeyboard);
        }
        
        private Task EditMessageAsync(string text, InlineKeyboardMarkup inlineKeyboard, ChatId chatId, Message message)
        {
            return bot.EditMessageTextAsync(chatId, message.MessageId, text, ParseMode.Default, replyMarkup: inlineKeyboard);
        }

        private bool IsUserAllowed(User user) => !telegramConfig.AllowedUsers.Any() ||
                                                 telegramConfig.AllowedUsers.Contains(user.Id);

    }
}