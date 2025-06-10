using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// ���˵���Ч������ 
/// </summary>
public class CanvasManager : MonoBehaviour
{
    [Header("���˵���Ч������")]
    public GameObject mainMenuPanel; // ���˵����
    public Transform starPanel;
    public RectTransform buttonPanel;
    public RectTransform iconPanel;//���˵���һЩͼ��

    [Header("װ���˵���Ч������")]
    public GameObject equipmentMenuPanel; // װ���˵����
    public Transform starPanel2;
    public RectTransform buttonPanel2;
    public RectTransform iconPanel2;

    #region ���˵���Ч
    public void AnimateStarPanel()
    {
        if (starPanel != null)
        {
            var cg = starPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.DOFade(1, 1f);

            starPanel.localScale = Vector3.zero;
            Vector3 targetScale = new Vector3(106f,100f,143.1f);
            starPanel.DOScale(targetScale, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void AnimateButtonPanel()
    {
        if (buttonPanel != null)
        {
            var cg = buttonPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.DOFade(1, 1f);

            Vector2 targetPos = new Vector2(-91.8f,120f);
            //buttonPanel.anchoredPosition = new Vector2(1332, targetPos.y);
            buttonPanel.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.OutCubic);
        }
    }

    public void HideButtonPanel()
    {
        if (buttonPanel != null)
        {
            Vector2 targetPos = new Vector2(1253f, 156f);
            buttonPanel.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.InCubic);
        }
    }

    public void ShowIconPanel()
    {
        if (iconPanel != null)
        {
            //iconPanel.transform.position = new Vector3(1355f,0,0);
            Vector2 targetPos = new Vector2(-9f, 0);
            iconPanel.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.InCubic);
        }
    }

    public void HideIconPanel()
    {
        if (buttonPanel != null)
        {
            Vector2 targetPos = new Vector2(1355f, 0);
            iconPanel.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.InCubic);
        }
    }
    #endregion

    #region װ���˵���Ч
    public void ShowStarPanel2()
    {
        if (starPanel != null)
        {
            var cg = starPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.DOFade(1, 1f);

            starPanel2.localScale = Vector3.zero;
            Vector3 targetScale = new Vector3(1, 1, 1);
            starPanel2.DOScale(targetScale, 0.5f).SetEase(Ease.OutBack);
        }
    }

    public void ShowButtonPanel2()
    {
        if (buttonPanel != null)
        {
            var cg = buttonPanel.GetComponent<CanvasGroup>();
            if (cg != null) cg.DOFade(1, 1f);

            Vector2 targetPos = new Vector2(600, 156);
            //buttonPanel.position = new Vector3(1300, targetPos.y,0);
            buttonPanel2.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.OutCubic);
        }
    }

    public void HideButtonPanel2()
    {
        if (buttonPanel != null)
        {
            Vector2 targetPos = new Vector2(1253f, 156f);
            buttonPanel2.DOAnchorPos(targetPos, 0.3f).SetEase(Ease.InCubic);
        }
    }

    #endregion
}