using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour {

    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI posDisplay;
    public Image imageDisplay;
    private MouseController mouseController;
    void Start() {
        mouseController = MouseController._instance;
        mouseController.OnMouseClick += GetClickedCell;
    }


    private void GetClickedCell(Vector2 vector) {
        // Debug.Log("pos = " + vector.ToString());
        Vector2Int chunkPos = new() {
            x = Mathf.FloorToInt(vector.x / WorldGeneration.chunkSize),
            y = Mathf.FloorToInt(vector.y / WorldGeneration.chunkSize),
        };
        Transform chunk = WorldGeneration._instance.chunks[chunkPos.x, chunkPos.y];
        Texture2D texture = (Texture2D)chunk.GetComponent<MeshRenderer>().material.mainTexture;
        Color c = texture.GetPixel(Mathf.FloorToInt(vector.x - chunkPos.x * WorldGeneration.chunkSize), Mathf.FloorToInt(vector.y - chunkPos.y * WorldGeneration.chunkSize));
        float h = ColorInverseLerp(Color.black, Color.white, c);

        imageDisplay.color = c;
        posDisplay.text = new Vector2Int(Mathf.FloorToInt(vector.x - chunkPos.x * WorldGeneration.chunkSize), Mathf.FloorToInt(vector.y - chunkPos.y * WorldGeneration.chunkSize)).ToString();
        nameDisplay.text = h switch {
                _ when 0.13f >= h => "Water deep",
                _ when 0.285f >= h => "Water low",
                _ when 0.47f >= h => "Sand",
                _ when 0.66f >= h => "Grass low",
                _ when 0.78f >= h => "Grass",
                _ when 0.9f >= h => "Mountain h",
                _ when 1f >= h => "Snow",
                _ => "Error",
            };

    }
    public float ColorInverseLerp(Color a, Color b, Color value)
    {
        float rLerp = Mathf.InverseLerp(a.r, b.r, value.r);
        float gLerp = Mathf.InverseLerp(a.g, b.g, value.g);
        float bLerp = Mathf.InverseLerp(a.b, b.b, value.b);
        float aLerp = Mathf.InverseLerp(a.a, b.a, value.a);
        
        // Average the interpolations
        return (rLerp + gLerp + bLerp + aLerp) / 4f;
    }

}
