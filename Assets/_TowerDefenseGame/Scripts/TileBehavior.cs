using UnityEngine;

public class TileBehavior : MonoBehaviour
{
    public Material highlightMat;
    public GameObject towerPrefab;


    Renderer _renderer;
    Material originalMaterial;
    GameObject tileTower;
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        originalMaterial = _renderer.material;

    }

    void OnMouseOver()
    {
        if (!TowerBuilder.Instance.HasSelectedTower())
            return;

        HighlightTile();

    }

    void OnMouseExit()
    {
        if (!TowerBuilder.Instance.HasSelectedTower())
            return;

        if (!tileTower)
        {
            _renderer.sharedMaterial = originalMaterial;
        }
    }

    void OnMouseDown()
    {
        if (!tileTower)
        {

            if (TowerBuilder.Instance.HasSelectedTower())
            {
                int cost = TowerBuilder.Instance.GetSelectedTowerCost();
                // can we afford the selected tower
                if (MoneyManager.Instance.BuyTower(cost))
                {
                    GameObject towerPrefab = TowerBuilder.Instance.GetSelectedTowerPrefab();

                    var tower = Instantiate(towerPrefab,
                     transform.parent.position, transform.parent.rotation);

                    tileTower = tower;
                }
                else
                {
                    // cant afford tower
                    Debug.LogWarning("Selected tower cannot be afforded");
                }
                TowerBuilder.Instance.ClearSelection();

            }
        }
    }

    void HighlightTile()
    {
        if (highlightMat)
        {
            _renderer.sharedMaterial = highlightMat;
        }
    }
}
