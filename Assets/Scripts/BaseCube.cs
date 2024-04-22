using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class BaseCube : MonoBehaviour, IPointerClickHandler
{
    private const float Half = 0.5f;

    public float SpawnChance { get; private set; }

    public event Action<BaseCube> Used;

    public void StartInitialization(Vector3 position, Vector3 scale, Quaternion rotation, float spawnChance)
    {
        SpawnChance = spawnChance * Half;

        SwitchColor();
        SetTransform(position, scale, rotation);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Used?.Invoke(this);
    }

    private void SetTransform(Vector3 position, Vector3 scale, Quaternion rotation)
    {
        transform.localScale = scale * Half;
        transform.rotation = rotation;
        float newX = UnityEngine.Random.Range(-scale.x, scale.x);
        float newY = UnityEngine.Random.Range(0, scale.y);
        float newZ = UnityEngine.Random.Range(-scale.z, scale.z);
        Vector3 offset = new Vector3(newX, newY, newZ);
        transform.position = position + offset;
    }

    private void SwitchColor()
    {
        float red = UnityEngine.Random.Range(0f, 1f);
        float green = UnityEngine.Random.Range(0f, 1f);
        float blue = UnityEngine.Random.Range(0f, 1f);
        float alpha = 1;
        Color newColor = new Color(red, green, blue, alpha);
        GetComponent<Renderer>().material.color = newColor;
    }
}
