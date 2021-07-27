using Umbraco.Core.Composing;
using Umbraco.Web;

public class SectionComposer : IUserComposer
{
    public void Compose(Composition composition)
    {
        composition.Sections().Append<DocumentsSection>();
        //composition.Sections().Remove<DocumentsSection>();
    }
}