using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

/// <summary>
/// Reduces the colors in the scene to these colors
/// </summary>
[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Image Effects/Palette/Quantize")]
[ImageEffectAllowedInSceneView]
public class RenderWithPalette : MonoBehaviour
{
    private Shader shader = null;
    private Material _mat = null;

    /// <summary>
    /// List of used paletes
    /// </summary>
    public List<Palette> palettes = new List<Palette>();

    /// <summary>
    /// List of palettes that were used last render, used to detect changes.
    /// </summary>
    private List<Palette> palettes_old = new List<Palette>();

    /// <summary>
    /// Colors on screen
    /// </summary>
    public Color[] colors;

    /// <summary>
    /// The current color space
    /// </summary>
    public ColorSpace currentColorSpace;

    private Material InternalMaterial
    {
        get
        {
            if (_mat == null)
            {
                shader = Shader.Find("Hidden/Quantize");
                _mat = new Material(shader) { hideFlags = HideFlags.DontSave };
            }
            return _mat;
        }
    }

    private void Start()
    {
        if(SystemInfo.supportsImageEffects == false)
        {
            Debug.LogError("Image effects are not supported!");
            return;
        }

        #if UNITY_EDITOR
        currentColorSpace = PlayerSettings.colorSpace;
        #endif

        UpdateColors();

    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // Make sure colors are up to date.
        if(ShouldUpdateColor())
        {
            UpdateColors();
        }

        // If there are no errors and we have colors to use
        if (InternalMaterial && colors.Length > 0)
        {
            InternalMaterial.SetColorArray("_Colors", colors);
            InternalMaterial.SetInt("_NumColors", colors.Length);
            Graphics.Blit(source, destination, InternalMaterial);
        }
        else // Should never happen, but safety measure so we don't get a black screen
        {
            Graphics.Blit(source, destination);
        }
    }

    /// <summary>
    /// Check if colors need to be updated.
    /// </summary>
    /// <returns>If colors should be updated</returns>
    public bool ShouldUpdateColor()
    {
        // If the sizes are different.
        if (palettes.Count != palettes_old.Count)
        {
            return true;
        }
        else
        {
            // If the contents are different
            for (int i = 0; i < palettes.Count; i++)
            {
                if(palettes[i] != palettes_old[i]) { return true; }
            }
            return false;
        }
    }

    /// <summary>
    /// Update the colors displayed on screen.
    /// </summary>
    public void UpdateColors()
    {
        // First, destroy the material, as it has cached color properties
        DestroyMaterial();

        // Sum all the sizes
        int size = 0;
        
        foreach (var item in palettes)
        {
            // We must compare null here, as in the inspector they start as null.
            if(item != null) { size += item.colors.Length;}
        }

        // Create new color array
        List<Color> newColors = new List<Color>();

        foreach (var item in palettes)
        {
            if(item != null)
            {
                foreach (var c in item.colors)
                {
                    Color toAdd;

                    // Convert to linear if needed.
                    if (currentColorSpace == ColorSpace.Linear)
                    {
                        toAdd = c.linear;
                    }
                    else
                    {
                        toAdd = c;
                    }

                    // If we don't already have this color, add it
                    // This is because the shader only has a limited amount of space for colors,
                    // And every additional color adds processing cost per pixel.
                    if(!newColors.Any(test => test == toAdd))
                    {
                        newColors.Add(toAdd);
                    }
                    
                }
            }
        }
        
        // Set new colors
        colors = newColors.ToArray();
    }

    public void DestroyMaterial()
    {
        if (_mat)
        {
            DestroyImmediate(_mat);
        }
    }

    private void OnDisable()
    {
        DestroyMaterial();
    }
}
