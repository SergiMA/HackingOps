using HackingOps.Common.CommandSystem;
using HackingOps.Common.CommandSystem.Commands;
using HackingOps.Common.Services;
using System.Collections;
using UnityEngine;

namespace HackingOps.Screens.UI
{
    public class LaptopScreen : MonoBehaviour
    {
        public void OnStartButtonPressed()
        {
            ServiceLocator.Instance.GetService<CommandQueue>().AddCommand(new StopUsingLaptopCommand());
        }
    }
}