using System;
using Cinemachine;
using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Camera
{
    public class DefaultCameraState : State
    {
        private CameraStateMachine _stateMachine;

        private CompositeDisposable _disposables;
        private Cinemachine3rdPersonFollow _cinemachineFollow;
        private float _targetCamDistance;
        private float _camDistance;
        public DefaultCameraState(CameraStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _cinemachineFollow = _stateMachine.DefaultCam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _camDistance = _stateMachine.CameraConfig.defaultCamDistance;
            _stateMachine.SensorSystem.Aim.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)CameraStates.Aim);
            }).AddTo(_disposables);
            _stateMachine.DefaultCam.gameObject.SetActive(true);
            
            _stateMachine.SensorSystem.LookInput.Subscribe(viewDir =>
            {
                if (viewDir == Vector2.zero) return;
                _stateMachine.CameraResolver.DefaultMoveCamera(viewDir);
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.MoveSpeed.Subscribe(speed =>
            {

                var distanceFraction = speed / _stateMachine.LocomotionConfig.sprintSpeed;
                _targetCamDistance = _stateMachine.CameraConfig.minDefaultCamDistance +
                                     distanceFraction * _stateMachine.CameraConfig.maxDefaultCamDistance;
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.LookInput
                .Where(viewDir => viewDir != Vector2.zero)
                .Throttle(TimeSpan.FromSeconds(_stateMachine.CameraConfig.defaultPosReturnDuration))
                .Subscribe(_ =>
                {
                    if (_stateMachine.SensorSystem.MoveSpeed.Value > 0)
                        ReturnToDefaultCamPos();
                }).AddTo(_disposables);
            
        }

        public void ReturnToDefaultCamPos()
        {
            Debug.Log("Returning to default cam pos");
            //_stateMachine.CameraResolver.ResetCameraFollow();
        }

        public override void Tick(float deltaTime)
        {
            _cinemachineFollow.CameraDistance = _camDistance;
            _camDistance = Mathf.MoveTowards(_camDistance, _targetCamDistance, deltaTime * _stateMachine.CameraConfig.defaultCamZoomSpeed);
        }

        public override void Exit()
        {
            _stateMachine.DefaultCam.gameObject.SetActive(false);
            _disposables.Dispose();
        }
    }
}