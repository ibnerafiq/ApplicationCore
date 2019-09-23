using Microsoft.IdentityModel.Protocols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Extension
{
    public static class StringEx
    {
        public static bool IsNullOrEmpty(this string val)
        {
            return val == null || val == string.Empty;
        }
        //public static string GetADOConnectionString(this string conName)
        //{
        //    return ConfigurationManager.ConnectionStrings[conName + "_ADO"].ConnectionString;

        //}

        //public static string GetConfigKey(this string val)
        //{
        //    return ConfigurationManager.AppSettings[val];
        //}
        //public static T GetConfigKey<T>(this string key, T defaultValue)
        //{
        //    return ConfigurationManager.AppSettings[key].IsNullOrEmpty() ?
        //    defaultValue : (T)Convert.ChangeType((object)ConfigurationManager.AppSettings[key], typeof(T));
        //}
        //public static string Connectionstring(this string connectionStringName)
        //{
        //    if (connectionStringName.IsNullOrEmpty())
        //    {
        //        throw new Exception("conncectionstring name is empty or null");
        //    }

           
        //    if (ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString.IsNullOrEmpty())
        //    {
        //        throw new Exception("connectionstring with name " + connectionStringName + " does not exist");
        //    }

        //    return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
        //}
        public static string KashfClientId(this string Id)
        {

            Id = Id.PadLeft(8, '0');
            return Id;

        }
        public static string KashfBranchId(this string Id)
        {
            Id = Id.PadLeft(4, '0');
            return Id;

        }

        public static List<string> AddZeroToList(this string number, List<string> list)
        {
            if (list != null && list.Count > 0)
            {
                list.ForEach(aa => aa = aa.PadLeft(11, '0'));
            }

            return list;
        }
        public static string AddZero(this string number)
        {

            number = number.PadLeft(11, '0').ToString();
            return number;
        }
        public static string ToDelimitedString<T>(this IEnumerable<T> lst, string separator = ", ")
        {
            return lst.ToDelimitedString(p => p, separator);
        }

        public static string ToDelimitedString<S, T>(this IEnumerable<S> lst, Func<S, T> selector,
                                                     string separator = ", ")
        {
            return string.Join(separator, lst.Select(selector));
        }
        public static Expression<Func<T, object>> LambdaFromProperty<T>(string propertyName)
        {
            var arg = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(arg, propertyName);
            //return the property as object
            var conv = Expression.Convert(property, typeof(object));
            var exp = Expression.Lambda<Func<T, object>>(conv, new ParameterExpression[] { arg });
            return exp;
            //var exp = LambdaFromProperty<Person>("Name");//exp: x=>x.Name
        }
    }
}
