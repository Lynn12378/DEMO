using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DEMO.DB
{
    public class DBMgr : MonoBehaviour
    {
        protected List<IMultipartFormSection> formData;
        protected string table;
        protected string action;
        protected string responseText;

        protected void SetForm(List<IMultipartFormSection> formData, string table, string action)
        {
            this.formData = formData;
            this.table = table;
            this.action = action;
        }

        protected IEnumerator SendData()
        {
            // 設置POST請求的URL
            string url = "http://localhost/DEMO/" + table + ".php?Action=" + action;
            
            if (formData == null || table == null)
            {
                Debug.LogError("Form data is not set.");
                yield break;
            }

            // 發送POST請求
            UnityWebRequest www = UnityWebRequest.Post(url, formData);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Data sent successfully!");
                responseText = www.downloadHandler.text;
            }
            else
            {
                Debug.Log("Error: " + www.error);
                responseText = null;
            }
        }
        public string GetResponseText()
        {
            return responseText;
        }
    }
}