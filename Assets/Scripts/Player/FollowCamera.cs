using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform Target;

    void LateUpdate()
    {
        if (Target == null)
        {
            return;
        }

        // 更新相機的位置，使其跟隨目標的X和Y軸
        transform.position = new Vector3(Target.position.x, Target.position.y, transform.position.z);
    }
}