using UnityEngine;

public class TowerBuilder : MonoBehaviour
{
    [System.Serializable] // expose in inspector
    public class Tower
    {
        public string name;
        public GameObject prefab;
        public int cost;
    }

    // list of towers that could be built
    public Tower[] towers;

    public static TowerBuilder Instance { get; private set; }


    int selectedTowerIndex;
    bool selectedTower = false;

    void Awake()
    {
        // ensure only one instance of TowerBuilder exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // when loading different scene, persists this instance
        // DontDestroyOnLoad(gameObject);
        // not being used for now
    }

    void Start()
    {
        // SelectTower(1);
    }

    public void SelectTower(int index)
    {
        if (index >= 0 && index < towers.Length)
        {
            selectedTowerIndex = index;
            selectedTower = true;
        }
        else
        {
            Debug.LogError("Invalid tower index: " + index);
            selectedTower = false;
        }
    }

    public GameObject GetSelectedTowerPrefab()
    {
        // safe check before getting at index
        if (towers != null && selectedTowerIndex >= 0 && selectedTowerIndex < towers.Length)
        {
            return towers[selectedTowerIndex].prefab;
        }
        return null;
    }

    public int GetSelectedTowerCost()
    {
        // safe check before getting at index
        if (towers != null && selectedTowerIndex >= 0 && selectedTowerIndex < towers.Length)
        {
            return towers[selectedTowerIndex].cost;
        }
        return 0;
    }


    public int GetSelectedTowerCost(int index)
    {
        // safe check before getting at index
        if (towers != null && index >= 0 && index < towers.Length)
        {
            return towers[index].cost;
        }
        return 0;
    }

    public bool HasSelectedTower()
    {
        return selectedTower;
    }

    public void ClearSelection()
    {
        selectedTower = false;
        selectedTowerIndex = -100;
    }

}
