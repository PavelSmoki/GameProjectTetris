using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TetrominoData
{
    public Tile Tile;
    public Tetromino Tetromino;

    public Vector2Int[] Cells { get; private set; }
    public Vector2Int[,] WallKicks { get; private set; }

    public void Initialize()
    {
        Cells = Data.Cells[Tetromino];
        WallKicks = Data.WallKicks[Tetromino];
    }

}