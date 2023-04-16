using UnityEngine;

namespace Features.Character.Interaction
{
    public class TestInteractable : MonoBehaviour, IInteractable
    {
        public void Interact()
        {
            Debug.Log("Test Interactable interacts");
        }
    }
}