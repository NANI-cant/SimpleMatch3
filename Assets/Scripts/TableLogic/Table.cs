using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Table {
        public event Action<Match> MatchRemoved;

        private IFigureFabric _figureFabric;
        private ITableView _tableView;
        private MatchFinder _finder;
        private Helper _helper;
        private SelectingHandler _selector;

        private TableMember[,] _table;
        private Vector2Int _size;
        private TableScheme _scheme;

        public Vector2Int Size => _size;
        public Helper Helper => _helper;
        public SelectingHandler Selector => _selector;

        public Table(ITableView tableView, IFigureFabric figureFabric, TableScheme scheme) {
            _scheme = scheme;
            _tableView = tableView;
            _figureFabric = figureFabric;

            _finder = new MatchFinder(this);
            _helper = new Helper(this, _finder);
            _selector = new SelectingHandler(this);
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

        public async void Change(Figure firstFigure, Figure secondFigure, bool withMatchFinding = true) {
            secondFigure.UnChoose();
            firstFigure.UnChoose();

            Vector2Int secondPos = secondFigure.Position;
            SetFigure(firstFigure.Position, secondFigure);
            SetFigure(secondPos, firstFigure);

            await _tableView.OnFiguresReplacedAsync(new List<Figure>(new Figure[] { firstFigure, secondFigure }));

            if (!withMatchFinding) return;
            Match match = _finder.FindStrongestMatch(3);
            if (match != null) {
                while (match != null) {
                    await RemoveMatch(match);
                    match = _finder.FindStrongestMatch(3);
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
            MatchRemoved?.Invoke(match);

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
            Match match = _finder.FindStrongestMatch(3);
            if (match == null) return;

            while (match != null) {
                foreach (var figure in match.Figures) {
                    SetFigure(figure.Position, _figureFabric.GetFigure(this, figure.Position));
                }
                match = _finder.FindStrongestMatch(3);
            }
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