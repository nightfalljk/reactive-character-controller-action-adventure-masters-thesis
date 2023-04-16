using Features.Perception.Sensors;
using UniRx;
using UnityEngine;

namespace Features.Polish.Footsteps
{
    public class Footsteps : MonoBehaviour
    {
        [SerializeField] private ParticleSystem leftFootParticles;
        [SerializeField] private ParticleSystem rightFootParticles;
        [SerializeField] private ParticleSystem landingParticles;
        [SerializeField] private SensorSystem sensorSystem;

        [SerializeField] private AudioClip defaultAudioClip;
        [SerializeField] private AudioClip stoneAudioClip;
        [SerializeField] private AudioClip defaultLandingClip;
        [SerializeField] private AudioClip stoneLandingClip;

        private AudioSource _audioSource;
        private AudioClip _currentAudioClip;
        private AudioClip _currentLandingClip;

        private void Awake()
        {
            _audioSource = GetComponent<AudioSource>();
            _currentAudioClip = defaultAudioClip;
        }

        private void Start()
        {
            sensorSystem.LeftStep.Subscribe(_ =>
            {
                leftFootParticles.Play();
                _audioSource.PlayOneShot(_currentAudioClip);
            }).AddTo(this);

            sensorSystem.RightStep.Subscribe(_ =>
            {
                rightFootParticles.Play();
                _audioSource.PlayOneShot(_currentAudioClip);
            }).AddTo(this);

            sensorSystem.Landing.Subscribe(_ =>
            {
                landingParticles.Play();
                _audioSource.PlayOneShot(_currentLandingClip);
            }).AddTo(this);

            sensorSystem.GroundType.Subscribe(groundType =>
            {
                switch (groundType)
                {
                    case GroundType.Default:
                        _currentAudioClip = defaultAudioClip;
                        _currentLandingClip = defaultLandingClip;
                        break;
                    case GroundType.Stone:
                        _currentAudioClip = stoneAudioClip;
                        _currentLandingClip = stoneLandingClip;
                        break;
                }
            }).AddTo(this);
        }

    }
}
