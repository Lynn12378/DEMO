using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Fusion;
using Photon.Voice.Unity;
using Photon.Voice.Fusion;

using DEMO.DB;
using DEMO.Manager;

namespace DEMO.GamePlay.Player
{
    public class PlayerVoiceDetection : NetworkBehaviour
    {
        [SerializeField] public VoiceNetworkObject voiceObject;
        [SerializeField] private PlayerNetworkData playerNetworkData;
        [SerializeField] private PlayerOutputData playerOutputData;

        public Recorder rec;
        [SerializeField] private List<PlayerController> playersInRange = new List<PlayerController>();
        
        private void Start()
        {
            GamePlayManager.Instance.playerVoiceList.Add(Object.InputAuthority, this);   

            if (playerNetworkData.playerRef == Runner.LocalPlayer)
            {
                rec = voiceObject.RecorderInUse;
                rec.TransmitEnabled = false;
            }
        }

        #region - UI -
        public void AudioCheck()
        {
            if(rec != null && rec.IsCurrentlyTransmitting)
            {
                if(playerNetworkData.playerRef == Runner.LocalPlayer)
                {
                    playerNetworkData.uIManager.UpdateMicIconColor(0);
                    // Use playerRef to test
                    playerNetworkData.uIManager.UpdateMicTxt(playerNetworkData.playerRefString);
                }
                else 
                {
                    playerNetworkData.uIManager.UpdateMicIconColor(-1);
                    playerNetworkData.uIManager.UpdateMicTxt("none");
                }
            }
            else
            {
                foreach (var kvp in GamePlayManager.Instance.playerVoiceList)
                {
                    PlayerVoiceDetection playerVoiceDetection = kvp.Value;

                    if (playerVoiceDetection.voiceObject.IsSpeaking)
                    {
                        playerNetworkData.uIManager.UpdateMicIconColor(1);
                        playerNetworkData.uIManager.UpdateMicTxt(kvp.Key.ToString());
                    }
                    else
                    {
                        playerNetworkData.uIManager.UpdateMicIconColor(-1);
                        playerNetworkData.uIManager.UpdateMicTxt("none");
                    }
                }
            }
        }
        #endregion

        #region - Distance Limit -
        private void EnableMicrophone(PlayerController playerController, bool enable)
        {
            var speaker = playerController.GetPlayerVoiceDetection().voiceObject.SpeakerInUse;
            if(enable == false && rec != null)
            {
                rec.TransmitEnabled = enable;
                speaker.enabled = enable;
            }
            else
            {
                rec.TransmitEnabled = rec.TransmitEnabled;
                speaker.enabled = enable;
            }
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if(collider.CompareTag("Player"))
            {
                var colliderPlayerController = collider.GetComponent<PlayerController>();
                playersInRange.Add(colliderPlayerController);
                EnableMicrophone(colliderPlayerController, true);
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                var colliderPlayerController = collider.GetComponent<PlayerController>();
                playersInRange.Remove(colliderPlayerController);
                EnableMicrophone(colliderPlayerController, false);
            }
        }
        #endregion

        #region - Voice Detection -

        #endregion
    }
}
