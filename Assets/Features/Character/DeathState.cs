using Features.Character.Locomotion;
using Features.StateMachines;

namespace Features.Character
{
    public class DeathState : State
    {
        private LocomotionStateMachine _stateMachine;

        public DeathState(LocomotionStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }
        public override void Enter()
        {
            _stateMachine.LockOtherStateMachines();
            _stateMachine.Ragdoll.EnableRagdoll();
        }

        public override void Tick(float deltaTime)
        {
            
        }

        public override void Exit()
        {
            _stateMachine.UnlockOtherStateMachines();
        }
    }
}