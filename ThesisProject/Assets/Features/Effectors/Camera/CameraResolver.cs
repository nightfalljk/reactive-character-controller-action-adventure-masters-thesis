using System;
using Cinemachine;
using Features.Character.Camera;
using Features.Player;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace Features.Effectors.Camera
{
    public class CameraResolver : MonoBehaviour
    {
        [SerializeField] private Transform cameraFollow;
        [SerializeField] private CameraConfig cameraConfig;
        [SerializeField] private CinemachineImpulseSource impulseSource;
    
    
        private IObservable<Transform> _viewTransform;
        private Quaternion _initCameraFollowRot;
    

        private CompositeDisposable _disposables = new CompositeDisposable();
        
        private void Awake()
        {
            _initCameraFollowRot = cameraFollow.rotation;
            _viewTransform = this.UpdateAsObservable().Select(_ => cameraFollow);
        }

        public void MoveCamera(Vector2 viewInput, float rotationSpeed)
        {
            viewInput = viewInput.normalized;
            var horizontalView = cameraFollow.right + new Vector3(viewInput.x,0, viewInput.y) * 10 * Time.deltaTime;
            cameraFollow.rotation *= Quaternion.AngleAxis(viewInput.x * rotationSpeed * Time.deltaTime, Vector3.up);
            cameraFollow.rotation *= Quaternion.AngleAxis(-viewInput.y * rotationSpeed * Time.deltaTime, Vector3.right);

            var angles = cameraFollow.localEulerAngles;
            angles.z = 0;
            var angle = angles.x;

            if (angle > cameraConfig.downAngle && angle < 180)
            {
                angles.x = cameraConfig.downAngle;
            }
            else if (angle < cameraConfig.upAngle && angle > 180)
            {
                angles.x = cameraConfig.upAngle;
            }

            cameraFollow.localEulerAngles = angles;
        }

        public void DefaultMoveCamera(Vector2 viewInput)
        {
            MoveCamera(viewInput, cameraConfig.rotateSpeed);
        }

        public void AimCamera(Vector2 viewInput)
        {
            MoveCamera(viewInput, cameraConfig.aimRotateSpeed);
        }

        public void ResetCameraFollow()
        {
            cameraFollow.rotation = _initCameraFollowRot;
        }

        public void ShakeCamera(float force = 1)
        {
            impulseSource.GenerateImpulse(force);
        }

        private void OnDestroy()
        {
            _disposables.Clear();
        }

        public IObservable<Transform> ViewTransform => _viewTransform;
    
    }
}
