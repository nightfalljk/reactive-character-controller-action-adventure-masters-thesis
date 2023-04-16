using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Features.StateMachines
{
    public abstract class StateMachine : MonoBehaviour
    {

        private ReactiveProperty<State> _currentState;
        protected Dictionary<int, State> stateDict;

        protected void Awake()
        {
            _currentState = new ReactiveProperty<State>();
        }

        protected void Update()
        {
            _currentState?.Value?.Tick(Time.deltaTime);
        }

        public void SwitchState(int stateIndex)
        {
            _currentState.Value?.Exit();
            _currentState.Value = stateDict[stateIndex];
            _currentState.Value?.Enter();
        }

        public ReactiveProperty<State> CurrentState => _currentState;

        public Dictionary<int, State> StateDict => stateDict;
    }
}
