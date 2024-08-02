using System;
using TMPro;
using UnityEngine;

public class WorldController : MonoBehaviour {

    [SerializeField] private GameObject _debugDisplay;
    [SerializeField] private TextMeshProUGUI _nameDisplay;
    [SerializeField] private TextMeshProUGUI _posDisplay;


    private void Start() {

        MouseController._OnMouseClickLeft += GetClickedCell;
    }


    private void GetClickedCell(Vector2 vector) {
        try {
            _debugDisplay.SetActive(true);
            Vector2Int chunkPos = new() {
                x = Mathf.FloorToInt(vector.x / WorldGeneration._chunkSize),
                y = Mathf.FloorToInt(vector.y / WorldGeneration._chunkSize),
            };
            Debug.Log(chunkPos.ToString());
            GameObject chunk = WorldGeneration._instance._currentChunksDict[chunkPos];
            Texture2D texture = (Texture2D)chunk.GetComponent<MeshRenderer>().material.mainTexture;
            Color c = texture.GetPixel(Mathf.FloorToInt(vector.x - chunkPos.x * WorldGeneration._chunkSize), Mathf.FloorToInt(vector.y - chunkPos.y * WorldGeneration._chunkSize));
            string name = chunk.GetComponent<ChunkController>()._chunks[Mathf.FloorToInt(vector.x - chunkPos.x * WorldGeneration._chunkSize), Mathf.FloorToInt(vector.y - chunkPos.y * WorldGeneration._chunkSize)]._terrainTypeName;
            _posDisplay.text = new Vector2Int(Mathf.FloorToInt(vector.x), Mathf.FloorToInt(vector.y)).ToString();
            _nameDisplay.text = name;
        }
        catch (SystemException e) {
            _debugDisplay.SetActive(false);
            Debug.Log("No tile selected\nError: " + e);
        }

    }
    public float ColorInverseLerp(Color a, Color b, Color value) {
        float rLerp = Mathf.InverseLerp(a.r, b.r, value.r);
        float gLerp = Mathf.InverseLerp(a.g, b.g, value.g);
        float bLerp = Mathf.InverseLerp(a.b, b.b, value.b);
        float aLerp = Mathf.InverseLerp(a.a, b.a, value.a);
        return (rLerp + gLerp + bLerp + aLerp) / 4f;
    }

}
