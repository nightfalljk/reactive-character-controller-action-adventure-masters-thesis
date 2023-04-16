using UnityEngine;

namespace Features.Character.Camera
{
    public class FollowPlayer : MonoBehaviour
    {
        [SerializeField] private Transform playerTransform;

        private float _cameraPositionYOffset;

        private void Awake()
        {
            _cameraPositionYOffset = transform.position.y;
        }
        private void Update()
        {
            Vector3 playerPos = playerTransform.position;
            transform.position = new Vector3(playerPos.x, _cameraPositionYOffset + playerPos.y, playerPos.z);
        }
    }
}
