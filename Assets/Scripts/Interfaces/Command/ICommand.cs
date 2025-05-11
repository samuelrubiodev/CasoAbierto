using System.Threading.Tasks;

public interface ICommand {
    public Task Execute();
}