using UnityEngine;

namespace HackingOps.Common.Settings
{
    public interface ISaveable
    {
        void Save();

        /// <summary>
        /// Recover saved values, if they exist
        /// </summary>
        void Recover();
    }
}