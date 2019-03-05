using NewMao.Common.Mapper;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NewMao.Core.Extensions
{
    public static class ExcelExtension
    {
        public static int PriceDecimals { get; set; }
        public static int QtyDecimals { get; set; }

        public static string qtyFormat
        {
            get
            {
                return (QtyDecimals == 0 ? "#,##0" : "#,##0." + "0".PadRight(QtyDecimals));
            }
        }

        public static string priceFormat
        {
            get
            {
                return (PriceDecimals == 0 ? "#,##0" : "#,##0." + "0".PadRight(PriceDecimals));
            }
        }

        public static MemoryStream ToExcel<T>(this List<T> excelSource)
        {
            return ExcelExtension.ToExcel(excelSource, "");
        }

        public static MemoryStream ToExcel<T>(this List<T>excelSource,string HeaderPicPath)
        {
            var memoryStream = new MemoryStream();
            var excelSourcePropertyInfos = typeof(T).GetProperties();
            var excelPropertInfos = new Dictionary<ExcelAttribute, PropertyInfo>();

            foreach (var propertyInfo in excelSourcePropertyInfos)
            {
                var excelAttr = propertyInfo.GetCustomAttributes(typeof(ExcelAttribute), true).FirstOrDefault() as ExcelAttribute;
                if (excelAttr != null)
                {
                    excelPropertInfos.Add(excelAttr, propertyInfo);
                }
            }

            using (ExcelPackage pck = new ExcelPackage())
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Sheet1");

                ws.View.ShowGridLines = false;

                var imagRowSize = 0.0;

                if (!string.IsNullOrEmpty(HeaderPicPath))
                {
                    Image img = Image.FromFile(@HeaderPicPath);
                    int rowIndex = 0;
                    int colIndex = 0;
                    int PixelTop = 0;
                    int PixelLeft = 0;
                    int Height = img.Height;
                    int Width = img.Width;

                    ExcelPicture pic = ws.Drawings.AddPicture("Header", img);

                    imagRowSize = Math.Ceiling(img.Height / ws.DefaultRowHeight);
                    pic.SetPosition(rowIndex, 0, colIndex, 0);
                    pic.SetSize(Width, Height);
                }

                int headerRowSize = (int)imagRowSize;
                int i = 1;

                foreach (var title in excelPropertInfos.OrderBy(x => x.Key.ListOrder).Select(x => x.Key).ToList()) 
                {
                    var cell = ws.Cells[headerRowSize + 1, i];
                    cell.Value = title.Name;
                    cell.Style.Font.Bold = false;
                    cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    cell.Style.Fill.BackgroundColor.SetColor(Color.Gray);
                    cell.Style.Font.Color.SetColor(Color.White);

                    i++;
                }

                int rows = 2 + headerRowSize;
                foreach (var item in excelSource)
                {
                    int cols = 1;

                    foreach (var propertyInfo in excelPropertInfos.OrderBy(x => x.Key.ListOrder).Select(x => x.Value).ToList())
                    {
                        var cell = ws.Cells[rows, cols];
                        var excelAtt = excelPropertInfos.First(x => x.Value == propertyInfo).Key;
                        var propValue = propertyInfo.GetValue(item);

                        var propertyGetType = propertyInfo.GetType();
                        var propertType = propertyInfo.PropertyType;

                        if ((propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?)) && excelAtt.DataType == Common.Enum.ExcelDataType.PriceDecimals) 
                        {
                            cell.Style.Numberformat.Format = priceFormat;
                        }

                        if ((propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?)) && excelAtt.DataType == Common.Enum.ExcelDataType.QtyDecimals)
                        {
                            cell.Style.Numberformat.Format = qtyFormat;
                        }

                        cell.Value = propValue == null ? string.Empty : propValue;

                        cols++;
                    }
                    rows++;
                }
                memoryStream = new MemoryStream((AddHeaderPic(pck, HeaderPicPath)).GetAsByteArray());
            }
            return memoryStream;
        }

        public static MemoryStream OpenTemplate(string TemplatePath)
        {
            var memoryStream = new MemoryStream();
            if (!string.IsNullOrEmpty(TemplatePath) && File.Exists(TemplatePath))
            {
                using (FileStream fs = new FileStream(TemplatePath, FileMode.Open))
                {
                    fs.CopyTo(memoryStream);
                }
            }
            return memoryStream;
        }

        public static ExcelPackage AddHeaderPic(ExcelPackage pck,string HeaderPicPath)
        {
            if (!string.IsNullOrEmpty(HeaderPicPath) && File.Exists(HeaderPicPath))
            {
                ExcelWorksheet ws = pck.Workbook.Worksheets[1];
                var imagRowSize = 0.0;

                Image img = Image.FromFile(@HeaderPicPath);
                int rowIndex = 0;
                int ColIndex = 0;
                int Height = img.Height;
                int width = img.Width;

                ExcelPicture pic = ws.Drawings.AddPicture("Header", img);
                imagRowSize = Math.Ceiling(img.Height / ws.DefaultRowHeight);
                ws.InsertRow(1, (int)imagRowSize);
                pic.SetPosition(rowIndex, 0, ColIndex, 0);
                pic.SetSize(width, Height);
            }
            return pck;
        }

        public static FileInfo SaveExcelFile<T>(this List<T> excelSource, string fileFolder, string fileName)
        {
            string filePath = Path.Combine(fileFolder, fileName);

            if (!Directory.Exists(fileFolder))
            {
                Directory.CreateDirectory(fileFolder);
            }

            using (MemoryStream ms = excelSource.ToExcel()) 
            {
                using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate)) 
                {
                    ms.CopyTo(fs);
                    fs.Flush();
                }
            }

            FileInfo fileInfo = new FileInfo(filePath);

            return fileInfo;
        }
    }
}
