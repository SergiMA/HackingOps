using System.Collections;
using UnityEngine;

namespace HackingOps.Characters.NPC.Senses.SightSense
{
    public interface IVisible
    {
        Vector3[] GetCheckpoints();
        Transform GetTransform();
    }
}