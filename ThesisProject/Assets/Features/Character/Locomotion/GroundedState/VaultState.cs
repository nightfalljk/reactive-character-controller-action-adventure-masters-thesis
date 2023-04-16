using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class VaultState : State
    {
        private LocomotionStateMachine _stateMachine;
        
        private readonly int _speedVaultAnim = Animator.StringToHash("Speed Vault");

        private CompositeDisposable _disposables;

        public VaultState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            //_stateMachine.AnimationResolver.EnableIKPosition(AvatarIKGoal.LeftHand, 0.5f, _stateMachine.SensorSystem.IKTargets[0]);
            _stateMachine.Animator.CrossFadeInFixedTime(_speedVaultAnim, 0.1f);
            _stateMachine.LockOtherStateMachines();
            _stateMachine.MovementResolver.enabled = false;

            _stateMachine.SensorSystem.AnimationFinished.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)_stateMachine.PreviousState);
            }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {
            AnimatorStateInfo animatorStateInfo = _stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.normalizedTime > 0.65f
                && _stateMachine.AnimationResolver.UseInverseKinematics)
            {
                _stateMachine.AnimationResolver.DisableIK();
            }
            else if (animatorStateInfo.normalizedTime is > 0.25f and < 0.65f 
                     && !_stateMachine.AnimationResolver.UseInverseKinematics)
            {
                _stateMachine.AnimationResolver.EnableIK();
            }
        }
        public override void Exit()
        {
            _disposables.Dispose();
            _stateMachine.UnlockOtherStateMachines();
            _stateMachine.MovementResolver.enabled = true;
        }

    }
}