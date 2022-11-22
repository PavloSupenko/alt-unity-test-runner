namespace TestsHtmlReportCreator;

public static class ReportConstants
{
    public const string ExpandScript = 
@"
var coll = document.getElementsByClassName(""collapsible"");
var i;
for (i = 0; i < coll.length; i++) {
    coll[i].addEventListener(""click"", function() {
        this.classList.toggle(""active"");
        var content = this.nextElementSibling;
        if (content.style.display === ""block"") {
            content.style.display = ""none"";
        } else {
            content.style.display = ""block"";
        }
    });
}
";
    
    public const string HeadStyle = 
@".collapsible {
    background-color: rgb(255, 255, 255);
    color: white;
    cursor: pointer;
    padding: 0px;
    width: 100%;
    border: none;
    text-align: left;
    outline: none;
    font-size: 18px;
 }

.failed {
    background-color: rgb(120, 0, 0);
}

.passed {
    background-color: rgb(0, 120, 0);
}

.active, .collapsible:hover {
    background-color: rgb(255, 255, 255);
} 

.content {
    padding: 0 0px;
    display: none;
    overflow: hidden;
    background-color: white;
}";
}