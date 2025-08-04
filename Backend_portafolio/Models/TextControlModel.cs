namespace Backend_portafolio.Models
{

    public enum EditMode
    {
        Full,
        Simplified
    }

    public class TextControlModel
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
        public EditMode EditMode { get; set; } = EditMode.Simplified;
        public string EditorId { get; set; } = Guid.NewGuid().ToString("N");
        public List<string> Language { get; set; } = new List<string>
        {
            "plaintext",
            "bash",
            "c",
            "cpp",
            "csharp",
            "css",
            "diff",
            "go",
            "html",
            "java",
            "javascript",
            "json",
            "kotlin",
            "lua",
            "markdown",
            "php",
            "powershell",
            "python",
            "ruby",
            "rust",
            "sql",
            "swift",
            "typescript",
            "xml",
            "yaml"
        };
    }
}
