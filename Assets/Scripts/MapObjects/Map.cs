using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map", menuName = "Map Object")]
public class Map : ScriptableObject
{
    public int buildIndex;

    public string mapname;
    public string description;

    public Enums.MapSize size;

    public Sprite thumbnail;
}
