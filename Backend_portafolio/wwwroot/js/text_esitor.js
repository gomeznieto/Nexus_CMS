function addTextFormat(style) {
    let txtarea = document.getElementById("post_text");
    const startPos = txtarea.selectionStart;
    const endPos = txtarea.selectionEnd;
    const before = txtarea.value.substring(0, startPos);
    const after = txtarea.value.substring(startPos, txtarea.value.length);
    var selectedText = txtarea.value.substring(startPos, endPos);
    let styleFormat = textFormat(style, selectedText);

    txtarea.value = before + `${styleFormat} ` + after.substring(selectedText.length);
    txtarea.selectionStart = txtarea.selectionEnd = startPos + styleFormat.length;
    txtarea.focus();
}

function textFormat(format, text) {

    switch (format) {
        case "h1":
            return `# ${text}`;
            break;
        case "h2":
            return `## ${text}`;
            break;
        case "h3":
            return `### ${text}`;
            break;
        case "bold":
            return `**${text}**`;
            break;
        case "italic":
            return `*${text}*`;
            break;
        case "link":
            return `[title](${text})`;
            break;
        case "image":
            return `![alt text](${text})`;
            break;
        default:
            return "?";
            break;
    }
}