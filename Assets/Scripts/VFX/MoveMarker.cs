using UnityEngine;

public class MoveMarker : MonoBehaviour
{
    [SerializeField] private float lifeTime = 1.0f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
