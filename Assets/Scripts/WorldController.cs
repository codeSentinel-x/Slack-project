using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WorldController : MonoBehaviour {

    public GameObject debugDisplay;
    public TextMeshProUGUI nameDisplay;
    public TextMeshProUGUI posDisplay;
    // public Image imageDisplay;
    private MouseController mouseController;
    void Start() {
        mouseController = MouseController._instance;
        mouseController.OnMouseClick += GetClickedCell;
    }


    private void GetClickedCell(Vector2 vector) {
        // Debug.Log("pos = " + vector.ToString());
        try {
            debugDisplay.SetActive(true);
            Vector2Int chunkPos = new() {
                x = Mathf.FloorToInt(vector.x / WorldGeneration.chunkSize),
                y = Mathf.FloorToInt(vector.y / WorldGeneration.chunkSize),
            };
            Debug.Log(chunkPos.ToString());
            GameObject chunk = WorldGeneration._instance.chunks[chunkPos];
            // Debug.Log(chunk.transform.position.ToString());
            Texture2D texture = (Texture2D)chunk.GetComponent<MeshRenderer>().material.mainTexture;
            Color c = texture.GetPixel(Mathf.FloorToInt(vector.x - chunkPos.x * WorldGeneration.chunkSize), Mathf.FloorToInt(vector.y - chunkPos.y * WorldGeneration.chunkSize));
            string name = chunk.GetComponent<ChunkController>().chunkH[Mathf.FloorToInt(vector.x - chunkPos.x * WorldGeneration.chunkSize), Mathf.FloorToInt(vector.y - chunkPos.y * WorldGeneration.chunkSize)].name;

            // imageDisplay.color = c;
            posDisplay.text = new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y)).ToString();
            nameDisplay.text = name;
        }
        catch (SystemException e) {
            debugDisplay.SetActive(false);
            Debug.Log("No tile selected\nError: " + e);
        }

    }
    public float ColorInverseLerp(Color a, Color b, Color value) {
        float rLerp = Mathf.InverseLerp(a.r, b.r, value.r);
        float gLerp = Mathf.InverseLerp(a.g, b.g, value.g);
        float bLerp = Mathf.InverseLerp(a.b, b.b, value.b);
        float aLerp = Mathf.InverseLerp(a.a, b.a, value.a);

        // Average the interpolations
        return (rLerp + gLerp + bLerp + aLerp) / 4f;
    }

}
