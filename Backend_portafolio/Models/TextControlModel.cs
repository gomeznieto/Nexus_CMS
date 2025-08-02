namespace Backend_portafolio.Models
{
    public class TextControlModel
    {
        public string FieldName { get; set; }
        public string FieldValue { get; set; }

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
