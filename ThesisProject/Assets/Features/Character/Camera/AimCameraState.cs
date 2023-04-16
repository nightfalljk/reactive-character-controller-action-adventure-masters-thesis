using Features.Character.Locomotion;
using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Camera
{
    public class AimCameraState : State
    {
        private readonly CameraStateMachine _stateMachine;
        private CompositeDisposable _disposables;
        public AimCameraState(CameraStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.LocomotionStateMachine.SwitchState((int) LocomotionStates.Strafe);
            _stateMachine.SensorSystem.Aim.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)CameraStates.Default);
            }).AddTo(_disposables);
            
            _stateMachine.SensorSystem.LookInput.Subscribe(viewDir =>
            {
                if (viewDir == Vector2.zero) return;
                _stateMachine.CameraResolver.AimCamera(viewDir);
            }).AddTo(_disposables);
            
            _stateMachine.AimCam.gameObject.SetActive(true);
            _stateMachine.CrossHair.SetActive(true);
        }

        public override void Tick(float deltaTime)
        {

        }

        public override void Exit()
        {
            _disposables.Dispose();
            _stateMachine.LocomotionStateMachine.SwitchState((int) LocomotionStates.Idle);
            _stateMachine.AimCam.gameObject.SetActive(false);
            _stateMachine.CrossHair.SetActive(false);

        }
    }
}