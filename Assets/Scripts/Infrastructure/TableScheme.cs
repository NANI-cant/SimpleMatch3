using UnityEngine;

public class TableScheme {
    private bool[,] _map;
    private Vector2Int _size;

    public bool[,] Map => _map;
    public Vector2Int Size => _size;

    public TableScheme(bool[,] map, Vector2Int size) {
        _size = size;
        _map = map;
    }

    public override string ToString() {
        string text = $"Size: {_size}\n";
        for (int y = _size.y - 1; y >= 0; y--) {
            for (int x = 0; x < _size.x; x++) {
                text += _map[y, x] + " ";
            }
            text += "\n";
        }
        return text;
    }
}
