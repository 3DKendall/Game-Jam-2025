using UnityEngine;
using UnityEngine.Rendering;

public class Settings : MonoBehaviour
{
    public enum GraphicsQuality { Mobile, PC}

    public static GraphicsQuality graphicsQuality = GraphicsQuality.PC;

    public static float fov = 60f;

    public static bool postProcessing = true;

}
