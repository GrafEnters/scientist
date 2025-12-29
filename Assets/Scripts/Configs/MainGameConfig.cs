using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainGameConfig", menuName = "Scriptable Objects/MainGameConfig")]
public class MainGameConfig : ScriptableObject
{
    [Header("Свойства острова")]
    [Range(1, 32)]
    public int ChunkSize = 16;
    [Range(1, 16)]
    public int PolygonSize = 2;
    
    public Biome[] Biomes;
    
    
    [Header("Материи")]
    public List<Color> MattersColors;
    
    [Header("Свойства игрока")]
    public int ViewDistanceInChunks = 2;
}
