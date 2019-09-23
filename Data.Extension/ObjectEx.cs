using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Extension
{
    public static class ObjectEx
    {
        public static void AssignBasicProperties<T>(this T type)
        {
            //var type = tpe.GetType();
            var now = DateTime.Now;
            type.GetType().GetProperty("CreatedOn").SetValue(type, now);
            type.GetType().GetProperty("ModifiedOn").SetValue(type, now);
        }
        public static void ModifyMe<T>(this T type)
        {
            //var type = tpe.GetType();
            var now = DateTime.Now;
            //type.GetProperty("CreatedOn").SetValue(type, now);
            type.GetType().GetProperty("ModifiedOn").SetValue(type, now);
        }

        public static bool NotNullOrEmpty<T>(this T obj)
        {
            return obj != null && !string.IsNullOrEmpty(obj.ToString());
        }
    }
}

