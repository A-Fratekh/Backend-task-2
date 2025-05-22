

namespace OrderProcessing.Domain.SeedWork;

public interface IReadRepository<T> where T : class
{
    T GetById(int id);
    int GetNextId();
    IEnumerable<T> GetAll();
}
