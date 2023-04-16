using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class TakeOffState : State
    {
        private readonly LocomotionStateMachine _stateMachine;
        private CompositeDisposable _disposables;

        private LocomotionStates _nextState;

        private readonly int _jumpAnimHash = Animator.StringToHash("Jumping Up");
        private readonly int _runningJumpAnimHash = Animator.StringToHash("Running Jump");

        public TakeOffState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.LockOtherStateMachines();
            
            if(_stateMachine.PreviousState == LocomotionStates.Idle)
                _stateMachine.Animator.CrossFadeInFixedTime(_jumpAnimHash, 0.1f);
            else
                _stateMachine.Animator.CrossFadeInFixedTime(_runningJumpAnimHash, 0.1f);

            _stateMachine.SensorSystem.AnimationFinished
                .Subscribe(_ =>
                {
                    _stateMachine.SwitchState((int)LocomotionStates.Air);
                }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {
            //_stateMachine.MovementResolver.Move(_momentum, 1, deltaTime);
        }

        public override void Exit()
        {
            _stateMachine.UnlockOtherStateMachines();
            _disposables.Dispose();
        }
    }
}