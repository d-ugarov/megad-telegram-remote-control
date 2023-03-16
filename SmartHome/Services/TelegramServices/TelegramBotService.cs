using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models;
using MegaDTelegramRemoteControl.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartHome.Common.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace MegaDTelegramRemoteControl.Services.TelegramServices;

public class TelegramBotService : IBotService
{
    private readonly TelegramConfig telegramConfig;
    private TelegramBotClient bot = null!;

    private readonly IBotHandler botHandler;
    private readonly ILogger<TelegramBotService> logger;

    private bool isInitialized;

    public TelegramBotService(IOptions<TelegramConfig> telegramConfig,
        IBotHandler botHandler,
        ILogger<TelegramBotService> logger)
    {
        this.telegramConfig = telegramConfig.Value;
        this.botHandler = botHandler;
        this.logger = logger;
    }

    public Task InitBotAsync()
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            if (isInitialized)
                return;

            logger.LogTrace("TelegramBot starting..");

            bot = new TelegramBotClient(telegramConfig.BotAccessToken);

            bot.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: new()
                                 {
                                     AllowedUpdates = Array.Empty<UpdateType>(),
                                 }
            );

            await bot.SetMyCommandsAsync(new List<BotCommand>
                                         {
                                             new()
                                             {
                                                 Command = "dashboard",
                                                 Description = "Dashboard"
                                             }
                                         });

            isInitialized = true;
            logger.LogTrace("TelegramBot started");
        });
    }

    private Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken ct) => update switch
    {
        {Message: { } message} => OnMessageReceivedAsync(message),
        {CallbackQuery: { } callbackQuery} => OnCallbackQueryReceivedAsync(callbackQuery),
        _ => OnUnknownUpdateReceivedAsync(update),
    };

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception ex, CancellationToken ct)
    {
        logger.LogInformation($"HandleError: {ex}");
        return Task.CompletedTask;
    }

    private Task OnMessageReceivedAsync(Message message)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var allowed = IsUserAllowed(message.From);

            logger.LogTrace("[OnMessageReceived] " +
                            $"From: {(message.From != null ? $"{message.From.Username} ({message.From.Id})" : "unknown")}, " +
                            $"allowed: {allowed}, " +
                            $"message: {message.Text}");

            if (!allowed)
                return;

            await ProcessMessageAsync(message);
        });
    }

    private async Task ProcessMessageAsync(Message message)
    {
        switch (message.Text?.ToLowerInvariant())
        {
            case "dashboard":
            default:
            {
                var mainMenu = await botHandler.ProcessActionAsync();

                var (text, inlineKeyboard) = GetFormattedAnswer(mainMenu);
                await SendMessageAsync(text, inlineKeyboard, message.Chat);
                break;
            }
        }
    }

    private Task OnCallbackQueryReceivedAsync(CallbackQuery callbackQuery)
    {
        return InvokeOperations.InvokeOperationAsync(async () =>
        {
            var allowed = IsUserAllowed(callbackQuery.From);

            logger.LogTrace("[OnCallbackQueryReceived] " +
                            $"From: {callbackQuery.From.Username} ({callbackQuery.From.Id}), " +
                            $"allowed: {allowed}, " +
                            $"message: {callbackQuery.Data}");

            if (!allowed)
                return;

            await ProcessCallbackQueryAsync(callbackQuery);
        });
    }

    private async Task ProcessCallbackQueryAsync(CallbackQuery callbackQuery)
    {
        await bot.AnswerCallbackQueryAsync(callbackQuery.Id);
        
        var menu = await botHandler.ProcessActionAsync(callbackQuery.Data);

        if (callbackQuery.Message != null)
        {
            var chatId = new ChatId(callbackQuery.Message.Chat.Id);
            var (text, inlineKeyboard) = GetFormattedAnswer(menu);

            await EditMessageAsync(text, inlineKeyboard, chatId, callbackQuery.Message);
        }
    }

    private Task OnUnknownUpdateReceivedAsync(Update update)
    {
        logger.LogTrace($"[OnUnknownUpdateReceived] Type: {update.Type}");
        return Task.CompletedTask;
    }

    private static (string text, InlineKeyboardMarkup inlineKeyboard) GetFormattedAnswer(BotMenu menu)
    {
        // var buttons = menu.Buttons
        //                   .GroupBy(x => x.Order, (k, v) => new
        //                                                    {
        //                                                        Order = k,
        //                                                        Row = v.Select(x =>
        //                                                            InlineKeyboardButton.WithCallbackData(x.Name,
        //                                                                x.ActionId))
        //                                                    })
        //                   .OrderBy(x => x.Order)
        //                   .Select(x => x.Row)
        //                   .ToList();

        var buttons = menu.Buttons
                          .OrderBy(x => x.Order)
                          .Select(x => new List<InlineKeyboardButton>
                                       {
                                           InlineKeyboardButton.WithCallbackData(x.Name, x.ActionId)
                                       })
                          .ToList();

        if (menu.FooterButtons.Any())
        {
            buttons.Add(menu.FooterButtons
                            .OrderBy(x => x.Order)
                            .Select(x => InlineKeyboardButton.WithCallbackData(x.Name, x.ActionId))
                            .ToList());
        }

        var inlineKeyboard = new InlineKeyboardMarkup(buttons);

        return (menu.Text, inlineKeyboard);
    }

    private Task SendMessageAsync(string text, IReplyMarkup inlineKeyboard, Chat chat)
    {
        return bot.SendTextMessageAsync(chat.Id, text, replyMarkup: inlineKeyboard);
    }

    private Task EditMessageAsync(string text, InlineKeyboardMarkup inlineKeyboard, ChatId chatId, Message message)
    {
        return bot.EditMessageTextAsync(chatId, message.MessageId, text, ParseMode.MarkdownV2, replyMarkup: inlineKeyboard);
    }

    private bool IsUserAllowed(User? user) => !telegramConfig.AllowedUsers.Any() ||
                                              (user != null && telegramConfig.AllowedUsers.Contains(user.Id));

}