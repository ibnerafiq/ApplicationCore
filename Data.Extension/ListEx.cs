using Data.Core.BaseService;
using Data.Core.Paging;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Office.Interop.Excel;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Data.Extension
{
    public static class ListEx
    {
        #region Paging
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> source, int page = 1, int pageSize = 10) where T : class
        {
            return new PagedList<T>(source, page, pageSize);
        }

        public static PagedList<T> ToPagedList<T>(this List<T> source, int page = 1, int pageSize = 10) where T : class
        {
            return new PagedList<T>(source, page, pageSize);
        }
        public static PagedList<T> ToPagedList<T>(this IEnumerable<T> source, int page = 1, int pageSize = 10) where T : class
        {
            return new PagedList<T>(source, page, pageSize);
        }
        #endregion

        public static bool HaveRecords<T>(this List<T> list)
        {
            return list != null && list.Count > 0;
        }
        public static bool HaveRecords<T>(this HashSet<T> list)
        {
            return list != null && list.Count > 0;
        }

        public static List<SelectListItem> ToSelectList<T>(this List<T> entities, string typeId, string typeName) where T : class
        {


            //  var uow = new UnitOfWork();
            List<SelectListItem> listItems = new List<SelectListItem>();

            entities.ForEach(tt => listItems.Add(new SelectListItem
            {
                Value = tt.GetType().GetProperty(typeId).GetValue(tt, null).ToString(),
                Text = tt.GetType().GetProperty(typeName).GetValue(tt, null).ToString().ToUpper()

            }));

            return listItems;
        }

        public static void BulkInsert<T>(this IList<T> list, string connection, string tableName)
        {
            try
            {
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.DestinationTableName = tableName;
                    var table = new System.Data.DataTable();



                    var props = TypeDescriptor.GetProperties(typeof(T))
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System")).ToArray();




                    foreach (var propertyInfo in props)
                    {
                        //var displayName = typeof(T).GetDisplayNameAttributes<T>(propertyInfo.Name);

                        bulkCopy.ColumnMappings.Add(propertyInfo.Name, propertyInfo.Name);
                        table.Columns.Add(propertyInfo.Name,
                            Nullable.GetUnderlyingType(propertyInfo.PropertyType) ??
                            propertyInfo.PropertyType);
                    }

                    var values = new object[props.Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }
                    bulkCopy.WriteToServer(table);
                }


            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }
        public static void BulkInsert<T>(this IList<T> list, string connection, string tableName, bool buddyClass)
        {
            try
            {
                using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.UseInternalTransaction))
                {
                    bulkCopy.BatchSize = 1000;
                    bulkCopy.DestinationTableName = tableName;
                    var table = new System.Data.DataTable();
                    var props = typeof(T).GetDisplayNameAndType<T>(true);

                    foreach (var propertyInfo in props)
                    {
                        bulkCopy.ColumnMappings.Add(propertyInfo.Key, propertyInfo.Key);
                        table.Columns.Add(propertyInfo.Key,
                            Nullable.GetUnderlyingType(propertyInfo.Value) ??
                            propertyInfo.Value);
                    }

                    var values = new object[typeof(T).GetProperties().Length];
                    foreach (var item in list)
                    {
                        for (var i = 0; i < values.Length; i++)
                        {
                            values[i] = props[props.Keys.ToList()[i]];//  props[i].GetValue(item);
                        }

                        table.Rows.Add(values);
                    }
                    bulkCopy.WriteToServer(table);
                }


            }
            catch (Exception ex)
            {
                var message = ex.Message;
            }
        }
        public static void LoopExcelFile()
        {

            var app = new Microsoft.Office.Interop.Excel.Application();
            app.Visible = true;
            var books = (Microsoft.Office.Interop.Excel.Workbooks)app.Workbooks;
            for (var i = 1; i < books.Count; i++)
            {
                var book = books.get_Item(i);
                var name = book.FullName;
            }

        }
        public static void ToExcel<T>(List<T> list, Microsoft.Office.Interop.Excel.Workbook file)
        {
            var sheet = file.Sheets.Add();

        }
        public static void CreateExcelFile()
        {
            object optionalValue = Missing.Value;
            var excelApp = new Microsoft.Office.Interop.Excel.Application();
            var books = (Microsoft.Office.Interop.Excel.Workbooks)excelApp.Workbooks;

            var book = (Microsoft.Office.Interop.Excel._Workbook)(books.Add(optionalValue));
            book.SaveAs("D:\\Kszb Unique");
            excelApp.Quit();


        }
        public static Microsoft.Office.Interop.Excel.Workbook GetExcelFile(string fileName = "Unique Client")
        {
            var file = "D:\\" + fileName;
            var app = new Microsoft.Office.Interop.Excel.Application();
            var book = app.Workbooks.Open(file);
            return book;
        }

        public static void ToExcel<T>(this List<T> list, string FilePath, string sheetName)
        {
            #region Declarations

            if (string.IsNullOrEmpty(FilePath))
            {
                throw new Exception("Invalid file path.");
            }
            else if (FilePath.ToLower().Contains("") == false)
            {
                throw new Exception("Invalid file path.");
            }

            if (list == null)
            {
                throw new Exception("No data to export.");
            }

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbooks books = null;
            Microsoft.Office.Interop.Excel._Workbook book = null;
            Microsoft.Office.Interop.Excel.Sheets sheets = null;
            Microsoft.Office.Interop.Excel._Worksheet sheet = null;
            Microsoft.Office.Interop.Excel.Range range = null;
            Microsoft.Office.Interop.Excel.Font font = null;
            // Optional argument variable
            object optionalValue = Missing.Value;

            string strHeaderStart = "A1";
            string strDataStart = "A2";
            #endregion

            #region Processing


            try
            {
                #region Init Excel app.


                excelApp = new Microsoft.Office.Interop.Excel.Application();
                books = (Microsoft.Office.Interop.Excel.Workbooks)excelApp.Workbooks;
                book = excelApp.Workbooks.Open(FilePath);
                //book = (Microsoft.Office.Interop.Excel._Workbook)(books.Add(optionalValue));

                //sheets = (Microsoft.Office.Interop.Excel.Sheets)book.Worksheets;

                //sheet = (Microsoft.Office.Interop.Excel._Worksheet)(sheets.get_Item(1));

                #endregion

                #region Creating Header


                Dictionary<string, string> objHeaders = new Dictionary<string, string>();

                PropertyInfo[] headerInfo = typeof(T).GetProperties();


                foreach (var property in headerInfo)
                {
                    var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                            .Cast<DisplayNameAttribute>().FirstOrDefault();
                    objHeaders.Add(property.Name, attribute == null ?
                                        property.Name : attribute.DisplayName);
                }
                #endregion

                // foreach (var region in regionList)
                //{


                sheet = (Worksheet)book.Worksheets.get_Item(sheetName);
                if (sheet == null)
                {
                    sheet = (Worksheet)book.Sheets.Add();
                    sheet.Name = sheetName;
                }

                range = sheet.get_Range(strHeaderStart, optionalValue);
                range = range.get_Resize(1, objHeaders.Count);

                range.set_Value(optionalValue, objHeaders.Values.ToArray());
                // range.BorderAround(Type.Missing, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                font = range.Font;
                font.Bold = true;
                // range.Interior.Color = Color.LightGray.ToArgb();
                #region Writing data to cell

                //var mList = list.Where(xx => (string)xx.GetType().GetProperty(propertyName).GetValue(xx, null) == region).ToList();
                int count = list.Count;
                object[,] objData = new object[count, objHeaders.Count];

                for (int j = 0; j < count; j++)
                {
                    var item = list[j];
                    int i = 0;
                    foreach (KeyValuePair<string, string> entry in objHeaders)
                    {
                        var y = typeof(T).InvokeMember(entry.Key.ToString(), BindingFlags.GetProperty, null, item, null);
                        objData[j, i++] = (y == null) ? "" : y.ToString();
                    }
                }

                // sheet.get_Range()
                var lastCell = sheet.get_Range("A1048576", optionalValue);
                var add = lastCell.Address;
                lastCell = lastCell.End[XlDirection.xlUp].get_Offset(1);
                var address = lastCell.Address;
                range = sheet.get_Range(lastCell.Address, optionalValue);
                range = range.get_Resize(count, objHeaders.Count);

                range.set_Value(optionalValue, objData);
                //range.BorderAround(Type.Missing, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                range = sheet.get_Range(strHeaderStart, optionalValue);
                range = range.get_Resize(count + 1, objHeaders.Count);
                range.Columns.AutoFit();
                //excelApp.Visible = true;                 
                book.Close(true);
                sheet = null;
                #endregion

                //}






                #region Saving data and Opening Excel file.


                //if (string.IsNullOrEmpty(FilePath) == false)
                //     book.SaveAs(FilePath);


                excelApp.Visible = true;

                #endregion

                #region Release objects

                try
                {
                    if (sheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                    sheet = null;

                    if (sheets != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                    sheets = null;

                    if (book != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                    book = null;

                    if (books != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(books);
                    books = null;
                    excelApp.Quit();
                    if (excelApp != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);

                    excelApp = null;
                }
                catch (Exception ex)
                {
                    sheet = null;
                    sheets = null;
                    book = null;
                    books = null;
                    excelApp = null;
                }
                finally
                {

                    GC.Collect();
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion
        }
        public static void ToExcel<T>(this List<T> list, string PathToSave, List<string> regionList, string propertyName)
        {
            #region Declarations

            if (string.IsNullOrEmpty(PathToSave))
            {
                throw new Exception("Invalid file path.");
            }
            else if (PathToSave.ToLower().Contains("") == false)
            {
                throw new Exception("Invalid file path.");
            }

            if (list == null)
            {
                throw new Exception("No data to export.");
            }

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbooks books = null;
            Microsoft.Office.Interop.Excel._Workbook book = null;
            Microsoft.Office.Interop.Excel.Sheets sheets = null;
            Microsoft.Office.Interop.Excel._Worksheet sheet = null;
            Microsoft.Office.Interop.Excel.Range range = null;
            Microsoft.Office.Interop.Excel.Font font = null;
            // Optional argument variable
            object optionalValue = Missing.Value;

            string strHeaderStart = "A1";
            string strDataStart = "A2";
            #endregion

            #region Processing


            try
            {
                #region Init Excel app.


                excelApp = new Microsoft.Office.Interop.Excel.Application();
                books = (Microsoft.Office.Interop.Excel.Workbooks)excelApp.Workbooks;
                book = (Microsoft.Office.Interop.Excel._Workbook)(books.Add(optionalValue));

                //sheets = (Microsoft.Office.Interop.Excel.Sheets)book.Worksheets;

                //sheet = (Microsoft.Office.Interop.Excel._Worksheet)(sheets.get_Item(1));

                #endregion

                #region Creating Header


                Dictionary<string, string> objHeaders = new Dictionary<string, string>();

                PropertyInfo[] headerInfo = typeof(T).GetProperties();


                foreach (var property in headerInfo)
                {
                    var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                            .Cast<DisplayNameAttribute>().FirstOrDefault();
                    objHeaders.Add(property.Name, attribute == null ?
                                        property.Name : attribute.DisplayName);
                }
                #endregion

                foreach (var region in regionList)
                {
                    sheet = (Worksheet)book.Sheets.get_Item(region);

                    sheet = (Worksheet)book.Sheets.Add();
                    sheet.Name = region;



                    range = sheet.get_Range(strHeaderStart, optionalValue);
                    range = range.get_Resize(1, objHeaders.Count);

                    range.set_Value(optionalValue, objHeaders.Values.ToArray());
                    // range.BorderAround(Type.Missing, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                    font = range.Font;
                    font.Bold = true;
                    // range.Interior.Color = Color.LightGray.ToArgb();
                    #region Writing data to cell

                    var mList = list.Where(xx => (string)xx.GetType().GetProperty(propertyName).GetValue(xx, null) == region).ToList();
                    int count = mList.Count;
                    object[,] objData = new object[count, objHeaders.Count];

                    for (int j = 0; j < count; j++)
                    {
                        var item = mList[j];
                        int i = 0;
                        foreach (KeyValuePair<string, string> entry in objHeaders)
                        {
                            var y = typeof(T).InvokeMember(entry.Key.ToString(), BindingFlags.GetProperty, null, item, null);
                            objData[j, i++] = (y == null) ? "" : y.ToString();
                        }
                    }


                    range = sheet.get_Range(strDataStart, optionalValue);
                    range = range.get_Resize(count, objHeaders.Count);

                    range.set_Value(optionalValue, objData);
                    //range.BorderAround(Type.Missing, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                    range = sheet.get_Range(strHeaderStart, optionalValue);
                    range = range.get_Resize(count + 1, objHeaders.Count);
                    range.Columns.AutoFit();
                    sheet = null;
                    #endregion

                }






                #region Saving data and Opening Excel file.


                if (string.IsNullOrEmpty(PathToSave) == false)
                    book.SaveAs(PathToSave);

                excelApp.Visible = true;

                #endregion

                #region Release objects

                try
                {
                    if (sheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                    sheet = null;

                    if (sheets != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                    sheets = null;

                    if (book != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                    book = null;

                    if (books != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(books);
                    books = null;

                    if (excelApp != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                }
                catch (Exception ex)
                {
                    sheet = null;
                    sheets = null;
                    book = null;
                    books = null;
                    excelApp = null;
                }
                finally
                {

                    GC.Collect();
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion
        }
        /// <summary>
        /// Extension method to write list data to excel.
        /// </summary>
        /// <typeparam name="T">Ganeric list</typeparam>
        /// <param name="list"></param>
        /// <param name="PathToSave">Path to save file.</param>
        public static void ToExcel<T>(this List<T> list, string PathToSave)
        {
            #region Declarations

            if (string.IsNullOrEmpty(PathToSave))
            {
                throw new Exception("Invalid file path.");
            }
            else if (PathToSave.ToLower().Contains("") == false)
            {
                throw new Exception("Invalid file path.");
            }

            if (list == null)
            {
                throw new Exception("No data to export.");
            }

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Microsoft.Office.Interop.Excel.Workbooks books = null;
            Microsoft.Office.Interop.Excel._Workbook book = null;
            Microsoft.Office.Interop.Excel.Sheets sheets = null;
            Microsoft.Office.Interop.Excel._Worksheet sheet = null;
            Microsoft.Office.Interop.Excel.Range range = null;
            Microsoft.Office.Interop.Excel.Font font = null;
            // Optional argument variable
            object optionalValue = Missing.Value;

            string strHeaderStart = "A1";
            string strDataStart = "A2";
            #endregion

            #region Processing


            try
            {
                #region Init Excel app.


                excelApp = new Microsoft.Office.Interop.Excel.Application();
                books = (Microsoft.Office.Interop.Excel.Workbooks)excelApp.Workbooks;
                book = (Microsoft.Office.Interop.Excel._Workbook)(books.Add(optionalValue));
                sheets = (Microsoft.Office.Interop.Excel.Sheets)book.Worksheets;
                sheet = (Microsoft.Office.Interop.Excel._Worksheet)(sheets.get_Item(1));

                #endregion

                #region Creating Header


                Dictionary<string, string> objHeaders = new Dictionary<string, string>();

                PropertyInfo[] headerInfo = typeof(T).GetProperties();


                foreach (var property in headerInfo)
                {
                    var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), false)
                                            .Cast<DisplayNameAttribute>().FirstOrDefault();
                    objHeaders.Add(property.Name, attribute == null ?
                                        property.Name : attribute.DisplayName);
                }


                range = sheet.get_Range(strHeaderStart, optionalValue);
                range = range.get_Resize(1, objHeaders.Count);

                range.set_Value(optionalValue, objHeaders.Values.ToArray());
                // range.BorderAround(Type.Missing, Microsoft.Office.Interop.Excel.XlBorderWeight.xlThin, Microsoft.Office.Interop.Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                font = range.Font;
                font.Bold = true;
                // range.Interior.Color = Color.LightGray.ToArgb();

                #endregion

                #region Writing data to cell


                int count = list.Count;
                object[,] objData = new object[count, objHeaders.Count];

                for (int j = 0; j < count; j++)
                {
                    var item = list[j];
                    int i = 0;
                    foreach (KeyValuePair<string, string> entry in objHeaders)
                    {
                        var y = typeof(T).InvokeMember(entry.Key.ToString(), BindingFlags.GetProperty, null, item, null);
                        objData[j, i++] = (y == null) ? "" : y.ToString();
                    }
                }


                range = sheet.get_Range(strDataStart, optionalValue);
                range = range.get_Resize(count, objHeaders.Count);

                range.set_Value(optionalValue, objData);
                //range.BorderAround(Type.Missing, Excel.XlBorderWeight.xlThin, Excel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);

                range = sheet.get_Range(strHeaderStart, optionalValue);
                range = range.get_Resize(count + 1, objHeaders.Count);
                range.Columns.AutoFit();

                #endregion

                #region Saving data and Opening Excel file.


                if (string.IsNullOrEmpty(PathToSave) == false)
                    book.SaveAs(PathToSave);

                excelApp.Visible = true;

                #endregion

                #region Release objects

                try
                {
                    if (sheet != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheet);
                    sheet = null;

                    if (sheets != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(sheets);
                    sheets = null;

                    if (book != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(book);
                    book = null;

                    if (books != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(books);
                    books = null;

                    if (excelApp != null)
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                }
                catch (Exception ex)
                {
                    sheet = null;
                    sheets = null;
                    book = null;
                    books = null;
                    excelApp = null;
                }
                finally
                {
                    excelApp.Quit();
                    GC.Collect();
                }

                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }


            #endregion
        }
        public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> items, Func<T, TKey> property)
        {
            return items.GroupBy(property).Select(x => x.First());
        }

        public static System.Data.DataTable ToDataTable<T>(this IList<T> data)
        {
            //PropertyDescriptorCollection properties =
            //    TypeDescriptor.GetProperties(typeof(T));

            var properties = typeof(T).GetProperties().Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                              .OrderBy(xx => xx.MetadataToken)
                                              .ToArray();


            //var properties = TypeDescriptor.GetProperties(typeof(T))
            //                                  .Cast<PropertyDescriptor>()
            //                                  .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
            //                                  //.OrderBy(xx => xx.MetadataToken)
            //                                  .ToArray();

            System.Data.DataTable table = new System.Data.DataTable();
            foreach (var prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (var prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static Result<MemoryStream> ExportToExcel<T>(this IList<T> list)
        {
            MemoryStream excelStream = new MemoryStream();
            var properties = TypeDescriptor.GetProperties(typeof(T))
                 .Cast<PropertyDescriptor>()
                 .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                 .ToArray();
            var totalItems = list.Count;

            try
            {
                using (ExcelPackage excel = new ExcelPackage())
                {
                    var wk = excel.Workbook.Worksheets.Add("Sheet1");
                    wk.DefaultColWidth = 10;
                    wk.Cells.Style.WrapText = true;
                    var totalColumns = properties.Count();
                    for (var col = 0; col < totalColumns; col++)
                    {
                        wk.Cells[1, col + 1].Value = properties[col].Name;

                    }

                    using (var range = wk.Cells[1, 1, 1, totalColumns])
                    {
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(color: Color.Gray);
                        range.Style.Border.Bottom.Style = OfficeOpenXml.Style.ExcelBorderStyle.Thick;
                        range.Style.Border.Bottom.Color.SetColor(Color.Black);
                        range.Style.Font.Color.SetColor(Color.Black);
                        range.Style.Font.SetFromFont(new System.Drawing.Font("Arial", 10));
                        range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.CenterContinuous;

                    }

                    for (var index = 0; index < totalItems; index++)
                    {
                        var currentRecord = list[index];
                        for (var propIndex = 0; propIndex < totalColumns; propIndex++)
                        {
                            var val = currentRecord.GetType().GetProperty(properties[propIndex].Name).GetValue(currentRecord);
                            if (val != null)
                            {
                                var targetType = TypeEx.IsNullableType(properties[propIndex].PropertyType) ? Nullable.GetUnderlyingType(properties[propIndex].PropertyType) : properties[propIndex].PropertyType;
                                var propertyVal = Convert.ChangeType(val, targetType);
                                wk.Cells[index + 2, propIndex + 1].Value = propertyVal;
                                if (targetType == typeof(DateTime))
                                {
                                    wk.Cells[index + 2, propIndex + 1].Style.Numberformat.Format = "mmm/dd/yyyy";
                                    wk.Cells[index + 2, propIndex + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                                }


                            }
                        }
                    }



                    excel.Save();
                    excelStream = excel.Stream as MemoryStream;
                }

            }
            catch (Exception ex)
            {
                return new Result<MemoryStream>
                {

                    Data = null,
                    Message = ex.Message,
                    ResultType = ResultType.Exception

                };
            }

            return new Result<MemoryStream>
            {

                Data = new MemoryStream(excelStream.ToArray()),
                Message = ResultType.Success.ToString(),
                ResultType = ResultType.Success

            };


        }
    }
}
