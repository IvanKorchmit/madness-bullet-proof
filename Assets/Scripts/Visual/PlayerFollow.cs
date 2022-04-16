using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFollow : MonoBehaviour
{

    private static PlayerFollow singleTon;

    [Min(1f)]
    [SerializeField] private float speed;
    private void Start()
    {
        TimerUtils.AddTimer(0.02f, () => Player.Singleton.onEntityLand += Player_onPlayerJumpOrLand);
        TimerUtils.AddTimer(0.02f, () => Player.Singleton.onEntityJump += Player_onPlayerJumpOrLand);
        TimerUtils.AddTimer(0.02f, () => Player.Singleton.onEntityAttack += Player_onPlayerAttack);
        singleTon = this;

    }

    private void Player_onPlayerAttack()
    {
        StartCoroutine(Shake(0.009f, 0.25f, 0.5f));
    }

    private void Player_onPlayerJumpOrLand()
    {
        StartCoroutine(Shake(0.009f, 0.5f, 0.5f));
    }

    private void LateUpdate()
    {
        float z = transform.position.z;
        transform.position = Vector2.Lerp(transform.position, Player.Singleton.transform.position, Time.deltaTime * speed);
        transform.position = PixelPerfectUtils.SnapToPixel(transform.position, 16);
        transform.position = new Vector3(transform.position.x, transform.position.y, z);
    }
    private IEnumerator Shake(float duration, float magnitude, float fade)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            magnitude -= fade * Time.deltaTime;
            magnitude = Mathf.Clamp(magnitude, 0f, magnitude);
            yield return null;
        }
        transform.position = originalPos;
    }
}
