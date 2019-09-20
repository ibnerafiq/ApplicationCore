using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.Infrastructure
{
    public class GenericFilter
    {
        public string PropertyName { get; set; }
        public object Value { get; set; }
        public Op Operation { get; set; }
    }
    public class GenericListFilter
    {
        public GenericListFilter()
        {
            Value = new List<object>();

        }
        public string PropertyName { get; set; }
        public List<object> Value { get; set; }
        public Op Operation { get; set; }
    }

    public enum Op
    {
        Equals = 1,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Contains,
        StartsWith,
        EndsWith,
        NotEqual
    }
}
