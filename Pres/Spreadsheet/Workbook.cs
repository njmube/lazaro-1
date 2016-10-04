using System;
using System.Collections.Generic;
using OfficeOpenXml;

namespace Lazaro.Pres.Spreadsheet
{
        public class Workbook
        {
                public System.Collections.Generic.List<Sheet> Sheets = new List<Sheet>();

                public void SaveTo(string fileName, SaveFormats format)
                {
                        SaveTo(fileName, format, null);
                }

                public void Print()
                {
                        this.Print(PrintModes.Automatic);
                }

                public void Print(PrintModes printMode)
                {
                        PrintModes FinalPrintMode = printMode;
                        if (FinalPrintMode == PrintModes.Automatic) {
                                if (ExternalPrograms.MsOffice.IsExcelInstalled())
                                        FinalPrintMode = PrintModes.MsOffice;
                                else if (ExternalPrograms.OpenOffice.IsCalcInstalled())
                                        FinalPrintMode = PrintModes.OpenOffice;
                        }

                        switch (FinalPrintMode) {
                                case PrintModes.OpenOffice:
                                        string TempCalcFileName = Lfx.Environment.Folders.TemporaryFolder + System.IO.Path.GetFileName(System.IO.Path.GetRandomFileName()) + ".xml";
                                        this.SaveTo(TempCalcFileName, SaveFormats.Excel);
                                        ExternalPrograms.OpenOffice.PrintWithCalc(TempCalcFileName);
                                        break;
                                case PrintModes.MsOffice:
                                        string TempExcelFileName = Lfx.Environment.Folders.TemporaryFolder + System.IO.Path.GetFileName(System.IO.Path.GetRandomFileName()) + ".xml";
                                        this.SaveTo(TempExcelFileName, SaveFormats.Excel);
                                        ExternalPrograms.MsOffice.PrintWithExcel(TempExcelFileName);
                                        break;
                                default:
                                        throw new NotImplementedException();
                        }
                }

                public void SaveTo(string fileName, SaveFormats format, Sheet sheet)
                {
                        using (var wr = new System.IO.StreamWriter(new System.IO.FileStream(fileName, System.IO.FileMode.Create)))
                        {
                                switch (format)
                                {
                                        case SaveFormats.Excel:
                                                this.ToExcel(wr, sheet);
                                                break;
                                        //case SaveFormats.ExcelXml:
                                        //        this.ToExcelXml(wr, sheet);
                                        //        break;
                                        case SaveFormats.Html:
                                                this.ToHtml(wr, sheet);
                                                break;
                                }
                                wr.Close();
                        }
                }

                protected internal void ToHtml(System.IO.StreamWriter wr, Sheet sheet)
                {
                        wr.WriteLine(@"<!DOCTYPE html>");
                        wr.WriteLine(@"<html>");
                        wr.WriteLine(@"<head>");
                        if (sheet == null) {
                                if (this.Sheets.Count > 0) {
                                        wr.WriteLine(@"<title>" + this.Sheets[0].Name + @"</title>");
                                }
                        } else {
                                wr.WriteLine(@"<title>" + sheet.Name + @"</title>");
                        }
                        wr.WriteLine(@"<meta charset=""utf-8"" />");
                        wr.WriteLine(@"<meta name=""generator"" content=""Lázaro (www.lazarogestion.com)"">");
                        wr.WriteLine(@"<meta http-equiv=""Content-Type"" content=""text/html; charset=UTF-8"" />");
                        wr.WriteLine(@"<style>");
                        wr.WriteLine(@"@media print { body { margin: 0; padding: 0 } }");
                        wr.WriteLine(@"body, table, td { font-family: Open Sans, Segoe UI, Trebuchet MS, Helvetica, Sans Serif; font-size: 10pt; }");
                        wr.WriteLine(@".StyleTable, table { border-collapse: collapse; empty-cells: show; border: none; }");
                        wr.WriteLine(@".StyleTableCaption, caption { font-size: large; background-color: #C5D900; padding: 4px; }");
                        wr.WriteLine(@".StyleTableHead, thead { display: table-header-group; }");
                        wr.WriteLine(@".StyleColumnHeader, th { padding: 2px; background-color: #C5D9F1; font-weight: bold; }");
                        wr.WriteLine(@".StyleDataRow, tr { page-break-inside: avoid; background-color: white; border-bottom: 1px solid silver; }");
                        wr.WriteLine(@".StyleDataCell, td { padding: 2px; }");
                        wr.WriteLine(@"</style>");
                        wr.WriteLine(@"</head>");

                        wr.WriteLine("<body>");
                        if (sheet == null) {
                                //All sheets
                                foreach (Sheet sht in this.Sheets) {
                                        sht.ToHtml(wr);
                                }
                        } else {
                                //One sheet
                                sheet.ToHtml(wr);
                        }

                        wr.WriteLine("</body>");
                        wr.WriteLine("</html>");
                }


                protected void ToExcel(System.IO.StreamWriter wr, Sheet sheet)
                {
                        using (var Pkg = new ExcelPackage()) {
                                if (sheet == null) {
                                        //All sheets
                                        foreach (Sheet sht in this.Sheets) {
                                                ExcelWorksheet worksheet = Pkg.Workbook.Worksheets.Add(sht.Name);
                                                sht.ToExcel(worksheet);
                                        }
                                } else {
                                        //One sheet
                                        ExcelWorksheet worksheet = Pkg.Workbook.Worksheets.Add(sheet.Name);
                                        sheet.ToExcel(worksheet);
                                }

                                Pkg.SaveAs(wr.BaseStream);
                        }

                        return;
                }


                protected internal void ToExcelXml(System.IO.StreamWriter wr, Sheet sheet)
                {
                        wr.WriteLine(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
                        wr.WriteLine(@"<?mso-application progid=""Excel.Sheet""?>");
                        wr.WriteLine(@"<Workbook xmlns=""urn:schemas-microsoft-com:office:spreadsheet"" xmlns:c=""urn:schemas-microsoft-com:office:component:spreadsheet"" xmlns:html=""http://www.w3.org/TR/REC-html40"" xmlns:o=""urn:schemas-microsoft-com:office:office"" xmlns:ss=""urn:schemas-microsoft-com:office:spreadsheet"" xmlns:x2=""http://schemas.microsoft.com/office/excel/2003/xml"" xmlns:x=""urn:schemas-microsoft-com:office:excel"" xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"">");
                        wr.WriteLine(@"  <Styles>
    <Style ss:ID=""Default"" ss:Name=""Normal"">
      <Alignment ss:Vertical=""Bottom"" />
    </Style>
    <Style ss:ID=""StyleData"">
      <Font x:FontName=""Segoe IU"" />
    </Style>
    <Style ss:ID=""StyleHeader"">
      <Font x:FontName=""Segoe IU"" ss:Bold=""1"" />
      <Interior ss:Color=""#C5D9F1"" ss:Pattern=""Solid"" />
    </Style>
   </Styles>");

                        if (sheet == null) {
                                //All sheets
                                foreach (Sheet sht in Sheets) {
                                        sht.ToExcelXml(wr);
                                }
                        } else {
                                //One sheet
                                sheet.ToExcelXml(wr);
                        }

                        wr.WriteLine("</Workbook>");
                }

                public Sheet GetSheetByName(string name)
                {
                        foreach (Sheet sht in Sheets) {

                                if (sht.Name == name) {
                                        return sht;
                                }
                        }
                        return null;
                }
        }
}
