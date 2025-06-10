using System.Collections;
using UnityEngine;

public class UIPolygonChildrenMove : MonoBehaviour
{
    [Header("配置属性")]
    public float moveRange = 30f;
    public float duration = 0.2f;

    private Coroutine[] moveCoroutines;

    //void Start()
    //{
    //    foreach (Transform child in transform)
    //    {
    //        StartCoroutine(UpdateChildPos(child));
    //    }
    //}


    void OnEnable()
    {
        int count = transform.childCount;
        moveCoroutines = new Coroutine[count];
        int i = 0;
        foreach (Transform child in transform)
        {
            moveCoroutines[i++] = StartCoroutine(UpdateChildPos(child));
        }
    }

    void OnDisable()
    {
        if (moveCoroutines != null)
        {
            foreach (var c in moveCoroutines)
            {
                if (c != null) StopCoroutine(c);
            }
        }
        moveCoroutines = null;
    }

    IEnumerator UpdateChildPos(Transform child)
    {
        Vector3 prePos = child.localPosition;
        Vector3 fromPos = prePos;
        Vector3 targetPos = prePos + RandomOffset(moveRange);
        float pastTime = 0;
        while (true)
        {
            pastTime += Time.deltaTime;
            if (pastTime >= duration)
            {
                fromPos = targetPos;
                targetPos = prePos + RandomOffset(moveRange);
                pastTime = 0;
            }
            var t = pastTime / duration;
            child.localPosition = Vector3.Lerp(fromPos, targetPos, t);
            child.GetComponentInParent<UIPolygon>().SetAllDirty();//更新多边形

            yield return null;
        }
    }

    private Vector3 RandomOffset(float max)
    {
        return new Vector3(Random.Range(-max, max), Random.Range(-max, max), 0);
    }
}
