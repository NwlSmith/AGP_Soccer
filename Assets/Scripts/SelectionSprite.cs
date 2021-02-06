using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionSprite : MonoBehaviour
{

    private SpriteRenderer spriteRenderer;
    private Vector3 defaultLocalPosition = new Vector3(0, -.99f, 0);

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void AlignSpriteToPawn(Pawn newTarget)
    {
        spriteRenderer.enabled = true;
        transform.parent = newTarget.transform;
        transform.localPosition = defaultLocalPosition;
    }
}
