using UnityEngine;
using UnityEngine.Tilemaps;

public class Ghost : MonoBehaviour
{
    [SerializeField] private Tile _tile;
    [SerializeField] private Board _mainBoard;

    private Piece _trackingPiece;
    private Tilemap _tilemap;
    private Vector3Int[] _cells;
    private Vector3Int _position;
    private bool _isTrackingPieceNull = true;

    public void Initialize(Piece piece)
    {
        _trackingPiece = piece;
    }

    private void Awake()
    {
        _tilemap = GetComponentInChildren<Tilemap>();
        _cells = new Vector3Int[4];
        if (_trackingPiece != null)
        {
            _isTrackingPieceNull = false;
        }
    }

    private void LateUpdate()
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        if (!_isTrackingPieceNull)
        {
            return;
        }
        
        foreach (var cell in _cells)
        {
            var tilePosition = cell + _position;
            _tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        if (_isTrackingPieceNull)
        {
            return;
        }
        
        for (var i = 0; i < _cells.Length; i++)
        {
            _cells[i] = _trackingPiece.Cells[i];
        }
    }

    private void Drop()
    {
        if (_isTrackingPieceNull)
        {
            return;
        }
        
        var position = _trackingPiece.Position;

        var current = position.y;
        var bottom = -_mainBoard.BoardSize.y / 2 - 1;

        _mainBoard.Clear(_trackingPiece);

        for (var row = current; row >= bottom; row--)
        {
            position.y = row;

            if (_mainBoard.IsValidPosition(_trackingPiece, position))
            {
                _position = position;
            }
            else
            {
                break;
            }
        }

        _mainBoard.Set(_trackingPiece);
    }

    private void Set()
    {
        if (_isTrackingPieceNull)
        {
            return;
        }
        
        foreach (var cell in _cells)
        {
            var tilePosition = cell + _position;
            _tilemap.SetTile(tilePosition, _tile);
        }
    }
}