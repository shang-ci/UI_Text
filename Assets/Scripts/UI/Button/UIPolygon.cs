using UnityEngine;
using UnityEngine.UI;

public class UIPolygon : Graphic
{
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        if (transform.childCount < 3)
        {
            return;
        }
        vh.Clear();
        foreach (Transform child in transform)
        {
            vh.AddVert(child.localPosition, color, Vector2.zero);
        }
        for (int i = 0; i < transform.childCount - 2; i++)
        {
            vh.AddTriangle(0, i + 1, i + 2);
        }
    }
}
