using Features.Perception.Sensors;
using UniRx;
using UnityEngine;

namespace Features.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private SensorSystem sensorSystem;
        [SerializeField] private GameObject interactObject;


        void Start()
        {
            sensorSystem.InteractableInView.Subscribe(inView =>
            {
                interactObject.SetActive(inView);
            }).AddTo(this);
        }

    }
}
