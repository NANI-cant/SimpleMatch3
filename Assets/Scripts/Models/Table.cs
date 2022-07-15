using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace TableLogic {
    public class Table {
        private Figure[,] _table;
        private Vector2Int _size;
        private Figure _selectedFigure;
        private ITableView _tableView;

        public Vector2Int Size => _size;

        public Table(ITableView tableView) {
            _tableView = tableView;
        }

        public void Generate(Vector2Int size) {
            _size = new Vector2Int(Mathf.Abs(size.x), Mathf.Abs(size.y));

            _table = new Figure[_size.y, _size.x];

            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    _table[y, x] = new Figure(this, new Vector2Int(x, y));
                }
            }

            RemoveMatches();
        }

        public Figure GetFigure(Vector2Int position) {
            Figure figure;
            try {
                figure = _table[position.y, position.x];
            }
            catch (System.Exception) {
                return null;
            }

            return figure;
        }

        private void SetFigure(Vector2Int position, Figure figure) {
            _table[position.y, position.x] = figure;
            figure?.SetPosition(position);
        }

        public bool TryChooseFigure(Figure figure) {
            if (_selectedFigure == null) {
                _selectedFigure = figure;
                return true;
            }

            float distance = Vector2Int.Distance(figure.Position, _selectedFigure.Position);
            if (distance > 1) {
                _selectedFigure.UnChoose();
                _selectedFigure = figure;
                return true;
            }
            else {
                Change(_selectedFigure, figure);
                return false;
            }
        }

        public void UnChooseFigure(Figure figure) {
            if (_selectedFigure == figure) {
                _selectedFigure = null;
            }
        }

        private async void Change(Figure firstFigure, Figure secondFigure, bool withMatchFinding = true) {
            secondFigure.UnChoose();
            firstFigure.UnChoose();

            Vector2Int secondPos = secondFigure.Position;
            SetFigure(firstFigure.Position, secondFigure);
            SetFigure(secondPos, firstFigure);

            await _tableView.OnFiguresReplacedAsync(new List<Figure>(new Figure[] { firstFigure, secondFigure }));

            if (!withMatchFinding) return;
            Match match = FindFirstMatch();
            if (match != null) {
                while (match != null) {
                    await RemoveMatch(match);
                    match = FindFirstMatch();
                }
            }
            else {
                Change(firstFigure, secondFigure, false);
            }
        }

        private async Task RemoveMatch(Match match) {
            foreach (var figure in match.Figures) {
                SetFigure(figure.Position, null);
            }
            await _tableView.OnFiguresDestroyedAsync(new List<Figure>(match.Figures));

            await DropFiguresAbove(match.Positions);
        }

        private async Task DropFiguresAbove(IEnumerable<Vector2Int> positions) {
            List<Figure> dropedFigures = new List<Figure>();
            foreach (var position in positions) {
                if (GetFigure(position) != null) continue;
                int figuresAbove = 0;
                for (int y = position.y + 1; y < _size.y; y++) {
                    Figure anotherFigure = GetFigure(new Vector2Int(position.x, y));
                    if (anotherFigure != null) {
                        SetFigure(anotherFigure.Position, null);
                        SetFigure(new Vector2Int(position.x, position.y + figuresAbove), anotherFigure);
                        figuresAbove++;
                        dropedFigures.Add(anotherFigure);
                    }
                }
            }
            dropedFigures.Distinct();
            await _tableView.OnFiguresReplacedAsync(dropedFigures);

            await FillEmpties();
        }

        private async Task FillEmpties() {
            List<Figure> arrivedFigures = new List<Figure>();
            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    Vector2Int position = new Vector2Int(x, y);
                    Figure currentFigure = GetFigure(position);
                    if (currentFigure != null) continue;

                    Figure newFigure = new Figure(this, position);
                    SetFigure(position, newFigure);
                    arrivedFigures.Add(newFigure);
                }
            }
            await _tableView.OnFiguresArrivedAsync(arrivedFigures);
        }

        private Match FindFirstMatch() {
            List<Figure> matchedFigures = new List<Figure>();
            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    Figure currentFigure = GetFigure(new Vector2Int(x, y));
                    if (currentFigure == null) continue;

                    Match possibleMatch = new Match(currentFigure);
                    Match findedMatch = CheckFigure(new Vector2Int(x, y), possibleMatch, matchedFigures);
                    if (findedMatch.Count > 2) {
                        return findedMatch;
                    }
                }
            }
            return null;
        }

        private void RemoveMatches() {
            Match match = FindFirstMatch();
            if (match == null) return;

            while (match != null) {
                foreach (var figure in match.Figures) {
                    SetFigure(figure.Position, new Figure(this, figure.Position));
                }
                match = FindFirstMatch();
            }
        }

        private Match CheckFigure(Vector2Int position, Match possibleMatch, List<Figure> matchedFigures) {
            Figure currentFigure = GetFigure(position);

            if (matchedFigures.Contains(currentFigure)) return possibleMatch;
            if (!possibleMatch.TryToAdd(currentFigure)) return possibleMatch;

            matchedFigures.Add(currentFigure);

            if (position.y > 0) {
                CheckFigure(position + Vector2Int.down, possibleMatch, matchedFigures);
            }
            if (position.y < _size.y - 1) {
                CheckFigure(position + Vector2Int.up, possibleMatch, matchedFigures);
            }
            if (position.x > 0) {
                CheckFigure(position + Vector2Int.left, possibleMatch, matchedFigures);
            }
            if (position.x < _size.x - 1) {
                CheckFigure(position + Vector2Int.right, possibleMatch, matchedFigures);
            }

            return possibleMatch;
        }

        public override string ToString() {
            string str = "Table\n";
            str += "\tSize: " + _size + "\n";
            for (int y = _size.y - 1; y >= 0; y--) {
                for (int x = 0; x < _size.x; x++) {
                    var figure = GetFigure(new Vector2Int(x, y));
                    if (figure == null) {
                        str += "n ";
                    }
                    else {
                        str += figure.Id + " ";
                    }
                }
                str += "\n";
            }

            return str;
        }
    }
}