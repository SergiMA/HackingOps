using UnityEngine;

namespace HackingOps.Weapons.Common
{
    [CreateAssetMenu(fileName = "New Ammunition type", menuName = "Scriptable Object/Ammunition Type")]
    public class AmmunitionTypeBase : ScriptableObject
    {
        public string AmmoName;
        public float Weight;
    }
}