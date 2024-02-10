using HackingOps.Common.Events;
using HackingOps.Common.Services;
using UnityEngine;

namespace HackingOps.Audio.Music
{
    public class MusicController : MonoBehaviour, IEventObserver
    {
        [SerializeField] private AudioClip _backgroundMusic;
        [SerializeField] private AudioClip _combatMusic;

        private AudioSwapper _audioSwapper;
        private IEventQueue _eventQueue;

        private void Awake()
        {
            _audioSwapper = ServiceLocator.Instance.GetService<AudioSwapper>();
            _eventQueue = ServiceLocator.Instance.GetService<IEventQueue>();
        }

        private void Start()
        {
            _eventQueue.Subscribe(EventIds.OnEnterCombatMode, this);
            _eventQueue.Subscribe(EventIds.OnLeaveCombatMode, this);
        }

        private void PlayBackgroundMusic()
        {
            _audioSwapper.Swap(_backgroundMusic);
        }

        private void PlayCombatMusic()
        {
            _audioSwapper.Swap(_combatMusic);
        }

        public void Process(EventData eventData)
        {
            switch (eventData.EventId)
            {
                case EventIds.OnEnterCombatMode: PlayCombatMusic(); break;
                case EventIds.OnLeaveCombatMode: PlayBackgroundMusic(); break;
            }
        }
    }
}