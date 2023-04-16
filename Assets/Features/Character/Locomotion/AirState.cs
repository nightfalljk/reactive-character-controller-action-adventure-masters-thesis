using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class AirState : State
    {
        private readonly LocomotionStateMachine _stateMachine;
        private Vector3 _momentum;
        private CompositeDisposable _disposables;
        private readonly int _fallAnimHash = Animator.StringToHash("Falling Idle");

        public AirState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.LockOtherStateMachines();
            
            _stateMachine.Animator.CrossFadeInFixedTime(_fallAnimHash, 0.1f);
            
            _stateMachine.SensorSystem.Grounded
                .Subscribe(grounded =>
                {
                    if(!grounded) return;
                    _stateMachine.SwitchState((int)LocomotionStates.Landing);
                }).AddTo(_disposables);

        }

        public override void Tick(float deltaTime)
        {
            _momentum = _stateMachine.SensorSystem.CharacterForward.Value;
            _momentum.y = 0;
            var speed = _stateMachine.PreviousState == LocomotionStates.Idle
                ? 0
                : _stateMachine.LocomotionConfig.airSpeed;
            _stateMachine.MovementResolver.Move(_momentum, speed, deltaTime);
        }

        public override void Exit()
        {
            _stateMachine.UnlockOtherStateMachines();
            _disposables.Dispose();
        }
    }
}