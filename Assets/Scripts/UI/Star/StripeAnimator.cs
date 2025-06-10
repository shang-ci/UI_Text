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
        // �� Shader �е� _Time ������ʱ�����
        float time = Time.time;
        material.SetFloat("_StripeTime", time);
        material.SetFloat("_Speed", speed);
    }
}
