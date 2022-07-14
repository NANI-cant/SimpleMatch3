using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TableLogic {
    public class Match {
        private int _targetId;
        private List<Cell> _matchedCells;
        private List<Vector2Int> _positions;

        public int TargetId => _targetId;
        public int Count => _matchedCells.Count;
        public IEnumerable<Vector2Int> Positions => _positions;
        public IEnumerable<Cell> Cells => _matchedCells;

        public Match(Cell firstCell) {
            _matchedCells = new List<Cell>();
            _positions = new List<Vector2Int>();

            _targetId = firstCell.Figure.Id;
            _matchedCells.Add(firstCell);
            _positions.Add(firstCell.Position);
        }

        public bool TryToAdd(Cell cell) {
            if (cell.Figure.Id != _targetId) return false;
            if (_matchedCells.Contains(cell)) return true;

            _matchedCells.Add(cell);
            _positions.Add(cell.Position);
            return true;
        }

        public bool TryToMerge(Match match) {
            if (match.TargetId != _targetId) return false;

            foreach (var cell in match.Cells) {
                if (_matchedCells.Contains(cell)) {
                    _matchedCells.AddRange(match.Cells);
                    _positions.AddRange(match.Positions);

                    _matchedCells = _matchedCells.Distinct().ToList();
                    _positions = _positions.Distinct().ToList();
                    return true;
                }
            }

            return false;
        }

        public override string ToString() {
            string result = $"Match \n";
            result += $"count: {Count}\n";
            foreach (var position in _positions) {
                result += $"\t {position}\n";
            }
            return result;
        }
    }
}