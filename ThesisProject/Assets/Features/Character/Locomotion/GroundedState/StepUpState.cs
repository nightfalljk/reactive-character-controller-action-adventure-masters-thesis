using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class StepUpState : State
    {
        private LocomotionStateMachine _stateMachine;
        private readonly int _stepUpAnim = Animator.StringToHash("Step Up");

        private CompositeDisposable _disposables;
        public StepUpState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.Animator.CrossFadeInFixedTime(_stepUpAnim, 0.1f);
            _stateMachine.LockOtherStateMachines();

            SetAnimationIK();

            _stateMachine.SensorSystem.AnimationFinished.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)_stateMachine.PreviousState);
            }).AddTo(_disposables);
        }

        private void SetAnimationIK()
        {
            _stateMachine.AnimationResolver.EnableIKPosition(AvatarIKGoal.LeftHand, 0.5f,
                _stateMachine.SensorSystem.ObstacleInteractionEndPos.Value);
            var rightFootPos = _stateMachine.SensorSystem.ObstacleInteractionEndPos.Value +
                               _stateMachine.SensorSystem.CharacterRight.Value * 0.25f;
            _stateMachine.AnimationResolver.EnableIKPosition(AvatarIKGoal.RightFoot, 0.5f, rightFootPos);
        }

        public override void Tick(float deltaTime)
        {
            AnimatorStateInfo animatorStateInfo = _stateMachine.Animator.GetCurrentAnimatorStateInfo(0);
            if (animatorStateInfo.normalizedTime > 0.35f
                && _stateMachine.AnimationResolver.UseInverseKinematics)
            {
                _stateMachine.AnimationResolver.DisableIK();
            }
            else if (animatorStateInfo.normalizedTime is > 0.15f and < 0.35f 
                     && !_stateMachine.AnimationResolver.UseInverseKinematics)
            {
                _stateMachine.AnimationResolver.EnableIK();
            }
        }
        
        //TODO: MATCH TARGET INSTEAD OF IK?

        public override void Exit()
        {
            _stateMachine.UnlockOtherStateMachines();
            _disposables.Dispose();
        }
    }
}