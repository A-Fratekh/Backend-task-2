
using System.Linq.Expressions;
namespace OrderProcessing.Domain.SeedWork;

public interface IReadRepository<T> where T : class
{
    T GetById(int id, string? includes);
    T GetById(int id);
    T GetById(object[] id);
    IEnumerable<T> GetAll();
    IQueryable<T> Query();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}
