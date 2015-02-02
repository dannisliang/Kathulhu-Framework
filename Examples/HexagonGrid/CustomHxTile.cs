using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using Kathulhu;

public class CustomHxTile : HxTile, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Material _mainMaterial;
    private Material _hoveredMaterial;

    protected override void Awake()
    {
        base.Awake();

        _mainMaterial = meshRenderer.sharedMaterial;
        Shader hoveredShader = Shader.Find( "Transparent/Diffuse" );
        _hoveredMaterial = new Material( hoveredShader );
        _hoveredMaterial.color = new Color32( 0, 90, 255, 96 );
    }

    public void OnPointerEnter( PointerEventData eventData )
    {
        transform.position = Position + Vector3.up * 0.015f;
        transform.localScale = Vector3.one * 1.1f;
        meshRenderer.sharedMaterial = _hoveredMaterial;
    }

    public void OnPointerExit( PointerEventData eventData )
    {
        transform.position = Position;
        transform.localScale = Vector3.one;
        meshRenderer.sharedMaterial = _mainMaterial;
    }

    public void OnPointerClick( PointerEventData eventData )
    {
        Debug.Log( "Clicked on HxTile " + ToString() );
    }

}
