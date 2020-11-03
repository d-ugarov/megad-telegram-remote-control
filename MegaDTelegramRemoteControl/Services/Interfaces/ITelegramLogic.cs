﻿using MegaDTelegramRemoteControl.Infrastructure.Models;
using MegaDTelegramRemoteControl.Models;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces
{
    public interface ITelegramLogic
    {
        Task<OperationResult<TelegramBotMenu>> ProcessTelegramActionAsync(string actionId = null);
    }
}