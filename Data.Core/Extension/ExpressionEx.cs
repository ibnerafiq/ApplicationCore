﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Data.Core.Extension
{
    public static class ExpressionEx
    {
        public static Expression<Func<TItem, object>> GroupByExpression<TItem>(string[] propertyNames)
        {
            var properties = propertyNames.Select(name => typeof(TItem).GetProperty(name)).ToArray();
            var propertyTypes = properties.Select(p => p.PropertyType).ToArray();
            var tupleTypeDefinition = typeof(Tuple).Assembly.GetType("System.Tuple`" + properties.Length);
            var tupleType = tupleTypeDefinition.MakeGenericType(propertyTypes);
            var constructor = tupleType.GetConstructor(propertyTypes);
            var param = Expression.Parameter(typeof(TItem), "item");
            var body = Expression.New(constructor, properties.Select(p => Expression.Property(param, p)));
            var expr = Expression.Lambda<Func<TItem, object>>(body, param);
            return expr;
        }

        public static IQueryable<TEntity> OrderBy<TEntity>(this IQueryable<TEntity> source, string orderByValues) where TEntity : class
        {
            IQueryable<TEntity> returnValue = null;

            string orderPair = orderByValues.Trim().Split(',')[0];
            string command = orderPair.ToUpper().Contains("DESC") ? "OrderByDescending" : "OrderBy";

            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "p");

            string propertyName = (orderPair.Split(' ')[0]).Trim();

            System.Reflection.PropertyInfo property;
            MemberExpression propertyAccess;

            if (propertyName.Contains('.'))
            {
                // support to be sorted on child fields. 
                String[] childProperties = propertyName.Split('.');
                property = typeof(TEntity).GetProperty(childProperties[0]);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);

                for (int i = 1; i < childProperties.Length; i++)
                {
                    Type t = property.PropertyType;
                    if (!t.IsGenericType)
                    {
                        property = t.GetProperty(childProperties[i]);
                    }
                    else
                    {
                        property = t.GetGenericArguments().First().GetProperty(childProperties[i]);
                    }

                    propertyAccess = Expression.MakeMemberAccess(propertyAccess, property);
                }
            }
            else
            {
                property = type.GetProperty(propertyName);
                propertyAccess = Expression.MakeMemberAccess(parameter, property);
            }

            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(typeof(Queryable), command, new Type[] { type, property.PropertyType },

            source.Expression, Expression.Quote(orderByExpression));

            returnValue = source.Provider.CreateQuery<TEntity>(resultExpression);

            if (orderByValues.Trim().Split(',').Count() > 1)
            {
                // remove first item
                string newSearchForWords = orderByValues.ToString().Remove(0, orderByValues.ToString().IndexOf(',') + 1);
                return source.OrderBy(newSearchForWords);
            }

            return returnValue;
        }
    }
}
