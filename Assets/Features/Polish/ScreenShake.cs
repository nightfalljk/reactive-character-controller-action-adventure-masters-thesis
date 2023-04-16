using Features.Effectors.Camera;
using Features.Perception.Sensors;
using UniRx;
using UnityEngine;

namespace Features.Polish
{
    public class ScreenShake : MonoBehaviour
    {
        [SerializeField] private SensorSystem sensorSystem;
        [SerializeField] private CameraResolver cameraResolver;
        
        void Start()
        {
            sensorSystem.Health
                .Zip(sensorSystem.Health.Skip(1), (prev, curr) => new { Previous = prev, Current = curr })
                .Where(arg => arg.Previous > arg.Current)
                .Subscribe(_ =>
                {
                    cameraResolver.ShakeCamera();
                }).AddTo(this);

            sensorSystem.FallHeight
                .Where(fallHeight => fallHeight > 0)
                .Subscribe(fallHeight =>
                {
                    float forceFactor = fallHeight / 6f;
                    forceFactor = Mathf.Min(forceFactor, 1f);
                    float force = forceFactor * 3;
                    cameraResolver.ShakeCamera(force);
                }).AddTo(this);
        }
    }
}
