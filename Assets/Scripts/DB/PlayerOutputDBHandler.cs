using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

namespace DEMO.DB
{
    public class PlayerOutputDBHandler : DBMgr
    {
        [SerializeField] private PlayerOutputData playerOutputData;

        public void SendPOD(PlayerOutputData POD)
        {
            playerOutputData = POD;
            StartCoroutine(SendData());
        }

        public new IEnumerator SendData()
        {
            formData = new List<IMultipartFormSection>
            {
                new MultipartFormDataSection("PlayerOutputData", playerOutputData.ToJson()),
            };

            SetForm(formData, "OutputData");
            yield return StartCoroutine(base.SendData());

            string responseText = base.GetResponseText();
            Debug.Log(responseText);
        }
    }
}