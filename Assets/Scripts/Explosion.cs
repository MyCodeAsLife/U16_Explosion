using System.Collections;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(PlayEffect());
    }

    private IEnumerator PlayEffect()
    {
        const float Duration = 12f;
        yield return new WaitForSeconds(Duration);
        Destroy(this.gameObject);
    }
}
