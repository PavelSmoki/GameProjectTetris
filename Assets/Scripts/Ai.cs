using UnityEngine;

public class Ai : MonoBehaviour
{
    [SerializeField] private float _behaviourDelay;

    private Piece _activePiece;
    private float _delay;
    private bool _isLocked;


    public void Initialize(Piece piece)
    {
        _activePiece = piece;
    }

    public void LockAi()
    {
    }

    private void Update()
    {
        _delay += Time.deltaTime;
        if (_behaviourDelay <= _delay)
        {
            _delay = 0;
            var random = Random.Range(0, 5);
            switch (random)
            {
                case 0:
                {
                    Debug.Log("RotatedLeft");
                    _activePiece.Rotate(-1);
                    break;
                }
                case 1:
                {
                    Debug.Log("RotatedRight");
                    _activePiece.Rotate(1);
                    break;
                }
                case 2:
                {
                    Debug.Log("MovedLeft");
                    _activePiece.Move(Vector2Int.left);
                    break;
                }
                case 3:
                {
                    Debug.Log("RotatedRight");
                    _activePiece.Move(Vector2Int.right);
                    break;
                }
                case 4:
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        Debug.Log("HardDropped");
                        _activePiece.HardDrop();
                    }

                    break;
                }
            }
        }
    }
}