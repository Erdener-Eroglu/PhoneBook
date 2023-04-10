using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBookDataLayer.InterfacesOfRepo
{
    public interface IRepository<T,Id> where T : class, new()
    {
        int Add(T entity);
        int Update(T entity);
        int Delete(T entity);
        T GetById(Id id);
        IQueryable<T> GetAll();
        T GetByConditions();
    }
}
