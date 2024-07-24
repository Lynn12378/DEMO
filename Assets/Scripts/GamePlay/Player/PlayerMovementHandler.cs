using UnityEngine;

using Fusion.Addons.Physics;
using DEMO.Manager;

namespace DEMO.GamePlay.Player
{
    public class PlayerMovementHandler : MonoBehaviour
    {
        [SerializeField] private NetworkRigidbody2D playerNetworkRigidbody = null;
        [SerializeField] private Transform Weapon = null;

        [SerializeField] private float moveSpeed = 5f;
        private float lastPlayTime;
        private float audioClipLength = 0.667f;

        public void Move(NetworkInputData data)
        {
            Vector2 moveVector = data.movementInput.normalized;
            Vector2 newVelocity = moveVector * moveSpeed;
            playerNetworkRigidbody.Rigidbody.velocity = newVelocity;

            if(newVelocity.magnitude > 0)
            {
                if (Time.time - lastPlayTime >= audioClipLength)
                {
                    AudioManager.Instance.Play("Walk");
                    lastPlayTime = Time.time;
                }
            }
        }

        public void SetRotation(Vector2 mousePosition)
        {
            float rotation = Vector2.SignedAngle(Vector2.up, mousePosition - new Vector2(transform.position.x, transform.position.y));
            Weapon.rotation = Quaternion.Euler(Vector3.forward * (rotation + 90));
        }
    }
}