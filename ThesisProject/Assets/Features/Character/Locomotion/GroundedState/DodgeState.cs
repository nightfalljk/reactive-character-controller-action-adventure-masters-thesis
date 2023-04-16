using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class DodgeState : State
    {
        private readonly LocomotionStateMachine _stateMachine;
        private readonly int _standDodgeAnimHash = Animator.StringToHash("Stand To Roll");
        private readonly int _runDodgeAnimHash = Animator.StringToHash("Sprinting Forward Roll");

        private CompositeDisposable _disposables;
        public DodgeState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _stateMachine.LockOtherStateMachines();
            if (_stateMachine.PreviousState == LocomotionStates.Idle)
            {
                _stateMachine.Animator.CrossFadeInFixedTime(_standDodgeAnimHash, 0.1f);
            }
            else
            {
                _stateMachine.Animator.CrossFadeInFixedTime(_runDodgeAnimHash, 0.1f);
            }
            
            _disposables = new CompositeDisposable();

            _stateMachine.SensorSystem.Grounded.Subscribe(grounded =>
            {
                if (!grounded)
                {
                    _stateMachine.SwitchState((int)LocomotionStates.Air);
                }
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.AnimationFinished.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)_stateMachine.PreviousState);
            }).AddTo(_disposables);

        }
        public override void Tick(float deltaTime)
        {
            
        }
        public override void Exit()
        {
            _stateMachine.UnlockOtherStateMachines();
            _disposables.Dispose();
        }
        
    }
}