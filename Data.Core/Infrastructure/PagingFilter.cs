using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.Infrastructure
{
    public class PagingFilter
    {
        public int currentPage { get; set; }
        public int pageSize { get; set; }
        public List<GenericFilter> filter { get; set; }
    }
}
