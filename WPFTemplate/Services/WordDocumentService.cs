using System.IO;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace WPFTemplate.Services
{
    public static class WordDocumentService
    {
        public static void GenerateTestCaseReport(string filePath, IEnumerable<(string Action, string Expected, string Result)> testCases)
        {
            try
            {
                using (var wordDocument = WordprocessingDocument.Create(filePath, WordprocessingDocumentType.Document))
                {
                    var mainPart = wordDocument.AddMainDocumentPart();
                    mainPart.Document = new Document(new Body());
                    var body = mainPart.Document.Body;
                    
                    var titleParagraph = new Paragraph(
                        new ParagraphProperties(new Justification() { Val = JustificationValues.Center }),
                        new Run(
                            new RunProperties(new Bold(), new FontSize() { Val = "32" }),
                            new Text("Отчет о тестировании (ТестКейс)")
                        )
                    );
                    body?.Append(titleParagraph);
                    body?.Append(new Paragraph(new Run(new Text(""))));

                    var table = new Table();
                    
                    var tblProp = new TableProperties(
                        new TableBorders(
                            new TopBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new BottomBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new LeftBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new RightBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new InsideHorizontalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 },
                            new InsideVerticalBorder() { Val = new EnumValue<BorderValues>(BorderValues.Single), Size = 12 }
                        ),
                        new TableWidth() { Type = TableWidthUnitValues.Pct, Width = "5000" }
                    );
                    table.AppendChild(tblProp);
                    
                    var headerRow = new TableRow();
                    headerRow.Append(CreateCell("Действие", true));
                    headerRow.Append(CreateCell("Ожидаемый результат", true));
                    headerRow.Append(CreateCell("Результат", true));
                    table.Append(headerRow);
                    
                    foreach (var tc in testCases)
                    {
                        var dataRow = new TableRow();
                        dataRow.Append(CreateCell(tc.Action));
                        dataRow.Append(CreateCell(tc.Expected));

                        var isError = tc.Result.Contains("Ошибка") || tc.Result.Contains("Провал");
                        dataRow.Append(CreateCell(tc.Result, false, isError ? "FF0000" : "008000"));
                        
                        table.Append(dataRow);
                    }
                    
                    body?.Append(table);
                    mainPart.Document.Save();
                }
                
                System.Windows.MessageBox.Show($"Файл успешно сохранен:\n{Path.GetFullPath(filePath)}", 
                                               "Экспорт завершен", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (IOException)
            {
                System.Windows.MessageBox.Show("Ошибка: Закройте файл Word перед экспортом!", 
                                               "Ошибка доступа", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            }
        }
        
        private static TableCell CreateCell(string text, bool isHeader = false, string hexColor = "000000")
        {
            var cell = new TableCell();
            

            var run = new Run(new Text(text));
            var runProps = new RunProperties();
            
            if (isHeader)
            {
                runProps.Append(new Bold());
                cell.Append(new TableCellProperties(new Shading() { Val = ShadingPatternValues.Clear, Fill = "D9D9D9" }));
            }

            if (hexColor != "000000")
            {
                runProps.Append(new Color() { Val = hexColor });
                runProps.Append(new Bold());
            }

            run.RunProperties = runProps;
            cell.Append(new Paragraph(run));
            return cell;
        }
    }
}