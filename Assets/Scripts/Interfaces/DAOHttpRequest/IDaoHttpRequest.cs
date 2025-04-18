using System.Threading.Tasks;

public interface IDaoHttpRequest<T, O>
{
    Task<T> PostAsync(string url, O data);
}