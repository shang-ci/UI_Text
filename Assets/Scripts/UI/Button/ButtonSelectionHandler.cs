using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSelectionHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler
{
    public UIPolygonChildrenMove redPolygonMove;
    public UIPolygonChildrenMove cyanPolygonMove;

    // 放大比例
    public float pointerScale = 1.1f;
    // 动画速度
    public float scaleLerpSpeed = 10f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    private bool isPointerOver = false;

    private void Awake()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    private void Update()
    {
        // 平滑缩放
        if (transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleLerpSpeed);
        }
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetPolygonMoveEnabled(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetPolygonMoveEnabled(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetPolygonMoveEnabled(true);
        isPointerOver = true;
        targetScale = originalScale * pointerScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetPolygonMoveEnabled(false);
        isPointerOver = false;
        targetScale = originalScale;
    }

    /// <summary>
    /// 控制红色和青色多边形的移动
    /// </summary>
    private void SetPolygonMoveEnabled(bool enabled)
    {
        if (redPolygonMove != null)
            redPolygonMove.gameObject.SetActive(enabled);
        if (cyanPolygonMove != null)
            cyanPolygonMove.gameObject.SetActive(enabled);
    }
}