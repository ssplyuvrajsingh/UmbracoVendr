using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Providers.Services
{
    public class Composer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IRemoveProviderService, RemoveProviderServices>();
            composition.Register<IDummyDataSurfaceServices, DummyDataSurfaceServices>();
            composition.Register<IGeneralService, GeneralService>();
            composition.Register<IProductSurfaceServices, ProductSurfaceServices>();
            composition.Register<INpocoService, NpocoService>();
        }
    }
}