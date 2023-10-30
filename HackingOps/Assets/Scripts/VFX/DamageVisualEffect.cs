using HackingOps.CombatSystem.HitHurtBox;
using System;
using UnityEngine;

namespace HackingOps.VFX
{
    public class DamageVisualEffect : MonoBehaviour
    {
        [SerializeField] private HurtBoxWithLife _healthSystem;
        [SerializeField] private Material _damageShaderMaterial;

        private void Update()
        {
            UpdateEffect();
        }

        private void UpdateEffect()
        {
            float healthPercentage = _healthSystem.GetCurrentHealthPercentage();
            float processedPercentage = Remap(healthPercentage, 1f, 0f, 0f, 1f);

            _damageShaderMaterial.SetFloat("_DamageIntensity", processedPercentage);
        }

        private float Remap(float value, float fromMin, float fromMax, float toMin, float toMax)
        {
            return Mathf.Lerp(toMin, toMax, Mathf.InverseLerp(fromMin, fromMax, value));
        }
    }
}