using UnityEngine;
using UnityEngine.UI;

public class MinimapArrow : MonoBehaviour
{
    public Transform playerTransform;
    public Transform baseTransform;
    public RectTransform arrowRectTransform;
    public float initialAngleOffset = 90f;

    private void Update()
    {
        if(playerTransform != null)
        {
            Vector3 direction = playerTransform.position - baseTransform.position;
    
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - initialAngleOffset;

            arrowRectTransform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        }
    }
}