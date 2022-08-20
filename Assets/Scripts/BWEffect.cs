using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class BWEffect : MonoBehaviour
{
    [SerializeField] float intensity;
    private Material material;

    public static BWEffect instance;

    // Creates a private material used to the effect
    void Awake()
    {
        material = new Material(Shader.Find("Hidden/BWDiffuse"));
        instance = this;
    }

    // Postprocess the image
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (intensity == 0)
        {
            Graphics.Blit(source, destination);
            return;
        }

        material.SetFloat("_bwBlend", intensity);
        Graphics.Blit(source, destination, material);
    }

    public void SetIntensity(float value)
    {
        intensity = value;
    }

    public void GetIntensity(float value)
    {
        intensity = value;
    }
}