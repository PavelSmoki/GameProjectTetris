using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    private Tilemap _tilemap;
    private Piece _activePiece;

    [SerializeField] private Ghost _ghost;
    [SerializeField] private TetrominoData[] _tetrominoes;
    [SerializeField] private Vector3Int _spawnPosition = new(-1, 8, 0);
    
    public Vector2Int BoardSize { get; } = new(10, 20);

    private RectInt Bounds
    {
        get
        {
            var position = new Vector2Int(-BoardSize.x / 2, -BoardSize.y / 2);
            return new RectInt(position, BoardSize);
        }
    }

    private void Awake()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _activePiece = GetComponent<Piece>();

        for (var i = 0; i < _tetrominoes.Length; i++)
        {
            _tetrominoes[i].Initialize();
        }
    }

    public void SpawnPiece(int index)
    {
        var data = _tetrominoes[index];
        
        _activePiece.Initialize(this, _spawnPosition, data);
        _ghost.Initialize(_activePiece);

        if (IsValidPosition(_activePiece, _spawnPosition))
        {
            Set(_activePiece);
        }
        else
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _tilemap.ClearAllTiles();
    }

    public void Set(Piece piece)
    {
        foreach (var cell in piece.Cells)
        {
            var tilePosition = cell + piece.Position;
            _tilemap.SetTile(tilePosition, piece.Data.Tile);
        }
    }

    public void Clear(Piece piece)
    {
        foreach (var cell in piece.Cells)
        {
            var tilePosition = cell + piece.Position;
            _tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        var bounds = Bounds;
        
        foreach (var cell in piece.Cells)
        {
            var tilePosition = cell + position;
            
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            
            if (_tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }

    public void ClearLines()
    {
        var bounds = Bounds;
        var row = bounds.yMin;

        while (row < bounds.yMax)
        {
            if (IsLineFull(row))
            {
                LineClear(row);
            }
            else
            {
                row++;
            }
        }
    }

    private bool IsLineFull(int row)
    {
        var bounds = Bounds;

        for (var col = bounds.xMin; col < bounds.xMax; col++)
        {
            var position = new Vector3Int(col, row, 0);
            
            if (!_tilemap.HasTile(position))
            {
                return false;
            }
        }

        return true;
    }

    private void LineClear(int row)
    {
        var bounds = Bounds;
        
        for (var col = bounds.xMin; col < bounds.xMax; col++)
        {
            var position = new Vector3Int(col, row, 0);
            _tilemap.SetTile(position, null);
        }
        
        while (row < bounds.yMax)
        {
            for (var col = bounds.xMin; col < bounds.xMax; col++)
            {
                var position = new Vector3Int(col, row + 1, 0);
                var above = _tilemap.GetTile(position);

                position = new Vector3Int(col, row, 0);
                _tilemap.SetTile(position, above);
            }

            row++;
        }
    }
}