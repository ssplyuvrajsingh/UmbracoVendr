using Umbraco.Core.Models.Sections;

public class DocumentsSection : ISection
{
    public string Alias => "documentsSection";

    /// <inheritdoc />
    public string Name => "Providers";
}