using System.Collections.Generic;
using UnityEngine;

public class ColorChanger : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Material _standartMaterial;
    [SerializeField] private List<Material> _materials = new List<Material>();

    public void ChangeColor(Cube cube)
    {
        cube.ChangeMaterial(GetUniqueMaterial(cube.Material));
    }

    public void ResetMaterial(Cube cube)
    {
        cube.ChangeMaterial(_standartMaterial);
    }

    private Material GetUniqueMaterial(Material original)
    {
        if (_materials.Count == 0)
            return CreateRandomMaterial();

        var available = _materials
            .FindAll(material => MaterialsEqual(material, original) == false);

        return available.Count > 0
            ? available[Random.Range(0, available.Count)]
            : _materials[Random.Range(0, _materials.Count)];
    }

    private bool MaterialsEqual(Material newMaterial, Material originalMaterial)
    {
        return newMaterial && originalMaterial && newMaterial.color == originalMaterial.color && newMaterial.mainTexture == originalMaterial.mainTexture;
    }

    private Material CreateRandomMaterial()
    {
        return new Material(_standartMaterial) { color = Random.ColorHSV() };
    }
}