using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    /// <summary>
    /// Данный метод вызывается автоматически при клике на кнопки с изображениями тайлов.
    /// В качестве параметра передается префаб тайла, изображенный на кнопке.
    /// Вы можете использовать префаб tilePrefab внутри данного метода.
    /// </summary>
    
    [SerializeField] private Grid _grid;
    [SerializeField] private Material _highlightingMaterialPutTale;
    [SerializeField] private Material _highlightingMaterialNotPutTale;
    
    private GameObject _tilePrefab;
    private Camera _mainCamera;
    private Material[] _basicMaterials;
    private bool _isItGameField;
    private Collider _hitCollider;
    private Renderer[] _spriteElements;
    
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    
    private void Update()
    {
        if (_tilePrefab == null) return;
        PutTaleOnMap();
    }

    public void StartPlacingTile(GameObject tilePrefab)
    {
        _tilePrefab = Instantiate(tilePrefab);
        SetLayerRecursivelyToChildren(_tilePrefab.transform, 2);
        TakeTileRenders();
        SaveTileBasicMaterials();
    }
    
    private void PutTaleOnMap()
    {
        var mousePosition = Input.mousePosition;
        var mousePositionRay = _mainCamera.ScreenPointToRay(mousePosition);
        
        var worldPosition = new Vector3();
        
        if (Physics.Raycast(mousePositionRay, out var hitInfo))
        {
            worldPosition = hitInfo.point;
            _hitCollider = hitInfo.collider;
        }
        
        var cellPosition = _grid.WorldToCell(worldPosition);
        var cellCenterWorld = _grid.GetCellCenterWorld(cellPosition);
        
        _tilePrefab.transform.position = cellCenterWorld;
        
        TakeTileRenders();
        DefiningObjectUnderTile();
        Highlighting();

        if (!_isItGameField || !Input.GetMouseButtonDown(0)) return;
        
        RestoreTileBasicMaterials();
        
        _tilePrefab.transform.position = cellCenterWorld;
        SetLayerRecursivelyToChildren(_tilePrefab.transform, 0);
        _tilePrefab = null;
    }

    private void Highlighting()
    {
        if (_isItGameField)
        {
            foreach (var renderer in _spriteElements)
            {
                renderer.material = _highlightingMaterialPutTale;
            }

            return;
        }

        foreach (var renderer in _spriteElements)
        {
            renderer.material = _highlightingMaterialNotPutTale;
        }
    }

    private void TakeTileRenders()
    {
        _spriteElements = _tilePrefab.GetComponentsInChildren<Renderer>();
    }

    private void SaveTileBasicMaterials()
    {
        _basicMaterials = new Material[_spriteElements.Length];
        
        for (var i = 0; i < _basicMaterials.Length; i++)
        {
            _basicMaterials[i] = _spriteElements[i].material;
        }
    }

    private void RestoreTileBasicMaterials()
    {
        for (var i = 0; i < _spriteElements.Length; i++)
        {
            _spriteElements[i].material = _basicMaterials[i];
        }
    }

    private void DefiningObjectUnderTile()
    {
        _isItGameField = false;
        
        if (!_hitCollider.CompareTag("GameField")) return;
        
        _isItGameField = true;
    }
    
    private void SetLayerRecursivelyToChildren(Transform currentTransform, int layer)
    {
        currentTransform.gameObject.layer = layer;
        
        foreach (Transform child in currentTransform)
        {
            SetLayerRecursivelyToChildren(child, layer);
        }
    }
}