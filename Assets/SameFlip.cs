using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SameFlip : MonoBehaviour
{
    private SpriteRenderer self;
    private SpriteRenderer parent;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<SpriteRenderer>();
        parent = transform.parent.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        self.flipX = parent.flipX;
    }
}
