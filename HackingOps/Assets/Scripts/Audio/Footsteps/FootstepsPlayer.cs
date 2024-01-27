using UnityEngine;

namespace HackingOps.Audio.Footsteps
{
    [RequireComponent(typeof(AudioSource))]
    public class FootstepsPlayer : MonoBehaviour
    {
        [Header("Audio clips")]
        [SerializeField] private AudioClip[] _concreteFootstepClips;
        [SerializeField] private AudioClip[] _dirtFootstepClips;
        [SerializeField] private AudioClip[] _metalGrateFootstepClips;
        [SerializeField] private AudioClip[] _metalFootstepClips;

        [Header("Settings - Detection")]
        [SerializeField] private float _raycastDistance = 0.1f;

        [Header("Settings - Pitch")]
        [SerializeField] private float _basePitch = 1f;
        [Range(-1, 1)][SerializeField] private float _pitchVariation = 0.1f;
        [SerializeField] private bool _randomizePitch = true;

        [Header("Debug")]
        [SerializeField] private bool _debugDoStep;

        private AudioSource _audioSource;
        private TerrainDetector _terrainDetector;

        private void OnValidate()
        {
            if (_debugDoStep)
            {
                _debugDoStep = false;

                _audioSource = _audioSource != null ? _audioSource : GetComponent<AudioSource>();
                _terrainDetector ??= new TerrainDetector();

                Step();
            }
        }

        private void Awake()
        {
            _audioSource ??= GetComponent<AudioSource>();
            _terrainDetector ??= new TerrainDetector();
        }

        private SurfaceType GetGroundSurface()
        {
            float debugRayDuration = 0.5f;
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, _raycastDistance))
            {
                if (hit.transform.TryGetComponent(out Terrain terrain))
                {
                    Debug.DrawRay(transform.position, Vector3.down, Color.green, debugRayDuration);
                    return SurfaceType.Terrain;
                }
                else if (hit.transform.TryGetComponent(out SteppableSurface steppableSurface))
                {
                    Debug.DrawRay(transform.position, Vector3.down, Color.cyan, debugRayDuration);
                    return steppableSurface.SurfaceType;
                }
            }

            Debug.DrawRay(transform.position, Vector3.down, Color.red, debugRayDuration);
            return SurfaceType.Nothing;
        }

        private AudioClip[] GetClipsBasedOnSurfaceType(SurfaceType surfaceType)
        {
            return surfaceType switch
            {
                SurfaceType.Terrain => GetClipsFromTerrainTexture(),
                SurfaceType.Metal => _metalFootstepClips,
                SurfaceType.MetalGrate => _metalGrateFootstepClips,
                SurfaceType.Concrete => _concreteFootstepClips,
                SurfaceType.Stone => _concreteFootstepClips,
                SurfaceType.Nothing => null,
                _ => _concreteFootstepClips,
            };
        }

        private AudioClip[] GetClipsFromTerrainTexture()
        {
            int terrainTextureIndex = _terrainDetector.GetActiveTerrainTextureIndex(transform.position);

            return terrainTextureIndex switch
            {
                0 => _dirtFootstepClips,
                2 => _dirtFootstepClips,
                4 => _dirtFootstepClips,
                _ => _concreteFootstepClips,
            };
        }

        private void PlayRandomClip(AudioClip[] clips)
        {
            AudioClip clip = GetRandomClip(clips);
            if (_randomizePitch) ChangePitchRandomly();
            _audioSource.PlayOneShot(clip);
        }

        private AudioClip GetRandomClip(AudioClip[] clips) => clips[Random.Range(0, clips.Length)];

        private void ChangePitchRandomly()
        {
            _audioSource.pitch = 1f + Random.Range(-_pitchVariation, _pitchVariation);
        }

        public void Step()
        {
            SurfaceType surfaceType = GetGroundSurface();
            AudioClip[] clips = GetClipsBasedOnSurfaceType(surfaceType);

            if (clips != null) PlayRandomClip(clips);
        }

        public void ChangeVolume(float volume) => _audioSource.volume = volume;
    }
}