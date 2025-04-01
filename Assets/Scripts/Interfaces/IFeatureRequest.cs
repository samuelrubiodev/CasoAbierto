using System.Threading.Tasks;

public interface IFeatureRequest<O> {
    Task<O> SendRequest(string prompt);
}