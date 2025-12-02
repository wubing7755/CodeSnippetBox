using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocReader;

public static class DocHelper
{
    // 支持的代码文件扩展名
    private static readonly HashSet<string> CodeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".c", ".h", ".cpp", ".hpp", ".cs", ".java", ".py",
        ".txt", ".md", ".xml", ".json", ".log", ".cfg", ".ini"
    };

    // ============================ 新功能：递归生成文件夹结构 ============================
    public static void BuildStructureAndInsert(string folderPath, string outputDocx)
    {
        LogHelper.Write($"开始生成代码文档结构 → {outputDocx}");

        using var doc = WordprocessingDocument.Open(outputDocx, true);
        var body = doc.MainDocumentPart!.Document.Body!;

        // 可选：清空模板原内容（如果你模板是完全空白可注释掉）
        // body.RemoveAllChildren();

        AddFolderRecursive(folderPath, body, 1);

        doc.Save();
        LogHelper.Write("结构生成与内容插入完成");
    }

    private static void AddFolderRecursive(string currentFolder, Body body, int level)
    {
        string folderName = Path.GetFileName(currentFolder.TrimEnd('\\', '/'));
        if (string.IsNullOrEmpty(folderName)) folderName = "根目录";

        // 添加文件夹标题（如：Src、Common、Config等）
        Paragraph folderHeading = CreateHeadingParagraph(folderName, level);
        body.AppendChild(folderHeading);

        // 1. 先插入本目录下的文件（文件紧跟在文件夹标题后）
        var files = Directory.GetFiles(currentFolder, "*.*", SearchOption.TopDirectoryOnly)
            .Where(f => CodeExtensions.Contains(Path.GetExtension(f)))
            .OrderBy(Path.GetFileName);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);

            Paragraph fileHeading = CreateHeadingParagraph(fileName, level + 1);
            body.AppendChild(fileHeading);

            // 直接插入文件内容（不依赖标题匹配，避免同名文件问题）
            InsertFileContentAfterParagraph(fileHeading, file);

            // 文件之间加两个空行，分隔清晰
            body.AppendChild(new Paragraph());
            body.AppendChild(new Paragraph());
        }

        // 2. 再递归处理子文件夹
        var subFolders = Directory.GetDirectories(currentFolder)
            .OrderBy(Path.GetFileName);

        foreach (string subFolder in subFolders)
        {
            AddFolderRecursive(subFolder, body, level + 1);
        }
    }
    
    private static Paragraph CreateHeadingParagraph(string text, int level)
    {
        string styleId = "Heading" + (level > 9 ? 9 : level);

        var paragraph = new Paragraph();
        var pPr = new ParagraphProperties();
        pPr.ParagraphStyleId = new ParagraphStyleId { Val = styleId };
        
        // 可选美化：段后间距
        pPr.SpacingBetweenLines = new SpacingBetweenLines { After = "200" };

        paragraph.Append(pPr);

        var run = new Run(new Text(text));
        paragraph.Append(run);

        return paragraph;
    }
    
    /// <summary>
    /// 读取给定 <see cref="folderPath"/> 中所有文件，按标题插入到 <see cref="outputDocx"/> 中
    /// </summary>
    public static void ProcessDocument(string folderPath, string outputDocx)
    {
        LogHelper.Write($"正在处理 → {outputDocx}\n");

        var headingStyles = GetHeadingStyles(outputDocx);

        if (headingStyles.Count == 0)
        {
            LogHelper.Write("False: 未找到标题样式，无法继续");
            return;
        }

        InsertFilesIntoDoc(folderPath, outputDocx, headingStyles);
    }

    // ────────────────────────────────────────────────────────────────
    // 1) 自动从文档中读取标题样式
    // ────────────────────────────────────────────────────────────────
    private static List<string> GetHeadingStyles(string docxPath)
    {
        var result = new List<string>();

        using var doc = WordprocessingDocument.Open(docxPath, false);
        var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;

        if (stylePart?.Styles == null)
            return result;

        foreach (var style in stylePart.Styles.Elements<Style>())
        {
            if (style.Type != null 
                && style.Type == StyleValues.Paragraph 
                && style.StyleParagraphProperties?.OutlineLevel != null &&
                style.StyleId != null)
            {
                result.Add(style.StyleId);
            }
        }

        result.Sort();
        LogHelper.Write($"检测到 {result.Count} 个标题样式: {string.Join(", ", result)}");

        return result;
    }

    // ────────────────────────────────────────────────────────────────
    // 2) 遍历文档标题并插入文件内容
    // ────────────────────────────────────────────────────────────────
    private static void InsertFilesIntoDoc(string folderPath, string docxPath, List<string> headingStyles)
    {
        int success = 0, notFound = 0;

        using var doc = WordprocessingDocument.Open(docxPath, true);
        var body = doc.MainDocumentPart?.Document.Body;

        // 查找标题段落
        var headings = body?.Descendants<Paragraph>()
            .Where(p => p.ParagraphProperties?.ParagraphStyleId?.Val != null &&
                        headingStyles.Contains(p.ParagraphProperties.ParagraphStyleId.Val))
            .Select(p => new { Para = p, Text = GetText(p).Trim() })
            .Where(x => !string.IsNullOrWhiteSpace(x.Text))
            .ToList();

        if (headings == null || headings.Count == 0) return;
        
        LogHelper.Write($"找到 {headings.Count} 个标题节点\n");
        
        // 计算标题长度，用于对齐
        int maxTitleLength = headings.Max(h => h.Text.Length);
        
        foreach (var h in headings)
        {
            string title = h.Text.Trim();
            string? file = FindMatchingFile(folderPath, title);

            string paddedTitle = title.PadRight(maxTitleLength + 5);
            string relative = file == null ? "<未找到>" : Path.GetRelativePath(folderPath, file);
            
            if (file == null)
            {
                LogHelper.Write($"False: 未找到：{paddedTitle}");
                notFound++;
                continue;
            }

            LogHelper.Write($"True  : 成功插入 {paddedTitle} ← {relative}");
            InsertFileContentAfterParagraph(h.Para, file);
            success++;
        }
        
        LogHelper.Write("\n" + new string('═', 80));
        LogHelper.Write($"汇总统计".PadRight(20) + $"成功插入标题：{success} 个");
        LogHelper.Write($"未找到标题：{notFound} 个".PadRight(20) + (notFound > 0 ? "（通常是目录标题，子文件已由子标题插入）" : ""));
        LogHelper.Write($"输出文件：{docxPath}");
    }

    // ────────────────────────────────────────────────────────────────
    // 3) 文件匹配
    // ────────────────────────────────────────────────────────────────
    private static string? FindMatchingFile(string folder, string title)
    {
        string titleTrim = title.Trim();
        if (string.IsNullOrWhiteSpace(titleTrim)) return null;

        var allFiles = Directory.EnumerateFiles(folder, "*.*", SearchOption.AllDirectories)
            .Select(f => new { File = f, Name = Path.GetFileName(f) })
            .ToList();

        // 只用精确匹配（带或不带扩展名）
        var match = allFiles.FirstOrDefault(x => x.Name.Equals(titleTrim, StringComparison.OrdinalIgnoreCase)) 
                    ?? allFiles.FirstOrDefault(x => Path.GetFileNameWithoutExtension(x.File).Equals(titleTrim, StringComparison.OrdinalIgnoreCase));

        if (match != null) 
            return match.File;
        
        return null;
    }
    
    // ────────────────────────────────────────────────────────────────
    // 4) 将代码文件内容插入到标题后
    // ────────────────────────────────────────────────────────────────
    private static void InsertFileContentAfterParagraph(Paragraph heading, string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        bool isCode = CodeExtensions.Contains(ext);

        string content = File.ReadAllText(filePath, Encoding.UTF8);

        var lines = content.Replace("\r\n", "\n").Split('\n');

        OpenXmlElement insertAfter = heading;

        foreach (string line in lines)
        {
            Text t = new Text(line)
            {
                Space = SpaceProcessingModeValues.Preserve
            };

            Run run = new Run(t);

            // 代码文件 → 设置等宽字体
            if (isCode)
            {
                run.RunProperties = new RunProperties(
                    new RunFonts
                    {
                        Ascii = DocCs.CodeFont,
                        HighAnsi = DocCs.CodeFont,
                        EastAsia = DocCs.CodeFontEastAsia,
                        Hint = FontTypeHintValues.Default
                    },
                    new FontSize
                    {
                        Val = (DocCs.CodeFontSizePt * 2).ToString() // 小四 12pt → 24
                    }
                );
            }

            // 段落样式
            Paragraph p = new Paragraph(run)
            {
                ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId { Val = "Normal" }
                )
            };

            insertAfter = insertAfter.InsertAfterSelf(p);
        }

        // // 添加空行分隔
        // insertAfter.InsertAfterSelf(new Paragraph());
        // insertAfter.InsertAfterSelf(new Paragraph());
    }

    // 读取段落文本
    private static string GetText(Paragraph p) => string.Concat(p.Descendants<Text>().Select(t => t.Text));
}
