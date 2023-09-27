using System.Threading.Tasks;

namespace HackingOps.Common.CommandSystem
{
    public interface ICommand
    {
        Task Execute();
    }
}