using UnityEngine;

public class PieceButton : MonoBehaviour
{
    [SerializeField] private Board _board;
    
    public void SpawnPiece(int index)
    {
        _board.SpawnPiece(index);
    }
}