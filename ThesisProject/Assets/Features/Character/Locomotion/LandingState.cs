using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class LandingState : State
    {
        private readonly LocomotionStateMachine _stateMachine;
        private CompositeDisposable _disposables;

        private readonly int _fallingToLanding = Animator.StringToHash("Falling To Landing");
        private readonly int _fallingToRoll = Animator.StringToHash("Falling To Roll");
        private readonly int _hardLandingHash = Animator.StringToHash("Hard Landing");

        public LandingState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            
            _disposables = new CompositeDisposable();
            _stateMachine.LockOtherStateMachines();
            _stateMachine.SensorSystem.FallHeight
                .Subscribe(dist =>
                {
                    if (dist >= _stateMachine.LocomotionConfig.deadlyFallHeight)
                    {
                        _stateMachine.Health.TakeDamage(10000);
                    }
                    else if (dist >= _stateMachine.LocomotionConfig.rollLandingFallHeight)
                    {
                        _stateMachine.Animator.CrossFadeInFixedTime(_hardLandingHash, 0.1f);
                        _stateMachine.Health.TakeDamage(20);
                    }
                    else if (dist >= _stateMachine.LocomotionConfig.heavyLandingFallHeight)
                    {
                        _stateMachine.Animator.CrossFadeInFixedTime(_fallingToRoll, 0.1f);
                    }
                    else
                    {
                        _stateMachine.Animator.CrossFadeInFixedTime(_fallingToLanding, 0.1f);
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