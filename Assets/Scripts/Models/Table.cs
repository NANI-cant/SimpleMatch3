using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Table {
        private TableMember[,] _table;
        private Vector2Int _size;
        private ITableView _tableView;
        private IFigureFabric _figureFabric;
        private TableScheme _scheme;
        private Figure _selectedFigure;

        public Vector2Int Size => _size;

        public Table(ITableView tableView, IFigureFabric figureFabric, TableScheme scheme) {
            _scheme = scheme;
            _tableView = tableView;
            _figureFabric = figureFabric;
        }

        public void Generate() {
            (_table, _size) = new TableGenerator(_figureFabric, this).GenerateByScheme(_scheme);

            RemoveMatches();
        }

        public TableMember GetMember(Vector2Int position) {
            try {
                TableMember member = _table[position.y, position.x];
                return member;
            }
            catch (System.Exception) {
                return null;
            }
        }

        public Figure GetFigure(Vector2Int position) {
            try {
                TableMember member = _table[position.y, position.x];
                if (member is Void) return null;
                return (Figure)member;
            }
            catch (System.Exception) {
                return null;
            }
        }

        public void SetFigure(Vector2Int position, Figure figure) {
            try {
                _table[position.y, position.x] = figure;
                figure?.SetPosition(position);
            }
            catch (System.Exception) {
                return;
            }
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

                    Figure helpfulFigure = edgeFigure.FindAroundById(edgeFigurePosition, match.TargetId, -directionToEdgeFigure);
                    if (helpfulFigure == null) continue;

                    return new Figure[] { edgeFigure, helpfulFigure };
                }
            }
            return new Figure[0];
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
                for (int y = position.y + 1; y < _size.y; y++) {
                    TableMember anotherMember = GetMember(new Vector2Int(position.x, y));
                    if (anotherMember != null) {
                        if (anotherMember.TryFallInPosition(position)) {
                            dropedFigures.Add((Figure)anotherMember);
                        }
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
                    TableMember currentMember = GetMember(position);
                    if (currentMember != null) continue;

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
                Match possibleMatch = new Match();
                for (int x = 0; x < _size.x; x++) {
                    Figure figure = GetFigure(new Vector2Int(x, y));
                    if (possibleMatch.TryToAdd(figure)) continue;

                    if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);

                    possibleMatch = figure == null ? new Match() : new Match(figure);
                }
                if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);
            }
            return matches;
        }

        private List<Match> FindVerticalMatches(int targetCount) {
            List<Match> matches = new List<Match>();
            for (int x = 0; x < _size.x; x++) {
                Match possibleMatch = new Match();
                for (int y = 0; y < _size.y; y++) {
                    Figure figure = GetFigure(new Vector2Int(x, y));
                    if (possibleMatch.TryToAdd(figure)) continue;

                    if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);

                    possibleMatch = figure == null ? new Match() : new Match(figure);
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