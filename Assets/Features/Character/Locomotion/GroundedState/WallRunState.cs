using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class WallRunState : State
    {
        private readonly LocomotionStateMachine _stateMachine;
        private readonly int _wallClimbRunAnim = Animator.StringToHash("Wall Run");

        private CompositeDisposable _disposables;
        public WallRunState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.LockOtherStateMachines();
            _stateMachine.Animator.CrossFadeInFixedTime(_wallClimbRunAnim, 0.0f);
            _stateMachine.MovementResolver.enabled = false;
            AnimationMatchEndPositionTarget();

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
            _disposables.Dispose();
            _stateMachine.MovementResolver.enabled = true;
            _stateMachine.UnlockOtherStateMachines();
        }
        
        private void AnimationMatchEndPositionTarget()
        {
            Vector3 matchPosition = _stateMachine.SensorSystem.ObstacleInteractionEndPos.Value;
            Quaternion matchRotation = Quaternion.LookRotation(_stateMachine.SensorSystem.CharacterForward.Value);
            MatchTargetWeightMask matchTargetWeightMask = new MatchTargetWeightMask(Vector3.one, 1f);
            _stateMachine.Animator.MatchTarget(matchPosition, matchRotation, AvatarTarget.LeftFoot,
                matchTargetWeightMask, 0.6f, 0.9f);
            _stateMachine.Animator.MatchTarget(matchPosition, matchRotation, AvatarTarget.RightFoot,
                matchTargetWeightMask, 0.6f, 0.9f);
            _stateMachine.Animator.MatchTarget(matchPosition, matchRotation, AvatarTarget.LeftHand,
                matchTargetWeightMask, 0.25f, 0.4f);
            _stateMachine.Animator.MatchTarget(matchPosition, matchRotation, AvatarTarget.RightHand,
                matchTargetWeightMask, 0.25f, 0.4f);
        }
    }
}