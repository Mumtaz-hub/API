using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using ExcelDataReader;
using OfficeOpenXml;

namespace Extensions
{
    public static class ExcelExtensions
    {
        public static byte[] ToExcel<T>(this IEnumerable<T> excelData, string sheetName = "Sheet1")
        {
            // LicenseContext of the ExcelPackage class:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using ExcelPackage pck = new ExcelPackage();
            //Create the worksheet
            ExcelWorksheet excelWorksheet = pck.Workbook.Worksheets.Add(sheetName);

            //get the column headings
            var headings = GetHeader<T>();
            CreateHeader(headings, excelWorksheet);

            var columnCount = headings.Length;

            var modelCells = excelWorksheet.Cells["A1"];
            var infoResponseResults = excelData as T[] ?? excelData.ToArray();
            var modelRows = infoResponseResults.Length + 1;
            string modelRange = $"A1:{ColumnIndexToColumnLetter(columnCount)}{modelRows}";
            var modelTable = excelWorksheet.Cells[modelRange];

            // Fill worksheet with data to export
            modelCells.LoadFromCollection(infoResponseResults, true);
            FormatCells(excelWorksheet, headings, infoResponseResults.Length);
            modelTable.AutoFitColumns();
            return pck.GetAsByteArray();
        }

        public static byte[] ToExcel<T>(this Dictionary<string, IEnumerable<T>> excelData)
        {
            // LicenseContext of the ExcelPackage class:
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            ExcelPackage excelPackage = new ExcelPackage();
            foreach (var data in excelData)
            {
                //Create the worksheet
                var excelWorksheet = excelPackage.Workbook.Worksheets.Add(data.Key);

                //get the column headings
                var headings = GetHeader<T>();
                CreateHeader(headings, excelWorksheet);

                var columnCount = headings.Length;

                var modelCells = excelWorksheet.Cells["A1"];
                var infoResponseResults = data.Value.ToArray();
                var modelRows = infoResponseResults.Length + 1;
                string modelRange = $"A1:{ColumnIndexToColumnLetter(columnCount)}{modelRows}";
                var modelTable = excelWorksheet.Cells[modelRange];

                // Fill worksheet with data to export
                modelCells.LoadFromCollection(infoResponseResults, true);
                FormatCells(excelWorksheet, headings, infoResponseResults.Length);
                modelTable.AutoFitColumns();
            }
            var resultArray = excelPackage.GetAsByteArray();
            return resultArray;
        }

        public static DataSet ToDataSet(this byte[] fileBytes)
        {
            using Stream fileStream = new MemoryStream(fileBytes);
            using var reader = ExcelReaderFactory.CreateReader(fileStream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = tableReader => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });
            return dataSet;
        }

        public static DataSet ToDataSet(this FileInfo filePath)
        {
            using Stream fileStream = File.Open(filePath.FullName, FileMode.Open, FileAccess.Read);
            using var reader = ExcelReaderFactory.CreateReader(fileStream);
            var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
            {
                ConfigureDataTable = tableReader => new ExcelDataTableConfiguration
                {
                    UseHeaderRow = true
                }
            });
            return dataSet;
        }

        private static PropertyInfo[] GetHeader<T>()
        {
            var t = typeof(T);
            return t.GetProperties();
        }

        private static void CreateHeader(PropertyInfo[] headings, ExcelWorksheet excelWorksheet)
        {
            for (int i = 0; i < headings.Length; i++)
            {
                excelWorksheet.Cells[1, i + 1].Value = headings[i].Name;
            }
        }

        private static string ColumnIndexToColumnLetter(int colIndex)
        {
            var div = colIndex;
            var colLetter = string.Empty;

            while (div > 0)
            {
                var mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (div - mod) / 26;
            }
            return colLetter;
        }

        private static void FormatCells(ExcelWorksheet excelWorksheet, PropertyInfo[] headings, int rowCount)
        {
            for (int i = 0; i < headings.Length; i++)
            {
                TypeCode typeCode = Type.GetTypeCode(headings[i].PropertyType);
                switch (typeCode)
                {
                    case TypeCode.DateTime:
                        FormatCells(excelWorksheet, i, rowCount, "mm/dd/yyyy hh:mm:ss AM/PM");
                        break;
                    case TypeCode.Object:
                        if (headings[i].PropertyType == typeof(DateTime?))
                            FormatCells(excelWorksheet, i, rowCount, "mm/dd/yyyy hh:mm:ss AM/PM");
                        break;
                }
            }
        }

        private static void FormatCells(ExcelWorksheet excelWorksheet, int index, int rowCount, string cellFormat)
        {
            var cellRange = $"{ColumnIndexToColumnLetter(index + 1)}1:{ColumnIndexToColumnLetter(index + 1)}{rowCount + 1}";
            var formatRange = excelWorksheet.Cells[cellRange];
            formatRange.Style.Numberformat.Format = cellFormat;
        }
    }
}
