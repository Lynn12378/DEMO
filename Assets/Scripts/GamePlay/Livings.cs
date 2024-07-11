using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D.Animation;
using UnityEngine.UI;
using Fusion;
using Fusion.Addons.Physics;
using DEMO.GamePlay.Player;
using DEMO.Manager;
using DEMO.DB;

namespace DEMO.GamePlay
{
    public class Livings : NetworkBehaviour
    {
        public enum LivingsType
        {
            Cat1,
            Cat2,
            Cat3,
            Cat4,
            Cat5,
            Cat6,
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

        [SerializeField] private float moveSpeed;   // 1


        #region - Initialize -
        public override void Spawned() 
        {
            var livingsTransform = GameObject.Find("Livings");
            transform.SetParent(livingsTransform.transform, false);

            Init(livingsID);
            GamePlayManager.Instance.livingsList.Add(this);

            Hp = maxHp;
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
                foreach (var kvp in GamePlayManager.Instance.playerOutputList)
                {
                    PlayerRef playerRefKey = kvp.Key;
                    PlayerOutputData playerOutputDataValue = kvp.Value;

                    if (shooter == playerRefKey)
                    {
                        playerOutputDataValue.AddKillNo_RPC();
                    }
                }

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
    }
}