using MegaDTelegramRemoteControl.Infrastructure.Jobs.Configurations;

namespace MegaDTelegramRemoteControl.Infrastructure.Jobs.Enums
{
    public enum JobType
    {
        [JobType(30, 60)]
        Test,
    }
}