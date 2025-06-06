﻿
using System.Linq.Expressions;
namespace OrderProcessing.Domain.SeedWork;

public interface IReadRepository<T> where T : class
{
    T GetById(int id);
    IEnumerable<T> GetAll();
}
