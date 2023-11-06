using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Picture_", menuName = "ScriptableObjects/SO_Picture", order = 1)]
public class SO_Picture : ScriptableObject
{
    public string prefabName;
    public Sprite imageSprite;
    public Texture2D imageTexture2D;

    public bool isEmpty;
    public bool isPlaced;

    
}
