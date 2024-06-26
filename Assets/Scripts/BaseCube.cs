using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Renderer))]
public class BaseCube : MonoBehaviour, IPointerClickHandler
{
    private const float Half = 0.5f;
    private const float MaxSpawnChance = 100f;

    private Explosion _prefabExplosion;

    private float _explosionRadius;
    private float _explosionForce;

    public event Action<BaseCube> Used;

    public float CurrentSpawnChance { get; private set; }

    private void Awake()
    {
        _explosionRadius = 30;
        _explosionForce = 900;
        _prefabExplosion = Resources.Load<Explosion>("Prefabs/Explosion");
    }

    public void StartInitialization(Vector3 position, Vector3 scale, Quaternion rotation, float spawnChance)
    {
        CurrentSpawnChance = spawnChance * Half;

        SwitchColor();
        SetTransform(position, scale, rotation);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        float chance = UnityEngine.Random.Range(1, MaxSpawnChance + 1);

        if (chance > CurrentSpawnChance)
        {
            CurrentSpawnChance = 0;
            Explode();
        }

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

    private void Explode()
    {
        var interactiveObjects = GetExplodableObjects(transform.position);
        float newExplosionForce = _explosionForce / transform.localScale.x;
        float newExplosionRadius = _explosionRadius / transform.localScale.x;

        foreach (var interactiveObject in interactiveObjects)
            interactiveObject.AddExplosionForce(newExplosionForce, transform.position, newExplosionRadius);

        var effect = Instantiate(_prefabExplosion);
        effect.transform.position = transform.position;
    }

    private List<Rigidbody> GetExplodableObjects(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, _explosionRadius);
        List<Rigidbody> interactiveObjects = new List<Rigidbody>();

        foreach (Collider hit in hits)
            if (hit.attachedRigidbody != null)
                interactiveObjects.Add(hit.attachedRigidbody);

        return interactiveObjects;
    }
}