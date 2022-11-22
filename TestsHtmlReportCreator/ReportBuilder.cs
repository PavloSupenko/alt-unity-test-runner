using System.Drawing;
using System.Text.RegularExpressions;
using HtmlTags;


namespace TestsHtmlReportCreator;

public class ReportBuilder
{
    private const int PaddingMultiplier = 20;
    
    private readonly string remoteIos;
    private readonly string remoteAndroid;
    private readonly string localIos;
    private readonly string localAndroid;
    private readonly string reportPath;

    
    public ReportBuilder(string localAndroid, string localIos, string remoteAndroid, string remoteIos, string reportPath)
    {
        this.reportPath = reportPath;
        this.localAndroid = localAndroid;
        this.localIos = localIos;
        this.remoteAndroid = remoteAndroid;
        this.remoteIos = remoteIos;
    }

    public void Build()
    {
        HtmlDocument document = new HtmlDocument();
        document.Title = "Tests report";

        document.Head
            .Append(new HtmlTag("meta")
                .Attr("content", "width=device-width, initial-scale=1")
                .Attr("name", "viewport")
                .UseClosingTag());
        
        document.Head
            .Append(new HtmlTag("style").Text(MultilineTextToOneLine(ReportConstants.HeadStyle)));

        document.Body
            .Append(new HtmlTag("h2").Text("Report"));

        AddDirectoryInfo(document.Body, "[iOS] Remote", remoteIos, 0);
        AddDirectoryInfo(document.Body, "[Android] Remote", remoteAndroid, 0);
        AddDirectoryInfo(document.Body, "[iOS] Local", localIos, 0);
        AddDirectoryInfo(document.Body, "[Android] Local", localAndroid, 0);
        
        document.Body
            .Append(new HtmlTag("script")
                .UseClosingTag()
                .Text(MultilineTextToOneLine(ReportConstants.ExpandScript)));
        
        SaveDocumentToFile(document);
    }

    private void AddDirectoryInfo(HtmlTag parent, string directoryName, string directoryPath, int deepLevel)
    {
        string passStatus = true ? "passed" : "failed";
        HtmlTag buttonContent = AddCollapsibleButton(
            parent: parent, 
            padding: deepLevel * PaddingMultiplier, 
            buttonContent: GetButtonContentForDirectory(directoryName, passStatus));

        string[] subDirectories = Directory.GetDirectories(directoryPath);
        string[] subFiles = Directory.GetFiles(directoryPath);
        
        DirectoryInfo[] subDirectoriesInfo = subDirectories
            .Select(directory => new DirectoryInfo(directory))
            .ToArray();
        
        FileInfo[] subFilesInfo = subFiles
            .Select(file => new FileInfo(file))
            .OrderBy(fileInfo => fileInfo.Name)
            .ToArray();

        foreach (var info in subDirectoriesInfo)
            AddDirectoryInfo(buttonContent, info.Name, info.FullName, deepLevel + 1);

        foreach (var info in subFilesInfo)
            AddFileInfo(buttonContent, info.Name, info.FullName, deepLevel + 1);
    }
    
    private void AddFileInfo(HtmlTag parent, string fileName, string filePath, int deepLevel)
    {
        int padding = deepLevel * PaddingMultiplier;
        string relativeFilePath = Path.GetRelativePath(reportPath, filePath)
            .Replace("..\\", string.Empty)
            .Replace("../", string.Empty);

        HtmlTag buttonContent = AddCollapsibleButton(
            parent: parent, 
            padding: padding, 
            buttonContent: GetButtonContentForFile(fileName, Color.Olive));

        if (relativeFilePath.Contains(".png"))
            buttonContent
                .Append(new HtmlTag("img")
                    .Attr("src", relativeFilePath)
                    .Attr("style", $"max-width: 500px; max-height: 500px; height: auto; padding-left: {padding}px;"));
        else
            buttonContent
                .Append(new HtmlTag("a")
                    .Attr("href", relativeFilePath)
                    .Attr("rel", "noopener noreferrer")
                    .Attr("style", $"padding-left: {padding}px")
                    .Attr("target", "_blank")
                    .Text("Open in new tab..."));
    }

    private static HtmlTag AddCollapsibleButton(HtmlTag parent, int padding, HtmlTag buttonContent)
    {
        var collapsedContent = new HtmlTag("div");

        parent
            .Append(new HtmlTag("button")
                .UseClosingTag()
                .Attr("class", "collapsible")
                .Attr("style", $"padding-left: {padding}px")
                .Attr("type", "button")
                .Append(buttonContent)
            )
            .Append(collapsedContent
                .UseClosingTag()
                .Attr("class", "content"));

        return collapsedContent;
    }

    private static HtmlTag GetButtonContentForDirectory(string title, string passStatus)
    {
        return new HtmlTag("div")
            .UseClosingTag()
            .Attr("class", passStatus)
            .Text(title);
    }
    
    private static HtmlTag GetButtonContentForFile(string title, Color backgroundColor)
    {
        return new HtmlTag("div")
            .UseClosingTag()
            .Attr("style", $"background-color: rgb({backgroundColor.R}, {backgroundColor.G}, {backgroundColor.B});")
            .Text(title);
    }

    private void SaveDocumentToFile(HtmlDocument document)
    {
        string documentContent = document.ToString();
        documentContent = documentContent
            .Replace("&quot;", @"""")
            .Replace("&lt;", "<")
            .Replace("&#x2B;", "+")
            .Replace("class_", "class");
        
        WriteDocumentToFile(documentContent);
    }

    private void WriteDocumentToFile(string text)
    {
        using StreamWriter sw = new StreamWriter(reportPath, false);
        sw.Write(text);
    }

    private static string MultilineTextToOneLine(string text)
    {
        return Regex.Replace(text, @"\t|\n|\r", "");
    }
}