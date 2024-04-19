using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ObjectPoolManager : MonoBehaviour
{
    private ParticleSystem _prefabExplosion;
    private BaseCube _prefabObject;
    private ObjectPool<BaseCube> _objectsPool;
    private AudioSource _explosionSound;

    private float _explosionRadius;
    private float _explosionForce;
    private float _maxSpawnChance;

    private Vector3 _startScale;
    private int _minNumberCoube;
    private int _maxNumberCoube;

    private void Awake()
    {
        _explosionRadius = 30;
        _explosionForce = 900;
        _maxSpawnChance = 100;
        _minNumberCoube = 2;
        _maxNumberCoube = 6;
        _startScale = new Vector3(4, 4, 4);
        _prefabExplosion = Resources.Load<ParticleSystem>("Prefabs/Explosion");
        _prefabObject = Resources.Load<BaseCube>("Prefabs/Cube");
        _objectsPool = new ObjectPool<BaseCube>(_prefabObject, Create, Enable, Disable);
        _explosionSound = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        _objectsPool.ReturnAll();
    }

    private void Start()
    {
        SpawnObjects(transform.position, _startScale, transform.rotation, _maxSpawnChance);
    }

    protected BaseCube Create(BaseCube prefab)
    {
        var item = Instantiate<BaseCube>(prefab);
        item.transform.SetParent(transform);

        return item;
    }

    protected void Enable(BaseCube cube)
    {
        cube.gameObject.SetActive(true);
        cube.Explode += OnExplode;
    }

    protected void Disable(BaseCube cube)
    {
        cube.Explode -= OnExplode;
        cube.gameObject.SetActive(false);
        cube.transform.position = Vector3.zero;
    }

    private void OnExplode(BaseCube cube)
    {
        float newSpawnChance = cube.SpawnChance;
        Vector3 scale = cube.transform.localScale;
        Vector3 position = cube.transform.position;
        Quaternion rotation = cube.transform.rotation;

        _objectsPool.Return(cube);
        SpawnObjects(position, scale, rotation, newSpawnChance);
        var interactiveObjects = GetExplodableObjects(position);

        foreach (var interactiveObject in interactiveObjects)
            interactiveObject.AddExplosionForce(_explosionForce, position, _explosionRadius);

        StartCoroutine(Explode(position));
    }

    private void SpawnObjects(Vector3 position, Vector3 scale, Quaternion rotation, float spawnChance)
    {
        float chance = Random.Range(0, _maxSpawnChance + 1);

        if (chance <= spawnChance)
        {
            int numberNewCoube = Random.Range(_minNumberCoube, _maxNumberCoube);

            for (int i = 0; i < numberNewCoube; i++)
            {
                var cube = _objectsPool.Get();
                cube.StartInitialization(position, scale, rotation, spawnChance);
            }
        }
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

    private IEnumerator Explode(Vector3 position)
    {
        if (_explosionSound.isPlaying)
            _explosionSound.Stop();

        _explosionSound.Play();
        const float Duration = 1.1f;
        var effect = Instantiate(_prefabExplosion);
        effect.transform.position = position;

        yield return new WaitForSeconds(Duration);
        Destroy(effect.gameObject);
    }
}
