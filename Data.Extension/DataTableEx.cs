using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Data.Extension
{
    public static class DataTableEx
    {
        public static List<T> ConvertToList<T>(this DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            //To Do Get DisplayName attributes
            var properties = TypeDescriptor.GetProperties(typeof(T))
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                               .ToArray();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    var p = pro.Name.ToUpper();
                    var pType = pro.PropertyType;
                    if (columnNames.Any(aa => aa.ToUpper() == p))
                    {
                        var val = row[p];
                        if (val != DBNull.Value)
                        {
                            var targetType = TypeEx.IsNullableType(pro.PropertyType) ? Nullable.GetUnderlyingType(pro.PropertyType) : pro.PropertyType;
                            var propertyVal = Convert.ChangeType(row[pro.Name], targetType);
                            pro.SetValue(objT, propertyVal);

                        }

                    }

                }

                return objT;
            }).ToList();

        }
        public static List<T> ConvertToList<T>(this System.Data.DataTable dt, bool displayName)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();
            //To Do Get DisplayName attributes
            var properties = TypeDescriptor.GetProperties(typeof(T))
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo => propertyInfo.PropertyType.Namespace.Equals("System"))
                                               .ToArray();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    var displayNameAttr = typeof(T).GetDisplayNameAttributes<T>(pro.Name);

                    //var p = pro.Name.ToUpper();
                    var p = displayNameAttr.ToUpper();
                    var pType = pro.PropertyType;
                    if (columnNames.Any(aa => aa.ToUpper() == p))
                    {
                        var val = row[p];
                        if (val != DBNull.Value)
                        {
                            var targetType = TypeEx.IsNullableType(pro.PropertyType) ? Nullable.GetUnderlyingType(pro.PropertyType) : pro.PropertyType;
                            var propertyVal = Convert.ChangeType(row[pro.Name], targetType);
                            pro.SetValue(objT, propertyVal);

                        }
                    }

                }

                return objT;
            }).ToList();

        }
        public static List<T> ConvertToListOld<T>(this System.Data.DataTable dt)
        {
            var columnNames = dt.Columns.Cast<DataColumn>()
                .Select(c => c.ColumnName)
                .ToList();

            var properties = TypeDescriptor.GetProperties(typeof(T))
                                               .Cast<PropertyDescriptor>()
                                               .Where(propertyInfo =>
                                               propertyInfo.PropertyType.Namespace.Equals("System"))
                                               .ToArray();

            return dt.AsEnumerable().Select(row =>
            {
                var objT = Activator.CreateInstance<T>();

                foreach (var pro in properties)
                {
                    var p = pro.Name.ToUpper();
                    var pType = pro.PropertyType;
                    if (columnNames.Contains(p))
                    {
                        var val = row[p];
                        if (val != DBNull.Value)
                        {
                            int intVal;
                            DateTime dateVal;
                            if (pType == typeof(int) || pType == typeof(Nullable<int>))
                            {

                                int.TryParse((string)val, out intVal);
                                pro.SetValue(objT, intVal);
                            }
                            else if (pType == typeof(short) || pType == typeof(Nullable<short>))
                            {
                                short shortVal;
                                short.TryParse((string)val, out shortVal);

                                pro.SetValue(objT, shortVal);
                            }
                            else if (pType == typeof(decimal) || pType == typeof(Nullable<decimal>))
                            {
                                decimal decimalVal;
                                decimal.TryParse(val.ToString(), out decimalVal);

                                pro.SetValue(objT, decimalVal);
                            }
                            else if (pType == typeof(Nullable<DateTime>) || pType == typeof(DateTime))
                            {

                                DateTime.TryParse((string)val, out dateVal);

                                pro.SetValue(objT, dateVal);
                            }

                            else
                            {
                                pro.SetValue(objT, val);
                            }

                        }

                    }

                }

                return objT;
            }).ToList();

        }
    }
}
