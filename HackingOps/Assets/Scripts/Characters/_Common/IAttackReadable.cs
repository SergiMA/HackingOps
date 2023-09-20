using System;

namespace HackingOps.Characters.Common
{
    public interface IAttackReadable
    {
        event Action OnMustAttack;
        bool MustAttack();
    }
}