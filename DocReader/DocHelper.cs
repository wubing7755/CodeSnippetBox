using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace DocReader;

public static class DocHelper
{
    // 支持读取的文件扩展名
    private static readonly HashSet<string> CodeExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".c", ".h", ".cpp", ".hpp", ".cs", ".java", ".py",
        ".txt", ".md", ".xml", ".json", ".log", ".cfg", ".ini"
    };

    /// <summary>
    /// 根据文件夹结构生成带层级标题的 Word 文档
    /// </summary>
    /// <param name="inputFolder">要遍历的项目根目录</param>
    /// <param name="docPath">输出的 .docx 文件完整路径</param>
    /// <remarks>仅结构，不含文件内容</remarks>
    public static void BuildDocStructure(string inputFolder, string docPath)
    {
        if (!File.Exists(docPath))
        {
            CreateBlankDoc(docPath);
        }
        
        LogHelper.Write($"开始生成代码文档结构 → {docPath}");

        using var doc = WordprocessingDocument.Open(docPath, true);
        var body = doc.MainDocumentPart!.Document.Body!;
        
        // 清空文档原有内容
        body.RemoveAllChildren();
        
        // 确保文档包含完整的内置标题样式（Heading1~Heading9）
        EnsureStyles(doc);
        
        // 递归生成文件夹 + 文件的标题结构
        AddTitleRecursive(inputFolder, body, 1);

        doc.Save();
        LogHelper.Write("结构生成与内容插入完成");
    }
    
    /// <summary>
    /// 创建一个最简有效的空白 .docx 文件（不含样式表）
    /// </summary>
    private static void CreateBlankDoc(string filePath)
    {
        using var doc = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document);
    
        // 添加主文档部件
        var mainPart = doc.AddMainDocumentPart();
        mainPart.Document = new Document();
    
        // 创建文档结构和正文
        var body = new Body();
        mainPart.Document.AppendChild(body);
    
        // 添加一个空段落确保文档结构有效
        var paragraph = new Paragraph(new Run(new Text("")));
        body.AppendChild(paragraph);
    
        mainPart.Document.Save();
    
        LogHelper.Write($"创建了空白Word文档: {filePath}");
    }
    
    /// <summary>
    /// 确保文档的 StyleDefinitionsPart 包含完整的标准标题样式（Heading1~Heading9）
    /// </summary>
    private static void EnsureStyles(WordprocessingDocument doc)
    {
        var mainPart = doc.MainDocumentPart!;
        var stylePart = mainPart.StyleDefinitionsPart ?? mainPart.AddNewPart<StyleDefinitionsPart>();

        if (stylePart.Styles == null || !stylePart.Styles.Elements<Style>().Any())
        {
            var styles = new Styles();

            // 先定义完整的 Normal 样式
            styles.AppendChild(CreateNormalStyle());

            // 再定义 Heading1~9（基于 Normal）
            for (int i = 1; i <= 9; i++)
                styles.AppendChild(CreateHeadingStyle(i));

            stylePart.Styles = styles;
            stylePart.Styles.Save();
        }
    }
    
    /// <summary>
    /// 创建基础样式
    /// </summary>
    private static Style CreateNormalStyle()
    {
        var style = new Style
        {
            Type = StyleValues.Paragraph,
            StyleId = "Normal",
            Default = OnOffValue.FromBoolean(true)
        };

        style.Append(new StyleName { Val = "Normal" });
        style.Append(new UIPriority { Val = 1 });
        style.Append(new UnhideWhenUsed());

        // 段落间距
        var pPr = new StyleParagraphProperties
        {
            SpacingBetweenLines = new SpacingBetweenLines { After = "0", Line = "240", LineRule = LineSpacingRuleValues.Auto }
        };
        style.Append(pPr);

        // 文字属性
        var rPr = new StyleRunProperties();
        rPr.RunFonts = new RunFonts
        {
            Ascii = "Times New Roman",
            HighAnsi = "Times New Roman",
            EastAsia = "宋体",
        };
        rPr.FontSize = new FontSize { Val = "24" };           // 12pt × 2
        rPr.FontSizeComplexScript = new FontSizeComplexScript { Val = "24" };

        style.Append(rPr);
        return style;
    }

    /// <summary>
    /// 创建单个 Heading 样式（Heading1 ~ Heading9）
    /// </summary>
    /// <param name="level">标题级别 1~9</param>
    private static Style CreateHeadingStyle(int level)
    {
        var style = new Style
        {
            Type = StyleValues.Paragraph,
            StyleId = $"Heading{level}",
            CustomStyle = false
        };

        style.Append(new StyleName { Val = $"heading {level}" });
        style.Append(new BasedOn { Val = "Normal" });
        style.Append(new UIPriority { Val = level });
        style.Append(new UnhideWhenUsed());

        // 段落属性：设置大纲级别
        var styleParagraphProperties = new StyleParagraphProperties();
        var outlineLevel = new OutlineLevel { Val = level - 1 };
        styleParagraphProperties.Append(outlineLevel);

        // 文字属性：加粗 + 近似 Word 默认字号 + 颜色
        var styleRunProperties = new StyleRunProperties
        {
            Bold = new Bold(),
            FontSize = new FontSize { Val = (32 - level * 2).ToString() },
            Color = new Color { Val = "2E74B5" }
        };

        if (level <= 3)
            styleRunProperties.Color.Val = "2E74B5";

        style.Append(styleParagraphProperties);
        style.Append(styleRunProperties);
        return style;
    }

    /// <summary>
    /// 创建带指定 Heading 级别的段落
    /// </summary>
    /// <param name="text">标题文本</param>
    /// <param name="level">1~9</param>
    private static Paragraph CreateHeadingParagraph(string text, int level)
    {
        level = Math.Clamp(level, 1, 9);

        var paragraph = new Paragraph();

        var pProps = new ParagraphProperties
        {
            ParagraphStyleId = new ParagraphStyleId { Val = $"Heading{level}" }
        };
        paragraph.Append(pProps);

        var run = new Run
        {
            RunProperties = new RunProperties(new Bold())
        };
        run.Append(new Text(text) { Space = SpaceProcessingModeValues.Preserve });

        paragraph.Append(run);
        return paragraph;
    }
    
    /// <summary>
    /// 递归遍历文件夹，生成层级标题结构
    /// 规则：
    ///   - 文件夹 → HeadingN
    ///   - 当前目录下的文件 → HeadingN+1
    ///   - 子文件夹 → HeadingN+1（递归）
    /// </summary>
    private static void AddTitleRecursive(string currentFolder, Body body, int level)
    {
        string folderName = Path.GetFileName(currentFolder.TrimEnd('\\', '/'));
        if (string.IsNullOrEmpty(folderName)) 
            folderName = "根目录";
        
        int headingLevel = Math.Min(level, 9);
        
        // 添加当前文件夹标题
        body.AppendChild(CreateHeadingParagraph(folderName, headingLevel));

        // 1. 添加当前目录下的文件标题（使用下一级标题）
        var files = Directory.GetFiles(currentFolder, "*.*", SearchOption.TopDirectoryOnly)
            .Where(f => CodeExtensions.Contains(Path.GetExtension(f)))
            .OrderBy(Path.GetFileName);

        foreach (string file in files)
        {
            string fileName = Path.GetFileName(file);
            body.AppendChild(CreateHeadingParagraph(fileName, headingLevel + 1));
        }

        // 2. 递归处理子文件夹
        var subFolders = Directory.GetDirectories(currentFolder)
            .OrderBy(Path.GetFileName);

        foreach (string subFolder in subFolders)
        {
            // 排除隐藏文件夹或 bin/obj 等
            string dirName = Path.GetFileName(subFolder);
            if (dirName.StartsWith(".") || dirName is "bin" or "obj" or "node_modules" or ".git" or ".vs")
                continue;
            
            AddTitleRecursive(subFolder, body, level + 1);
        }
    }
    
    /// <summary>
    /// 读取已生成的结构文档，在每个文件标题后插入真实文件内容
    /// </summary>
    public static void ProcessDocument(string inputFolder, string docxPath)
    {
        LogHelper.Write($"正在处理 → {docxPath}\n");

        var headingStyles = GetHeadingStyles(docxPath);

        if (headingStyles.Count == 0)
        {
            LogHelper.Write("False: 未找到标题样式，无法继续");
            return;
        }

        InsertFilesIntoDoc(inputFolder, docxPath, headingStyles);
    }

    // 自动从文档中读取所有具有 OutlineLevel 的段落样式（即标题样式）
    private static List<string> GetHeadingStyles(string docxPath)
    {
        var result = new List<string>();

        using var doc = WordprocessingDocument.Open(docxPath, false);
        var stylePart = doc.MainDocumentPart?.StyleDefinitionsPart;

        if (stylePart?.Styles == null)
            return result;

        foreach (var style in stylePart.Styles.Elements<Style>())
        {
            // 必须是段落样式且定义了大纲级别（OutlineLevel）
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

    // 遍历文档中所有标题段落，匹配文件并插入内容
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

    // 文件匹配
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
    
    // 在指定标题段落后插入文件内容（代码文件使用等宽字体）
    private static void InsertFileContentAfterParagraph(Paragraph heading, string filePath)
    {
        string ext = Path.GetExtension(filePath).ToLowerInvariant();
        bool isCode = CodeExtensions.Contains(ext);

        string content = File.ReadAllText(filePath, Encoding.UTF8);

        var lines = content.Replace("\r\n", "\n").Split('\n');

        OpenXmlElement insertAfter = heading;

        foreach (string line in lines)
        {
            var text = new Text(line) { Space = SpaceProcessingModeValues.Preserve };
            var run = new Run(text);

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
            var p = new Paragraph(run)
            {
                ParagraphProperties = new ParagraphProperties(
                    new ParagraphStyleId { Val = "Normal" },
                    new SpacingBetweenLines { After = "100" }  // 行间距
                )
            };

            insertAfter = insertAfter.InsertAfterSelf(p);
        }

        // // 添加空行分隔
        // insertAfter.InsertAfterSelf(new Paragraph());
        // insertAfter.InsertAfterSelf(new Paragraph());
    }

    // 读取段落纯文本
    private static string GetText(Paragraph p) => string.Concat(p.Descendants<Text>().Select(t => t.Text));
}
