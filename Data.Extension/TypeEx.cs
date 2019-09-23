using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Data.Extension
{
    public static class TypeEx
    {
        public static PropertyInfo GetPropertyValue(this Type obj, string propertyName)
        {
            PropertyInfo value = null;

            if (propertyName.Contains("."))
            {
                var _propertyNames = propertyName.Split('.');
                var targetObject = obj.GetProperty(_propertyNames[0]).PropertyType;
                var propName = _propertyNames[1];

                value = targetObject.GetProperty(propName);
            }
            else
            {
                value = obj.GetProperty(propertyName);
            }


            return value;
        }
        public static IEnumerable<Type> GetBaseClassesAndInterfaces(this Type type)
        {
            return type.BaseType == typeof(object)
                ? type.GetInterfaces()
                : Enumerable
                    .Repeat(type.BaseType, 1)
                    .Concat(type.GetInterfaces())
                    .Concat(type.BaseType.GetBaseClassesAndInterfaces())
                    .Distinct();
        }
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        public static string[] GetDisplayNameAttributes<T>(this Type t)
        {
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            bool isDisplayNameAttributeDefined = false;
            string[] headers = new string[Props.Length];
            int colCount = 0;

            foreach (var prop in Props)
            {
                isDisplayNameAttributeDefined = Attribute.IsDefined(prop, typeof(DisplayNameAttribute));
                if (isDisplayNameAttributeDefined)
                {
                    DisplayNameAttribute dna = (DisplayNameAttribute)Attribute.GetCustomAttribute(prop, typeof(DisplayNameAttribute));
                    if (dna != null)
                    {
                        headers[colCount] = dna.DisplayName;
                    }

                }
                else
                {
                    headers[colCount] = prop.Name;
                }
                colCount++;
                isDisplayNameAttributeDefined = false;
            }

            return headers;
        }
        //http://codekea.com/yN0bxvE07gnx/c-buddy-classes-meta-data-and-reflection.html
        public static string[] GetDisplayNameAttributes<T>(this Type t, bool fromBuddyClass)
        {

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            bool isDisplayNameAttributeDefined = false;
            string[] headers = new string[Props.Length];
            int colCount = 0;

            foreach (var prop in Props)
            {
                isDisplayNameAttributeDefined = Attribute.IsDefined(prop, typeof(DisplayAttribute));
                if (isDisplayNameAttributeDefined)
                {
                    DisplayAttribute dna = (DisplayAttribute)Attribute.GetCustomAttribute(prop, typeof(DisplayAttribute));
                    if (dna != null)
                    {
                        headers[colCount] = dna.Name;
                    }

                }
                else if (!isDisplayNameAttributeDefined)
                {
                  ModelMetadataTypeAttribute [] metaAttibs = (ModelMetadataTypeAttribute[])typeof(T).GetCustomAttributes(typeof(ModelMetadataTypeAttribute), true);

                    if (metaAttibs.Length != 0)
                    {
                        foreach (ModelMetadataTypeAttribute attr in metaAttibs)
                        {


                            PropertyInfo pi = attr.MetadataType.GetProperty(prop.Name);
                            if (pi != null)
                            {
                                bool isPropDefined = Attribute.IsDefined(pi, typeof(DisplayAttribute));
                                if (isPropDefined)
                                {
                                    DisplayAttribute displayAttr = (DisplayAttribute)Attribute.GetCustomAttribute(pi, typeof(DisplayAttribute));
                                    headers[colCount] = displayAttr.Name;
                                    break;
                                }
                            }
                            else
                            {
                                headers[colCount] = prop.Name;
                                break;
                            }
                        }
                    }
                }
                //else
                //{
                //    headers[colCount] = prop.Name;
                //}
                colCount++;
                isDisplayNameAttributeDefined = false;
            }

            return headers;
        }
        public static string GetDisplayNameAttributes<T>(this Type t, string propertyName)
        {


            PropertyInfo Prop = typeof(T).GetProperty(propertyName);
            bool isDisplayNameAttributeDefined = false;
            string displayName = string.Empty;


            isDisplayNameAttributeDefined = Attribute.IsDefined(Prop, typeof(DisplayAttribute));
            if (isDisplayNameAttributeDefined)
            {
                DisplayAttribute dna = (DisplayAttribute)Attribute.GetCustomAttribute(Prop, typeof(DisplayAttribute));
                if (dna != null)
                {
                    displayName = dna.Name;
                }

            }
            else if (!isDisplayNameAttributeDefined)
            {
                ModelMetadataTypeAttribute[] metaAttibs = (ModelMetadataTypeAttribute[])typeof(T).GetCustomAttributes(typeof(ModelMetadataTypeAttribute), true);

                if (metaAttibs.Length != 0)
                {
                    foreach (ModelMetadataTypeAttribute attr in metaAttibs)
                    {


                        PropertyInfo pi = attr.MetadataType.GetProperty(Prop.Name);
                        if (pi != null)
                        {
                            bool isPropDefined = Attribute.IsDefined(pi, typeof(DisplayAttribute));
                            if (isPropDefined)
                            {
                                DisplayAttribute displayAttr = (DisplayAttribute)Attribute.GetCustomAttribute(pi, typeof(DisplayAttribute));
                                displayName = displayAttr.Name;
                                break;
                            }
                        }
                        else
                        {
                            displayName = Prop.Name;
                            break;
                        }
                    }
                }
            }

            return displayName;


        }
        public static Dictionary<string, Type> GetDisplayNameAndType<T>(this Type t, bool fromBuddyClass)
        {
            Dictionary<string, Type> propWithTypes = new Dictionary<string, Type>();
            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            bool isDisplayNameAttributeDefined = false;
            foreach (var prop in Props)
            {
                isDisplayNameAttributeDefined = Attribute.IsDefined(prop, typeof(DisplayAttribute));
                if (isDisplayNameAttributeDefined)
                {
                    DisplayAttribute dna = (DisplayAttribute)Attribute.GetCustomAttribute(prop, typeof(DisplayAttribute));
                    if (dna != null)
                    {
                        //headers[colCount] = dna.Name;
                        propWithTypes.Add(dna.Name, prop.PropertyType);
                    }

                }
                else if (!isDisplayNameAttributeDefined)
                {
                    ModelMetadataTypeAttribute[] metaAttibs = (ModelMetadataTypeAttribute[])typeof(T).GetCustomAttributes(typeof(ModelMetadataTypeAttribute), true);

                    if (metaAttibs.Length != 0)
                    {
                        foreach (ModelMetadataTypeAttribute attr in metaAttibs)
                        {


                            PropertyInfo pi = attr.MetadataType.GetProperty(prop.Name);
                            if (pi != null)
                            {
                                bool isPropDefined = Attribute.IsDefined(pi, typeof(DisplayAttribute));
                                if (isPropDefined)
                                {
                                    DisplayAttribute displayAttr = (DisplayAttribute)Attribute.GetCustomAttribute(pi, typeof(DisplayAttribute));
                                    propWithTypes.Add(displayAttr.Name, prop.PropertyType);
                                    break;
                                }
                            }
                            else
                            {
                                propWithTypes.Add(prop.Name, prop.PropertyType);
                                break;
                            }
                        }
                    }
                }

                isDisplayNameAttributeDefined = false;
            }

            return propWithTypes;
        }
    }
}
