using Features.Character.Locomotion;
using Features.StateMachines;
using UniRx;
using UnityEngine;

namespace Features.Character.Combat
{
    public class MeleeCombatState : State
    {
        private CombatStateMachine _stateMachine;

        private CompositeDisposable _disposables;
        private const int CombatAnimationLayer = 1;
        private readonly int _meleeAttackAnimHash = Animator.StringToHash("Standing Melee Kick");

        private bool _attacking;

        public MeleeCombatState(CombatStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _attacking = false;
            _disposables = new CompositeDisposable();
            _stateMachine.SensorSystem.Aim.Subscribe(_ =>
            {
                if(_attacking) return;
                _stateMachine.SwitchState((int) CombatStates.Ranged);
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.Attack.Subscribe(_ =>
            {
                if(_attacking) return;
                _stateMachine.Animator.SetLayerWeight(CombatAnimationLayer, 1);
                _attacking = true;
                _stateMachine.Animator.CrossFadeInFixedTime(_meleeAttackAnimHash, 0.1f, CombatAnimationLayer);
                _stateMachine.LockOtherStateMachines();
            }).AddTo(_disposables);

            _stateMachine.SensorSystem.AnimationFinished.Subscribe(_ =>
            {
                if(!_attacking) return;
                _stateMachine.UnlockOtherStateMachines();
                _stateMachine.LocomotionStateMachine.SwitchState((int) LocomotionStates.Idle);
                _stateMachine.Animator.SetLayerWeight(CombatAnimationLayer, 0);
                _attacking = false;
            }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {
        }

        public override void Exit()
        {
            _disposables.Dispose();
        }
    }
}