using System;
using Features.Perception.Sensors;
using Features.Player;
using UniRx;
using UnityEngine;

namespace Features.Character.Stats
{
    public class Health : MonoBehaviour
    {
        [SerializeField] private PlayerStatsData playerStatsData;
        [SerializeField] private SensorSystem sensorSystem;
        private ReactiveProperty<int> _health;
        private Subject<Unit> _death;

        private void Awake()
        {
            _health = new ReactiveProperty<int>();
            _health.Value = playerStatsData.maxHealth;
            _death = new Subject<Unit>();
            _health.Subscribe(val =>
            {
                if (val <= 0)
                {
                    _death.OnNext(Unit.Default);
                }
            }).AddTo(this);

            // After we lose health an event is emitted after a period of of not losing health has passed and health is restored to full
            _health
                .Zip(_health.Skip(1), (prev, curr) => new { Previous = prev, Current = curr })
                .Where(obj => obj.Current < obj.Previous && obj.Current > 0)
                .Throttle(TimeSpan.FromSeconds(playerStatsData.healDelay))
                .Subscribe(_ =>
                {
                    Heal(playerStatsData.maxHealth);
                }).AddTo(this);
        }

        private void Start()
        {
            sensorSystem.Heal.Subscribe(_ =>
            {
                Heal(playerStatsData.healAmount);
            }).AddTo(this);
        }

        public void TakeDamage(int damage)
        {
            _health.Value = Mathf.Max(_health.Value - damage, 0);
        }

        public void Heal(int amount)
        {
            if(_health.Value <= 0) return;
            _health.Value = Mathf.Min(_health.Value + amount, playerStatsData.maxHealth);
        }

        public void FullyHeal()
        {
            if(_health.Value <= 0) return;
            Heal(playerStatsData.maxHealth);
        }

        public ReactiveProperty<int> GetHealth => _health;

        public Subject<Unit> Death => _death;
    }
}
