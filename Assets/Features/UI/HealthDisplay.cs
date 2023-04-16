using Features.Character.Stats;
using TMPro;
using UniRx;
using UnityEngine;

namespace Features.UI
{
    public class HealthDisplay : MonoBehaviour
    {

        [SerializeField] private Health health;
        [SerializeField] private TMP_Text healthText;

        private void Start()
        {
            health.GetHealth.Subscribe(healthAmount =>
            {
                healthText.text = "Health: " + healthAmount;
            }).AddTo(this);
        }
    }
}
