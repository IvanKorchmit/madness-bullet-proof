using UnityEngine;

public class SlicedColliderSet : MonoBehaviour
{
    void Start()
    {
        GetComponent<BoxCollider2D>().size = GetComponent<SpriteRenderer>().size;
    }
}
