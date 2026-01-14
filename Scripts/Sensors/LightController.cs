using UnityEngine;
using TMPro;

public class LightController : MonoBehaviour
{
    public RenderTexture currentPassthrough; 
    public Light worldLight;
    public TextMeshProUGUI debugText;

    private RenderTexture mipMapTexture;
    private Texture2D onePixelTexture;
    private float chrono;

    void Start()
    {
        chrono = 0f;
        mipMapTexture = new RenderTexture(128, 128, 0, RenderTextureFormat.ARGB32);
        mipMapTexture.useMipMap = true;
        mipMapTexture.autoGenerateMips = true;
        mipMapTexture.Create(); // Ensure it's created before use

        onePixelTexture = new Texture2D(1, 1, TextureFormat.RGB24, false);
    }

    float GetBrightness()
    {
        // Copiamos el passthrough a la RT con mipmaps
        Graphics.Blit(currentPassthrough, mipMapTexture);

        // Guardar el RenderTexture activo actual
        RenderTexture previousRT = RenderTexture.active;
        
        // Activar nuestro mipmap texture
        RenderTexture.active = mipMapTexture;
        
        // Leer el pixel desde el nivel de mipmap más bajo (1x1 aproximadamente)
        // Usamos ReadPixels en lugar de CopyTexture
        onePixelTexture.ReadPixels(new Rect(0, 0, 1, 1), 0, 0, false);
        onePixelTexture.Apply();
        
        // Restaurar el RenderTexture activo
        RenderTexture.active = previousRT;

        // Leer el color promedio
        Color color = onePixelTexture.GetPixel(0, 0);
        return (color.r + color.g + color.b) / 3f; // 0..1 aprox
    }

    float smoothBrightness = 0f;

    void Update()
    {
        chrono += Time.deltaTime;
        if (chrono >= 0.5f)
        {
            // Leer brillo cada frame (puedes bajarlo a cada 0.2s si quieres)
            float brightness = GetBrightness();

            // Suavizado para evitar parpadeos
            smoothBrightness = Mathf.Lerp(smoothBrightness, brightness, 0.1f);
            chrono = 0f;          
        }
        
        // Update debug text
        if (debugText != null)
        {
            float rawBrightness = GetBrightness();
            debugText.text = $"Raw Brightness: {rawBrightness:F3}\n" +
                            $"Smooth Brightness: {smoothBrightness:F3}\n" +
                            $"Light Intensity: {worldLight.intensity:F3}\n" +
                            $"Mipmap Levels: {mipMapTexture.mipmapCount}";
        }
        
        // Debug.Log($"Brillo: {smoothBrightness}");
        worldLight.intensity = smoothBrightness;
    }
}
