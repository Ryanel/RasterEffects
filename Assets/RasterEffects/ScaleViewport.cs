using UnityEngine;

/// <summary>
/// Renders the viewport to a lower resolution
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Raster/Scale Viewport")]
public class ScaleViewport : MonoBehaviour {
    
    /// <summary>
    /// How the screen's resolution is scaled
    /// </summary>
    public enum ScaleMode
    {
        ConstantDownscale,
        ScaleVerticalInterger,
        ExactVertical
    }

    /// <summary>
    /// The current scale mode
    /// </summary>
    public ScaleMode scaleMode;

    /// <summary>
    /// The manual downsampleScale (for constant downscale)
    /// </summary>
    [Range(1, 8)]  
    public float constantDownscale = 2;

    /// <summary>
    /// The maximum vertical resolution, for ExactVertical and ScaleVerticalInterger
    /// </summary>
    public int maximumVerticalResolution = 360;

    /// <summary>
    /// Internal downsample (for different scale modes)
    /// </summary>
    private float internalDownsampleScale = 1;

    /// <summary>
    /// The current horizontal resolution
    /// </summary>
    public int horizontalResolution;

    /// <summary>
    /// The current vertical resolution
    /// </summary>
    public int verticalResolution;
    
    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Determine scale based on scaling mode
        switch (scaleMode)
        {
            case ScaleMode.ScaleVerticalInterger: // Set scale so that it's downscaling by an interger, but it's resolution is less then maximumVertical Resolution.
                internalDownsampleScale = 1;
                verticalResolution = Screen.height;
                while (verticalResolution > maximumVerticalResolution)
                {
                    internalDownsampleScale++;
                    verticalResolution = Mathf.FloorToInt(Screen.height / internalDownsampleScale);
                }
                horizontalResolution = Mathf.FloorToInt(Screen.width / internalDownsampleScale);
                break;
            case ScaleMode.ExactVertical: // Force maximum vertical resolution
                verticalResolution = maximumVerticalResolution;
                internalDownsampleScale = Screen.height / verticalResolution;
                horizontalResolution = Mathf.FloorToInt(Screen.width / internalDownsampleScale);
                break;
            case ScaleMode.ConstantDownscale:
            default:
                horizontalResolution = Mathf.FloorToInt(Screen.width / constantDownscale);
                verticalResolution = Mathf.FloorToInt(Screen.height / constantDownscale);
                break;
        }

        // Sanity check values

        if (horizontalResolution <= 0) { horizontalResolution = 1; }
        if (verticalResolution <= 0) { verticalResolution = 1; }

        // Create renderTexture
        RenderTexture scaled = RenderTexture.GetTemporary(horizontalResolution, verticalResolution);
        scaled.filterMode = FilterMode.Point;

        // Blit
        Graphics.Blit(source, scaled);
        Graphics.Blit(scaled, destination);

        RenderTexture.ReleaseTemporary(scaled);
    }
}
