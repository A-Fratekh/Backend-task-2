namespace OrderProcessing.Domain.SeedWork;
public interface IRepository <T> where T : IAggregateRoot
{
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    T GetById(int id);
    int GetNextId();
}
