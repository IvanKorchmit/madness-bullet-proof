using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float maxDistance = 3;
    private Vector2 lastPlayerPosition;
    private void Start()
    {
        Player.Singleton.onEntityLand += Player_onPlayerJumpOrLand;
        Player.Singleton.onEntityJump += Player_onPlayerJumpOrLand;
        Player.Singleton.onEntityAttack += Player_onPlayerAttack;
        Player.Singleton.onEntityPunch += Player_onPlayerAttack;
        lastPlayerPosition = Player.Singleton.transform.position;
    }

    private void Player_onPlayerAttack()
    {
        StartCoroutine(Shake(0.009f, 0.1f, 0.5f));
    }

    private void Player_onPlayerJumpOrLand()
    {
        StartCoroutine(Shake(0.009f, 0.5f, 0.5f));
    }
    private void LateUpdate()
    {
        if (Player.Singleton == null) return;
        Vector2 deltaPosition = (Vector2)Player.Singleton.transform.position - lastPlayerPosition;
        lastPlayerPosition = Player.Singleton.transform.position;

        if (Vector2.Distance((Vector2)Player.Singleton.transform.position + deltaPosition, transform.position) < maxDistance || deltaPosition == Vector2.zero)
        {
            float z = transform.position.z;
            transform.position = Vector2.Lerp(transform.position, Player.Singleton.transform.position, Time.deltaTime * speed);
            transform.position = new Vector3(transform.position.x, transform.position.y, z);
        }   
        else
        {
            transform.position += (Vector3)deltaPosition;
        }
        
    }
    private IEnumerator Shake(float duration, float magnitude, float fade)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = PixelPerfectUtils.SnapVectorToPixel(originalPos + new Vector3(x, y, 0), PixelPerfectUtils.PIXELS_PER_UNIT);
            elapsed += Time.deltaTime;
            magnitude -= fade * Time.deltaTime;
            magnitude = Mathf.Clamp(magnitude, 0f, magnitude);
            yield return null;
        }
        transform.position = originalPos;
        transform.position = PixelPerfectUtils.SnapVectorToPixel(transform.position, PixelPerfectUtils.PIXELS_PER_UNIT);
    }
}
