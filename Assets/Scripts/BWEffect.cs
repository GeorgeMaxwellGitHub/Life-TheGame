using UnityEngine;
using System.Collections;

//This script allows you to handle saturation in shader on camera
//All thanks to Alan Zucconi for the great explanation
//https://www.alanzucconi.com/2015/07/08/screen-shaders-and-postprocessing-effects-in-unity3d/

[ExecuteInEditMode]
public class BWEffect : MonoBehaviour
{
    [SerializeField] float intensity;
    [SerializeField] Material material;

    public static BWEffect instance;

    // Creates a private material used to the effect
    void Awake()
    {
        //material = new Material(Shader.Find("Hidden/BWDiffuse"));
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