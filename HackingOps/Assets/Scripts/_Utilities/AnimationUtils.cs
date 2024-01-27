using DG.Tweening;
using UnityEngine;

namespace HackingOps.Utilities
{
    public static class AnimationUtils
    {
        public static Tweener UpdateAnimationWeight(Animator animator, int layerIndex, float startingWeight, float targetWeight, float transitionDuration)
        {
            return DOVirtual.Float(startingWeight, targetWeight, transitionDuration, weight =>
            {
                animator.SetLayerWeight(layerIndex, weight);
            });
        }
    }
}