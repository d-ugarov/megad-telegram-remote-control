using MegaDTelegramRemoteControl.Services.Interfaces;

namespace MegaDTelegramRemoteControl.Services.PrivateOffices.PES;

public class PesService : IPesService
{
    private readonly IPesConnector pesConnector;

    public PesService(IPesConnector pesConnector)
    {
        this.pesConnector = pesConnector;
    }
}