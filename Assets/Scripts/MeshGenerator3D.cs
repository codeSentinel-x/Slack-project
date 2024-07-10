using System;
using System.Collections;
using System.Collections.Generic;
using MyUtils.Structs;
using UnityEngine;

public class MeshGenerator3D : MonoBehaviour {

    
    /*public static MeshGenerator3D _instance;
    public NoiseGeneration noiseGeneration;
    public WeightedNoiseSetting[] ws;
    public AnimationCurve hCurve;
    public Material _sourceMaterial;
    public BiomeSO _biome;
    private GameObject chunkHolder;

    void Awake() {
        _instance = this;
    }
    void Start() {
        noiseGeneration = NoiseGeneration._instance;
        noiseGeneration._onNoiseGenerationCompleat += GenerateTerrainMesh;

    }
    public void GenerateMesh() {
        noiseGeneration.GenerateNoise(ws);
    }
    public void GenerateMesh(WeightedNoiseSetting[] ws, uint seed, int chunkSize) {
        if (chunkHolder == null) chunkHolder = new("Chunk holder");
        else {
            Destroy(chunkHolder);
            chunkHolder = new("Chunk holder");
        }
        NoiseGeneration.seed = seed;
        for (int i = 0; i < chunkSize; i++) {
            for (int j = 0; j < chunkSize; j++) {
                noiseGeneration.GenerateNoise(ws, new Vector2Int(i * ws[0].noiseSetting.mapSize, j * ws[0].noiseSetting.mapSize));
            }
        }

    }

    public void GenerateTerrainMesh(float[,] heightMap, Vector2Int offset) {

        int mapSize = heightMap.GetLength(0);
        float topLeftX = (mapSize - 1) / -2f;
        float topLeftZ = (mapSize - 1) / 2f;
        Texture2D colorTexture = new(mapSize, mapSize);
        Color[] colors = new Color[mapSize * mapSize];
        MeshData meshData = new(mapSize, mapSize);
        int vertexIndex = 0;

        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                float h = heightMap[x, y];
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, hCurve.Evaluate(h), topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)mapSize, y / (float)mapSize);

                if (x < mapSize - 1 && y < mapSize - 1) {
                    meshData.AddTriangle(vertexIndex, vertexIndex + mapSize + 1, vertexIndex + mapSize);
                    meshData.AddTriangle(vertexIndex + mapSize + 1, vertexIndex, vertexIndex + 1);
                }
                for (int i = 0; i < _biome.terrainTypes.Length; i++) {
                    if (_biome.terrainTypes[i].h >= h) {
                        float minH = i == 0 ? 0f : _biome.terrainTypes[i - 1].h;
                        float maxH = _biome.terrainTypes[i].h;
                        float localH = Mathf.InverseLerp(minH, maxH, h);
                        colors[y * mapSize + x] = _biome.terrainTypes[i].gradient.Evaluate(localH);
                        break;

                    }
                }
                vertexIndex++;
            }
        }
        colorTexture.wrapMode = TextureWrapMode.Clamp;
        colorTexture.filterMode = FilterMode.Point;
        colorTexture.SetPixels(colors);
        colorTexture.Apply();

        GameObject meshDisplay = new("MeshDisplay", new System.Type[2] { typeof(MeshRenderer), typeof(MeshFilter) });
        meshDisplay.transform.position = new Vector3(offset.x, 0, offset.y);
        (meshDisplay.GetComponent<MeshRenderer>().material = new Material(_sourceMaterial)).mainTexture = colorTexture;
        meshDisplay.GetComponent<MeshFilter>().mesh = meshData.CreateMesh();
        meshDisplay.transform.SetParent(chunkHolder.transform);

    }
}





public class MeshData {
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight) {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c) {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh() {
        Mesh mesh = new() {
            vertices = vertices,
            triangles = triangles,
            uv = uvs
        };
        mesh.RecalculateNormals();
        return mesh;
    }*/

}