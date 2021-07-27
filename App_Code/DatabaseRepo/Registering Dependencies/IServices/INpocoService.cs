using NPoco;

namespace Providers.Services
{
    public interface INpocoService
    {
        IDatabase Database();
    }
}