using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Fusion.Addons.Physics;

using DEMO.GamePlay.EnemyScript;
using DEMO.GamePlay.Interactable;
using DEMO.Manager;
using DEMO.DB;

namespace DEMO.GamePlay.Player
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private float bulletSpeed = 1f;
        [SerializeField] private float bulletTime = 1f;
        [SerializeField] private int damage = 10;

        [Networked] private TickTimer life { get; set; }

        public Vector2 mousePosition;

        private PlayerController shooter;
        private PlayerRef shooterPlayerRef;

        public void Init(Vector2 mousePosition, PlayerController shooter)
        {
            life = TickTimer.CreateFromSeconds(Runner, bulletTime);
            this.mousePosition = mousePosition.normalized;
            this.shooter = shooter;
            shooterPlayerRef = shooter.GetPlayerNetworkData().playerRef;
            transform.Translate(Vector2.zero);
        }

        public override void FixedUpdateNetwork()
        {
            transform.Translate(Vector2.right * bulletSpeed);
            if (life.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
        }

        #region - OnTrigger -
        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.CompareTag("MapCollision"))
            {
                foreach (var kvp in GamePlayManager.Instance.playerOutputList)
                {
                    PlayerRef playerRefKey = kvp.Key;
                    PlayerOutputData playerOutputDataValue = kvp.Value;

                    if (shooterPlayerRef == playerRefKey)
                    {
                        playerOutputDataValue.bulletCollision++;
                        Debug.Log(playerRefKey.ToString() + "'s bullet collision is: " + playerOutputDataValue.bulletCollision.ToString());
                    }
                }

                AudioManager.Instance.Play("Hit");
                Runner.Despawn(Object);
            }

            var enemy = collider.GetComponent<Enemy>();
            var player = collider.GetComponent<PlayerController>();
            var livings = collider.GetComponent<Livings>();

            if (enemy != null)
            {
                enemy.TakeDamage(damage, shooterPlayerRef);
                AudioManager.Instance.Play("Hit");
                Runner.Despawn(Object);
            }
            else if(player != null)                  ////////////////////////// team will not shoot each other
            {
                if(player.GetPlayerNetworkData().playerRef != shooterPlayerRef)
                {
                    if(player.GetPlayerNetworkData().teamID != shooter.GetPlayerNetworkData().teamID || shooter.GetPlayerNetworkData().teamID == -1)
                    {
                        player.TakeDamage(damage, shooterPlayerRef);
                    }
                    AudioManager.Instance.Play("Hit");
                    Runner.Despawn(Object);
                }
            }
            else if(collider.CompareTag("Livings"))
            {
                livings.TakeDamage(damage, shooterPlayerRef);

                foreach (var kvp in GamePlayManager.Instance.playerOutputList)
                {
                    PlayerRef playerRefKey = kvp.Key;
                    PlayerOutputData playerOutputDataValue = kvp.Value;

                    if (shooterPlayerRef == playerRefKey)
                    {
                        playerOutputDataValue.bulletCollisionOnLiving++;
                    }
                }
                AudioManager.Instance.Play("Hit");
                Runner.Despawn(Object);
            }
        }
        #endregion
    }
}