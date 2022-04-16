using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public const float PPU = 16;
    private Transform cam;
    private Vector2 lastCamPos;
    [SerializeField] private Vector2 parallaxModifier;
    [SerializeField] private Vector2 movement;
    [SerializeField] private bool infiniteOnYAxis;
    private Vector2 textureUnitSize;
    private void Start()
    {
        cam = Camera.main.transform;
        lastCamPos = cam.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D spriteTex = sprite.texture;
        textureUnitSize.x = spriteTex.width / sprite.pixelsPerUnit;
        textureUnitSize.y = spriteTex.height / sprite.pixelsPerUnit;
    }
    private void LateUpdate()
    {
        Vector2 deltaPosition = (Vector2)cam.position - lastCamPos;
        transform.position += (Vector3)(deltaPosition * parallaxModifier);
        transform.position += (Vector3)movement * Time.deltaTime;


        lastCamPos = cam.position;
        if (Mathf.Abs(cam.position.x - transform.position.x) >= textureUnitSize.x)
        {
            float offset = (cam.position.x - transform.position.x) % textureUnitSize.x;
            transform.position = new Vector3(cam.position.x + offset, transform.position.y);
        }
        if (Mathf.Abs(cam.position.y - transform.position.y) >= textureUnitSize.y && infiniteOnYAxis)
        {
            float offset = (cam.position.y - transform.position.y) % textureUnitSize.y;
            transform.position = new Vector3(transform.position.x, cam.position.y + offset);
        }
        transform.position = PixelPerfectUtils.SnapToPixel(transform.position, 16);
    }
}
