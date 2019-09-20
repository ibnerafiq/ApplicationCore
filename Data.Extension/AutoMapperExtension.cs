using AutoMapper;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Data.Extension
{
    public static class AutoMapperExtension
    {
        public static IMappingExpression<TSource, TDestination> IgnoreAllNonExisting<TSource, TDestination>
               (this IMappingExpression<TSource, TDestination> expression)
        {
            var flags = BindingFlags.Public | BindingFlags.Instance;
            var sourceType = typeof(TSource);
            var destinationProperties = typeof(TDestination).GetProperties(flags);

            foreach (var property in destinationProperties)
            {
                if (sourceType.GetProperty(property.Name, flags) == null)
                {
                    expression.ForMember(property.Name, opt => opt.Ignore());
                }
            }
            return expression;
        }

        //public static Ret MappedEntity<Ret, Param>(this Param p)
        //{
        //    return Mapper.Map<Ret>(p);
        //}

        //public static List<Ret> VMtoModel<Ret, Param>(List<Param> p)
        //{
        //    List<Ret> result = new List<Ret>();
        //    p.ForEach(aa =>
        //    {

        //        result.Add(Mapper.Map<Ret>(p));
        //    });

        //    return result;

        //}



    }
}
