using System;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    //Class used for model lookup with GetComponent
    [SerializeField] 
    private string outlineColorShaderVariableName = "_OutlineColor";

    private SkinnedMeshRenderer _meshRenderer;
    private MaterialPropertyBlock _outlineColorPropertyBlock;

    public void Awake()
    {
        _meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _outlineColorPropertyBlock = new MaterialPropertyBlock();
    }
    
    public void ChangeOutlineColor(Color newColor)
    {
        _meshRenderer.GetPropertyBlock(_outlineColorPropertyBlock);
        
        _outlineColorPropertyBlock.SetColor(outlineColorShaderVariableName, newColor);
        
        _meshRenderer.SetPropertyBlock(_outlineColorPropertyBlock);
    }
}
