using Redis.Models;
using Utilities;

namespace test;

public interface IConverter 
{
    public Result<T1, Exception> Convert<T2, T1>(T2 t2) where T1 : class;
}