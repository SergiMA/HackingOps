using System.Collections;
using UnityEngine;

namespace HackingOps.VFX.Holograms
{
    /// <summary>
    /// This script is only used to preview how the glitch effect in a hologram can be displayed over time.
    /// The script is free to improve the glitch effect
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class HologramGlitchAnimation : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2 _timeRange = new(1, 3);
        [SerializeField] private float _timeWait = 0.2f;

        private Material _material;
        private int _hash_useGlitch = Shader.PropertyToID("_UseGlitch");

        private void Start()
        {
            _material = GetComponent<Renderer>().material;

            StartCoroutine(StartGlitch());
        }

        private IEnumerator StartGlitch()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(_timeRange.x, _timeRange.y));

                _material.SetFloat(_hash_useGlitch, 1);

                yield return new WaitForSeconds(_timeWait);

                _material.SetFloat(_hash_useGlitch, 0);
            }
        }
    }
}
