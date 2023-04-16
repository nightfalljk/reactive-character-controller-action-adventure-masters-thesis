using UnityEngine;

namespace Features.Character.Stats
{
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "PlayerStats/Create new Player Stats Data", order = 0)]
    public class PlayerStatsData : ScriptableObject
    {
        public int maxHealth;
        public int healAmount;
        public float healDelay;
    }
}