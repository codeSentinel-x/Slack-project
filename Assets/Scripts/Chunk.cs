using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk<T> {
    
    public const int ChunkSize = 20;
    public T[] tiles;
    public Chunk(T[] tiles) {
        this.tiles = tiles;
    }
}
