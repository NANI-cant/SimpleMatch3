using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Table {
        private Figure[,] _table;
        private Vector2Int _size;
        private Figure _selectedFigure;
        private ITableView _tableView;
        private IFigureFabric _figureFabric;

        public Vector2Int Size => _size;

        public Table(ITableView tableView, IFigureFabric figureFabric) {
            _tableView = tableView;
            _figureFabric = figureFabric;
        }

        public void Generate(Vector2Int size) {
            _size = new Vector2Int(Mathf.Abs(size.x), Mathf.Abs(size.y));

            _table = new Figure[_size.y, _size.x];

            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    _table[y, x] = _figureFabric.GetFigure(this, new Vector2Int(x, y));
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

        public Figure[] GetHelp() {
            List<Match> possibleMatches = new List<Match>();
            possibleMatches.AddRange(FindHorizontalMatches(targetCount: 2));
            possibleMatches.AddRange(FindVerticalMatches(targetCount: 2));

            foreach (var match in possibleMatches) {
                List<Figure> figuresInMatch = new List<Figure>(match.Figures);
                for (int i = 0; i < figuresInMatch.Count; i++) {
                    Vector2Int directionToEdgeFigure = figuresInMatch[i].Position - figuresInMatch[(i + 1) % figuresInMatch.Count].Position;
                    Vector2Int edgeFigurePosition = figuresInMatch[i].Position + directionToEdgeFigure;

                    Figure edgeFigure = GetFigure(edgeFigurePosition);
                    if (edgeFigure == null) continue;

                    Figure helpfulFigure = FindAroundById(edgeFigurePosition, match.TargetId, -directionToEdgeFigure);
                    if (helpfulFigure == null) continue;

                    return new Figure[] { edgeFigure, helpfulFigure };
                }
            }
            return new Figure[0];
        }

        private Figure FindAroundById(Vector2Int position, string id, Vector2Int dontLookInDirection) {
            for (int i = 0; i < 4; i++) {
                Vector3 lookDirection = Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90 * i)).MultiplyPoint3x4(Vector3.up);
                Vector2Int formalizedDirection = new Vector2Int(Mathf.RoundToInt(lookDirection.x), Mathf.RoundToInt(lookDirection.y));

                if (formalizedDirection == dontLookInDirection) continue;


                Figure figure = GetFigure(position + formalizedDirection);
                if (figure == null) continue;

                if (figure.Id == id) {
                    return figure;
                }
            }

            return null;
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
            Match match = FindStrongestMatch();
            if (match != null) {
                while (match != null) {
                    await RemoveMatch(match);
                    match = FindStrongestMatch();
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
            dropedFigures = dropedFigures.Distinct().ToList();
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

                    Figure newFigure = _figureFabric.GetFigure(this, position);
                    SetFigure(position, newFigure);
                    arrivedFigures.Add(newFigure);
                }
            }
            await _tableView.OnFiguresArrivedAsync(arrivedFigures);
        }

        private void RemoveMatches() {
            Match match = FindStrongestMatch();
            if (match == null) return;

            while (match != null) {
                foreach (var figure in match.Figures) {
                    SetFigure(figure.Position, _figureFabric.GetFigure(this, figure.Position));
                }
                match = FindStrongestMatch();
            }
        }

        private Match FindStrongestMatch() {
            List<Match> allMatches = FindAllMatches();
            if (allMatches.Count == 0) return null;

            Match strongestMatch = allMatches[0];
            foreach (var match in allMatches) {
                if (match.Count > strongestMatch.Count) {
                    strongestMatch = match;
                }
            }

            return strongestMatch;
        }

        private List<Match> FindAllMatches() {
            List<Match> matches = new List<Match>();

            matches.AddRange(FindVerticalMatches(targetCount: 3));
            matches.AddRange(FindHorizontalMatches(targetCount: 3));
            matches.AddRange(DetectCrossMatches(matches));

            return matches;
        }

        private List<Match> DetectCrossMatches(List<Match> matches) {
            List<Match> crossMatches = new List<Match>();

            for (int i = 0; i < matches.Count - 1; i++) {
                for (int j = i + 1; j < matches.Count; j++) {
                    Match mainMatch = new Match(matches[i]);
                    if (mainMatch.TryToMerge(matches[j])) {
                        crossMatches.Add(mainMatch);
                    }
                }
            }

            return crossMatches;
        }

        private List<Match> FindHorizontalMatches(int targetCount) {
            List<Match> matches = new List<Match>();
            for (int y = 0; y < _size.y; y++) {
                Match possibleMatch = new Match(GetFigure(new Vector2Int(0, y)));
                for (int x = 0; x < _size.x; x++) {
                    Figure figure = GetFigure(new Vector2Int(x, y));
                    if (possibleMatch.TryToAdd(figure)) continue;

                    if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);

                    possibleMatch = new Match(figure);
                }
                if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);
            }
            return matches;
        }

        private List<Match> FindVerticalMatches(int targetCount) {
            List<Match> matches = new List<Match>();
            for (int x = 0; x < _size.x; x++) {
                Match possibleMatch = new Match(GetFigure(new Vector2Int(x, 0)));
                for (int y = 0; y < _size.y; y++) {
                    Figure figure = GetFigure(new Vector2Int(x, y));
                    if (possibleMatch.TryToAdd(figure)) continue;

                    if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);

                    possibleMatch = new Match(figure);
                }
                if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);
            }
            return matches;
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