using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A palette is a collection of colors.
/// </summary>
[CreateAssetMenu(fileName = "Palette", menuName = "Palette/Palette", order = 1)]
public class Palette : ScriptableObject {
    public Color[] colors;
}
