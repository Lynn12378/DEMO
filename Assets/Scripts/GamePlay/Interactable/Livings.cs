using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;
using DEMO.Manager;
using DEMO.UI;

namespace DEMO.GamePlay.Interactable
{
    public class Livings : NetworkBehaviour, IInteractable
    {
        public enum LivingsType
        {
            Cat1,
            Cat2,
            Cat3,
            Cat4,
            Dog1,
            Dog2,
            Dog3,
            Dog4,
            Goose,
            Squirrel,
            Frog,
        }

        [SerializeField] private NetworkRigidbody2D livingsNetworkRigidbody = null;
        [Networked] public int livingsID { get; set;}
        public LivingsType livingsType;
        [SerializeField] private SpriteResolver spriteResolver;
        [SerializeField] public SpriteRenderer spriteRenderer;


        [SerializeField] public Slider hPSlider;
        [Networked] [OnChangedRender(nameof(HandleHpChanged))]
        public int Hp { get; set; }
        private int maxHp = 30;

        [SerializeField] private float moveSpeed = 1.0f;

        private Vector3 moveDirection;
        private bool isMoving = false;

        private MapInteractionManager mapInteractionManager;


        #region - Initialize -
        private void Start()
        {
            mapInteractionManager = FindObjectOfType<MapInteractionManager>();
        }

        public override void Spawned() 
        {
            var livingsTransform = GameObject.Find("Livings");
            transform.SetParent(livingsTransform.transform, false);

            Init(livingsID);
            GamePlayManager.Instance.livingsList.Add(this);

            Hp = maxHp;

            // Start moving randomly
            StartCoroutine(RandomMovement());
        }

        public void Init(int livingsID)
        {
            livingsType = (LivingsType) livingsID;
            spriteResolver.SetCategoryAndLabel("livings", livingsType.ToString());

            SetLivingsID_RPC(livingsID);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetLivingsID_RPC(int livingsID)
        {
            this.livingsID = livingsID;
		}
        #endregion

        #region - Hp -
        public void TakeDamage(int damage, PlayerRef shooter)
        {
            Hp -= damage;
            SetLivingsHP_RPC(Hp);
            if (Hp <= 0)
            {
                DespawnLivings_RPC(Object);
            }
        }

        private void HandleHpChanged()
        {
            hPSlider.value = Hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void SetLivingsHP_RPC(int hp)
        {
            Hp = hp;
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        public void DespawnLivings_RPC(NetworkObject netObj)
        {
            Runner.Despawn(netObj);
        }
        #endregion

        #region - Movement -
        private IEnumerator RandomMovement()
        {
            while (true)
            {
                // Randomly choose a direction to move
                moveDirection = Random.insideUnitCircle.normalized;

                isMoving = true;
                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f)); // Move for a random duration
                isMoving = false;

                yield return new WaitForSeconds(Random.Range(1.0f, 3.0f)); // Wait before next movement
            }
        }

        private void FixedUpdate()
        {
            if (isMoving)
            {
                // Move in the chosen direction
                Vector3 newPosition = transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime;
                livingsNetworkRigidbody.Rigidbody.MovePosition(newPosition);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            // Reverse direction on collision with map boundaries or obstacles
            if (collision.gameObject.CompareTag("MapCollision"))
            {
                moveDirection = -moveDirection; // Reverse direction
            }
        }
        #endregion

        #region - Interact -
        public void Interact()
        {
            switch(livingsID)
            {
                case 0:
                case 1:
                case 2:
                case 3:
                    mapInteractionManager.SetCurrentInteraction("Cat");
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    mapInteractionManager.SetCurrentInteraction("Dog");
                    break;
                case 8:
                    mapInteractionManager.SetCurrentInteraction("Goose");
                    break;
                case 9:
                    mapInteractionManager.SetCurrentInteraction("Squirrel");
                    break;
                case 10:
                    mapInteractionManager.SetCurrentInteraction("Frog");
                    break;
            }
            
        }
        #endregion
    }
}