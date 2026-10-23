using UnityEngine;
public class DestroyAfterSeconds : MonoBehaviour
{
    public float delayTime = 3f;
    void Start()
    {
        Destroy(gameObject, delayTime);
    }
}