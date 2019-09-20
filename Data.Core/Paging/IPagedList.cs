using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Data.Core.Paging
{
    public interface IPagedList<T> where T : class
    {
        int TotalCount { get; set; }
        int PageSize { get; set; }
        int CurrentPage { get; set; }
        List<T> Data { get; set; }
    }

    public class PagedList<T> : IPagedList<T> where T : class
    {





        public PagedList(IQueryable<T> Source, int currentPage, int pageSize)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalCount = Source.Count();
            Data = Source.Skip(PageSize * (CurrentPage - 1)).Take(this.PageSize).ToList();
        }

        public PagedList(List<T> Source, int currentPage, int pageSize)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalCount = Source.Count();
            Data = Source.Skip(PageSize * (CurrentPage - 1)).Take(this.PageSize).ToList();
        }
        public PagedList(List<T> Source, int currentPage, int pageSize, int totalCount)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalCount = totalCount;
            Data = Source;
        }
        public PagedList(IEnumerable<T> Source, int currentPage, int pageSize)
        {
            PageSize = pageSize;
            CurrentPage = currentPage;
            TotalCount = Source.Count();
            Data = Source.Skip(PageSize * (CurrentPage - 1)).Take(this.PageSize).ToList();
        }


        public int TotalCount { get; set; }
        public List<T> Data { get; set; }
        public int PageSize { get; set; }
        public int CurrentPage
        {
            get;
            set;
        }

    }
}
