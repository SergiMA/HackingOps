using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using System.Threading.Tasks;

namespace HackingOps.Common.CommandSystem.Commands
{
    public class StopUsingLaptopCommand : ICommand
    {
        public Task Execute()
        {
            ServiceLocator.Instance.GetService<LaptopCutscene>().PlayGetUpCutscene();

            return Task.CompletedTask;
        }
    }
}
