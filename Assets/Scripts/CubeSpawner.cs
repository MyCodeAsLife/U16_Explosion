using UnityEngine;

public class CubeSpawner : MonoBehaviour
{
    private BaseCube _prefabObject;
    private ObjectPool<BaseCube> _objectsPool;

    private Vector3 _startScale;
    private int _minNumberCoube;
    private int _maxNumberCoube;
    private float _maxSpawnChance;

    private void Awake()
    {
        _maxSpawnChance = 100;
        _minNumberCoube = 2;
        _maxNumberCoube = 6;
        _startScale = new Vector3(4, 4, 4);
        _prefabObject = Resources.Load<BaseCube>("Prefabs/Cube");
        _objectsPool = new ObjectPool<BaseCube>(_prefabObject, Create, Enable, Disable);
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
        _objectsPool.Return(cube);

        if (newSpawnChance > 0)
            SpawnObjects(position, scale, rotation, newSpawnChance);
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
}