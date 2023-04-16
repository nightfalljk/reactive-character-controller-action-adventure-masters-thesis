using Features.Perception.Sensors;
using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Locomotion
{
    public class IdleState : State
    {
        private readonly LocomotionStateMachine _stateMachine;

        private CompositeDisposable _disposables;

        private readonly int _idleAnimHash = Animator.StringToHash("Idle");
        private readonly int _moveSpeedAnimHash = Animator.StringToHash("MoveSpeed");

        public IdleState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.Animator.CrossFadeInFixedTime(_idleAnimHash, 0.1f);
            _stateMachine.Animator.SetFloat(_moveSpeedAnimHash, 0);
            
            _stateMachine.SensorSystem.HorizontalMoveDir
                .Where(moveDir => moveDir != Vector2.zero)
                .Subscribe(_ =>
                {
                    _stateMachine.SwitchState((int)LocomotionStates.BasicMove);
                }).AddTo(_disposables);
            
            _stateMachine.SensorSystem.Jump.Subscribe(_ =>
            {
                switch (_stateMachine.SensorSystem.CurrentObstacleInteraction.Value)
                {
                    case ObstacleInteraction.Vault:
                        _stateMachine.SwitchState((int) LocomotionStates.Vault);
                        break;
                    case ObstacleInteraction.StepUp:
                        _stateMachine.SwitchState((int) LocomotionStates.StepUp);
                        break;
                    case ObstacleInteraction.ClimbUp:
                        _stateMachine.SwitchState((int) LocomotionStates.WallRun);
                        break;

                    default:
                        _stateMachine.SwitchState((int)LocomotionStates.Jump);
                        break;
                }
            }).AddTo(_disposables);
            
            _stateMachine.SensorSystem.Dodge.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int)LocomotionStates.Dodge);
            }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {
            
        }

        public override void Exit()
        {
            _stateMachine.PreviousState = LocomotionStates.Idle;
            _disposables.Dispose();
        }
    }
}