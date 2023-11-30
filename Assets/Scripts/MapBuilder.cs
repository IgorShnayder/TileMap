using System.Collections.Generic;
using UnityEngine;

public class MapBuilder : MonoBehaviour
{
    /// <summary>
    /// Данный метод вызывается автоматически при клике на кнопки с изображениями тайлов.
    /// В качестве параметра передается префаб тайла, изображенный на кнопке.
    /// Вы можете использовать префаб tilePrefab внутри данного метода.
    /// </summary>
    [SerializeField] private Map _map;
    [SerializeField] private Grid _grid;
    
    [SerializeField]private Tile _currentTile;
    private Camera _camera;
    
    private void Awake()
    {
        _camera = Camera.main;
    }
    
    public void StartPlacingTile(Tile tilePrefab)
    {
        if (_currentTile != null)
        {
            StartCoroutine(DestroyTileCoroutine(_currentTile));
        }
        
        _currentTile = Instantiate(tilePrefab, _map.transform);
    }
    
    private void Update()
    {
        if (_currentTile == null) return;
        PutTileOnMap();
    }
    
    private IEnumerator<Tile> DestroyTileCoroutine(Component tile)
    {
        yield return null;
        Destroy(tile.gameObject);
    }
    
    private void PutTileOnMap()
    {
        var mousePosition = Input.mousePosition;
        var ray = _camera.ScreenPointToRay(mousePosition);
        
        if (Physics.Raycast(ray, out var hitInfo))
        {
            var worldPosition = hitInfo.point;
            var cellPosition = _grid.WorldToCell(worldPosition);
            var cellCenterWorld = _grid.GetCellCenterWorld(cellPosition);
        
            _currentTile.transform.position = cellCenterWorld;

            var isAvailable = _map.IsCellAvailable(cellPosition);
            _currentTile.Highlighting(isAvailable);
            
            if (!isAvailable) return;

            if (!Input.GetMouseButtonDown(0)) return;
            
            _map.SetTile(cellPosition, _currentTile);
            _currentTile.ResetMaterialsColor();
            _currentTile = null;
        }
    }
}