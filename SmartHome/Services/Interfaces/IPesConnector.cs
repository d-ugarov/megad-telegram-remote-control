using MegaDTelegramRemoteControl.Infrastructure.Configurations;
using MegaDTelegramRemoteControl.Models.PES;
using SmartHome.Common.Infrastructure.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MegaDTelegramRemoteControl.Services.Interfaces;

public interface IPesConnector
{
    Task<OperationResult<List<PesGroup>>> GetGroupsAsync(PesClientConfig config);

    Task<OperationResult<List<PesAccount>>> GetAccountsAsync(PesClientConfig config, int groupId);

    Task<OperationResult<PesData>> GetAccountDataAsync(PesClientConfig config, int accountId);
}