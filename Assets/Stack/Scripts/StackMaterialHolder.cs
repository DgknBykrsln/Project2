using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackMaterialHolder : MonoBehaviour
{
    [SerializeField] private List<Material> materials;

    public Material GetMaterial(int index)
    {
        return materials[index % materials.Count];
    }
}