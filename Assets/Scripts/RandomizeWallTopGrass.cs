using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class RandomizeWallTopGrass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        System.Random random = new System.Random(gameObject.transform.position.GetHashCode());
        string typeString = "TX Tileset Grass";
        double rn = random.NextDouble();

        if (rn <= 0.8) {
            typeString += " " + random.Next(16);
        } else {
            typeString += " Flower " + random.Next(16);
        }

        UnityEngine.Sprite sprite = ((UnityEngine.Tilemaps.Tile) AssetDatabase.LoadAssetAtPath("Assets/Cainos/Pixel Art Top Down - Basic/Tile Palette/TP Grass/" + typeString + ".asset", typeof(UnityEngine.Tilemaps.Tile))).sprite;
        gameObject.GetComponent<SpriteRenderer>().sprite = sprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
