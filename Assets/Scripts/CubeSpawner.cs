using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[RequireComponent(typeof(AudioSource))]
public class CubeSpawner : MonoBehaviour
{
    private BaseCube _prefabObject;
    private ObjectPool<BaseCube> _objectsPool;
    //private ParticleSystem _prefabExplosion;
    //private AudioSource _explosionSound;

    //private float _explosionRadius;
    //private float _explosionForce;
    private float _maxSpawnChance;

    private Vector3 _startScale;
    private int _minNumberCoube;
    private int _maxNumberCoube;

    private void Awake()
    {
        //_explosionRadius = 30;
        //_explosionForce = 900;
        _maxSpawnChance = 100;
        _minNumberCoube = 2;
        _maxNumberCoube = 6;
        _startScale = new Vector3(4, 4, 4);
        _prefabObject = Resources.Load<BaseCube>("Prefabs/Cube");
        _objectsPool = new ObjectPool<BaseCube>(_prefabObject, Create, Enable, Disable);
        //_prefabExplosion = Resources.Load<ParticleSystem>("Prefabs/Explosion");
        //_explosionSound = GetComponent<AudioSource>();
    }

    private void OnDisable()
    {
        _objectsPool.ReturnAll();
    }

    private void Start()
    {
        SpawnObjects(transform.position, _startScale, transform.rotation, _maxSpawnChance);
    }

    private BaseCube Create(BaseCube prefab)
    {
        var item = Instantiate<BaseCube>(prefab);
        item.transform.SetParent(transform);

        return item;
    }

    private void Enable(BaseCube cube)
    {
        cube.gameObject.SetActive(true);
        cube.Used += OnCubeUsed;
    }

    private void Disable(BaseCube cube)
    {
        cube.Used -= OnCubeUsed;
        cube.gameObject.SetActive(false);
        cube.transform.position = Vector3.zero;
    }

    private void OnCubeUsed(BaseCube cube)
    {
        float newSpawnChance = cube.CurrentSpawnChance;
        Vector3 scale = cube.transform.localScale;
        Vector3 position = cube.transform.position;
        Quaternion rotation = cube.transform.rotation;
        //float chance = Random.Range(0, _maxSpawnChance + 1);
        _objectsPool.Return(cube);

        if (newSpawnChance > 0)
            SpawnObjects(position, scale, rotation, newSpawnChance);

        //if (chance <= newSpawnChance)
        //    SpawnObjects(position, scale, rotation, newSpawnChance);
        //else
        //    Explode(position, scale.x);
    }

    private void SpawnObjects(Vector3 position, Vector3 scale, Quaternion rotation, float spawnChance)
    {
        int numberNewCoube = Random.Range(_minNumberCoube, _maxNumberCoube);

        for (int i = 0; i < numberNewCoube; i++)
        {
            var cube = _objectsPool.Get();
            cube.StartInitialization(position, scale, rotation, spawnChance);
        }
    }

    //private void Explode(Vector3 position, float objectScale)
    //{
    //    var interactiveObjects = GetExplodableObjects(position);
    //    float newExplosionForce = _explosionForce / objectScale;
    //    float newExplosionRadius = _explosionRadius / objectScale;

    //    foreach (var interactiveObject in interactiveObjects)
    //        interactiveObject.AddExplosionForce(newExplosionForce, position, newExplosionRadius);

    //    StartCoroutine(ShowExplosion(position));
    //}

    //private List<Rigidbody> GetExplodableObjects(Vector3 position)
    //{
    //    Collider[] hits = Physics.OverlapSphere(position, _explosionRadius);
    //    List<Rigidbody> interactiveObjects = new List<Rigidbody>();

    //    foreach (Collider hit in hits)
    //        if (hit.attachedRigidbody != null)
    //            interactiveObjects.Add(hit.attachedRigidbody);

    //    return interactiveObjects;
    //}

    //private IEnumerator ShowExplosion(Vector3 position)
    //{
    //    const float Duration = 1.1f;

    //    if (_explosionSound.isPlaying)
    //        _explosionSound.Stop();

    //    _explosionSound.Play();
    //    var effect = Instantiate(_prefabExplosion);
    //    effect.transform.position = position;

    //    yield return new WaitForSeconds(Duration);
    //    Destroy(effect.gameObject);
    //}
}
