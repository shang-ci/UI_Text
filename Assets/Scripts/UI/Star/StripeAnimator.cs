using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class StripeAnimator : MonoBehaviour
{
    public float speed = 1.0f;

    private Material material;

    void Start()
    {
        material = GetComponent<Renderer>().material;
    }

    void Update()
    {
        // 让 Shader 中的 _Time 参数随时间更新
        float time = Time.time;
        material.SetFloat("_StripeTime", time);
        material.SetFloat("_Speed", speed);
    }
}
