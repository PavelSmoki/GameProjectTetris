using UnityEngine;

public class Piece : MonoBehaviour
{
    private Board _board;
    private int _rotationIndex;

    public TetrominoData Data { get; private set; }
    public Vector3Int[] Cells { get; private set; }
    public Vector3Int Position { get; private set; }

    [SerializeField] private Ai _ai;
    [SerializeField] private float _stepDelay = 1f;
    [SerializeField] private float _moveDelay = 0.1f;
    [SerializeField] private float _lockDelay = 0.5f;

    private float _stepTime;
    private float _moveTime;
    private float _lockTime;
    private bool _boardIsNull = true;
    private bool _isLocked;

    public void Initialize(Board board, Vector3Int position, TetrominoData data)
    {
        _board = board;
        if (_board != null)
        {
            _boardIsNull = false;
        }
        
        _isLocked = false;
        _rotationIndex = 0;

        Data = data;
        Position = position;

        _stepTime = Time.time + _stepDelay;
        _moveTime = Time.time + _moveDelay;
        _lockTime = 0f;

        Cells ??= new Vector3Int[data.Cells.Length];

        for (var i = 0; i < Cells.Length; i++)
        {
            Cells[i] = (Vector3Int)data.Cells[i];
        }
    }

    private void Update()
    {
        if (!_boardIsNull && !_isLocked)
        {
            _board.Clear(this);
        }

        _lockTime += Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Q))
        {
            Rotate(-1);
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Rotate(1);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            HardDrop();
        }

        if (Time.time > _moveTime)
        {
            HandleMoveInputs();
        }

        if (Time.time > _stepTime && !_boardIsNull)
        {
            Step();
        }

        if (!_boardIsNull && !_isLocked)
        {
            _board.Set(this);
        }
    }

    private void HandleMoveInputs()
    {
        if (Input.GetKey(KeyCode.S))
        {
            if (Move(Vector2Int.down))
            {
                _stepTime = Time.time + _stepDelay;
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            Move(Vector2Int.left);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            Move(Vector2Int.right);
        }
    }

    private void Step()
    {
        _stepTime = Time.time + _stepDelay;

        Move(Vector2Int.down);

        if (_lockTime >= _lockDelay)
        {
            Lock();
        }
    }

    public void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
        }

        Lock();
    }

    private void Lock()
    {
        if (!_isLocked)
        {   
            _board.Set(this);
            _board.ClearLines();
            _isLocked = true;
            _ai.LockAi();
            Debug.Log("Locked");
        }
    }

    public bool Move(Vector2Int translation)
    {
        var valid = false;

        if (_isLocked) return false;

        var newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        if (!_boardIsNull)
        {
            valid = _board.IsValidPosition(this, newPosition);
        }

        if (valid)
        {
            Position = newPosition;
            _moveTime = Time.time + _moveDelay;
            _lockTime = 0f;
        }

        return valid;
    }

    public void Rotate(int direction)
    {
        var originalRotation = _rotationIndex;

        _rotationIndex = Wrap(_rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(_rotationIndex, direction))
        {
            _rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        var matrix = global::Data.RotationMatrix;

        for (var i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];

            int x, y;

            if (Data.Tetromino is Tetromino.I or Tetromino.O)
            {
                cell.x -= 0.5f;
                cell.y -= 0.5f;
                x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
            }
            else
            {
                x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (var i = 0; i < Data.WallKicks.GetLength(1); i++)
        {
            var translation = Data.WallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        var wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, Data.WallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }

        return min + (input - min) % (max - min);
    }
}