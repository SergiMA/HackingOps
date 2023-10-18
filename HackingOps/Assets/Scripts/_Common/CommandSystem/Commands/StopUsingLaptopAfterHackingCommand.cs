using HackingOps.Common.Services;
using HackingOps.CutsceneSystem;
using System.Threading.Tasks;

namespace HackingOps.Common.CommandSystem.Commands
{
    public class StopUsingLaptopAfterHackingCommand : ICommand
    {
        public Task Execute()
        {
            ServiceLocator.Instance.GetService<LaptopCutscene>().PlayGetUpAfterHacking();

            return Task.CompletedTask;
        }
    }
}
