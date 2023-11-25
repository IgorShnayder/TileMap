using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _allowingColor;
    [SerializeField] private Color _prohibitingColor;

    private List<Material> _currentMaterials = new();
    
    private void Awake()
    {
        SaveTaleMaterials();
    }

    private void SaveTaleMaterials()
    {
        var renderers = GetComponentsInChildren<MeshRenderer>();

        foreach (var meshRenderer in renderers)
        {
            _currentMaterials.Add(meshRenderer.material);
        }
    }
    
    public void Highlighting(bool isCellAvailable)
    {
        if (isCellAvailable)
        {
            foreach (var material in _currentMaterials)
            {
                material.color = _allowingColor;
            }
        }
        else
        {
            foreach (var material in _currentMaterials)
            {
                material.color = _prohibitingColor;
            }
        }
    }
    
    public void ResetMaterialsColor()
    {
        foreach (var material in _currentMaterials)
        {
            material.color = Color.white;
        }
    }
}
