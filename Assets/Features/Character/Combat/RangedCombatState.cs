using Features.StateMachines;
using UniRx;

namespace Features.Character.Combat
{
    public class RangedCombatState : State
    {
        
        private readonly CombatStateMachine _stateMachine;
        private CompositeDisposable _disposables;

        private const int AimLayer = 2;
        public RangedCombatState(CombatStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _disposables = new CompositeDisposable();
            _stateMachine.SensorSystem.Aim.Subscribe(_ =>
            {
                _stateMachine.SwitchState((int) CombatStates.Melee);
            }).AddTo(_disposables);
            _stateMachine.Animator.SetLayerWeight(AimLayer, 1);

            _stateMachine.SensorSystem.Attack.Subscribe(_ =>
            {
                _stateMachine.CurrentWeapon.Attack(_stateMachine.Animator);
            }).AddTo(_disposables);
        }

        public override void Tick(float deltaTime)
        {

        }

        public override void Exit()
        {
            _stateMachine.Animator.SetLayerWeight(AimLayer, 0);
            _disposables.Dispose();
        }
    }
}