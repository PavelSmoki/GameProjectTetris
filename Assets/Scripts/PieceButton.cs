using UnityEngine;

public class PieceButton : MonoBehaviour
{
    [SerializeField] private Board _board;

    private bool _canSpawnNext = true;
    
    public void SpawnPiece(int index)
    {
        if (_canSpawnNext)
        {
            _board.SpawnPiece(index);
            _canSpawnNext = false;
        }
    }

    public void CanSpawnNext()
    {
        _canSpawnNext = true;
    }
}