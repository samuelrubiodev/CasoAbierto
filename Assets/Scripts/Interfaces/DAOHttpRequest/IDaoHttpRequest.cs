using System.Threading.Tasks;

public interface IDaoHttpRequest<T, O>
{
    Task<T> PostAsync(string url, O data);
}

public interface IDaoHttpRequestExtended<T, O>
{
    Task<T> GetAsync(string url);
}