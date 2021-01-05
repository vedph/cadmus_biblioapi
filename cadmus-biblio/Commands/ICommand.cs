using System.Threading.Tasks;

namespace Cadmus.Biblio.Commands
{
    public interface ICommand
    {
        Task Run();
    }
}
