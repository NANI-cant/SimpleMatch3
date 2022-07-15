using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace TableLogic {
    public class Match {
        private int _targetId;
        private List<Figure> _matchedFigures;
        private List<Vector2Int> _positions;

        public int TargetId => _targetId;
        public int Count => _matchedFigures.Count;
        public IEnumerable<Vector2Int> Positions => _positions;
        public IEnumerable<Figure> Figures => _matchedFigures;

        public Match(Figure firstFigure) {
            _matchedFigures = new List<Figure>();
            _positions = new List<Vector2Int>();

            _targetId = firstFigure.Id;
            _matchedFigures.Add(firstFigure);
            _positions.Add(firstFigure.Position);
        }

        public bool TryToAdd(Figure figure) {
            if (figure == null) return false;
            if (figure.Id != _targetId) return false;
            if (_matchedFigures.Contains(figure)) return true;

            _matchedFigures.Add(figure);
            _positions.Add(figure.Position);
            return true;
        }

        public bool TryToMerge(Match match) {
            if (match.TargetId != _targetId) return false;

            foreach (var figure in match.Figures) {
                if (_matchedFigures.Contains(figure)) {
                    _matchedFigures.AddRange(match.Figures);
                    _positions.AddRange(match.Positions);

                    _matchedFigures = _matchedFigures.Distinct().ToList();
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