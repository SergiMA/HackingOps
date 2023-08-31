using UnityEngine;

namespace HackingOps.Weapons.Common
{
    [CreateAssetMenu(fileName = "New Ammunition type", menuName = "Hacking Ops/Ammunition Type")]
    public class AmmunitionTypeBase : ScriptableObject
    {
        public string AmmoName;
        public float Weight;
    }
}