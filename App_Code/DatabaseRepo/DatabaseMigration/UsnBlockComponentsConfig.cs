using System.Collections.Generic;

public class UsnBlockComponentsConfig
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
    public class Block
    {
        public string backgroundColor { get; set; }
        public string iconColor { get; set; }
        public string thumbnail { get; set; }
        public string contentElementTypeKey { get; set; }
        public string settingsElementTypeKey { get; set; }
        public object view { get; set; }
        public object stylesheet { get; set; }
        public string label { get; set; }
        public string editorSize { get; set; }
        public bool forceHideContentEditorInOverlay { get; set; }
    }

    public class ValidationLimit
    {
    }

    public class Root
    {
        public List<Block> blocks { get; set; }
        public ValidationLimit validationLimit { get; set; }
        public bool useLiveEditing { get; set; }
        public bool useInlineEditingAsDefault { get; set; }
        public object maxPropertyWidth { get; set; }
    }

}