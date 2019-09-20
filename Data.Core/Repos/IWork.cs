using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Core.Repos
{
    public interface IWork : IDisposable
    {
        IRepository<T> RepositoryFor<T>() where T : class;

        void SaveChanges();
    }
}
