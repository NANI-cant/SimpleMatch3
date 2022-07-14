using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace TableLogic {
    public class Table {
        public event Action<Match> MatchFound;
        public event Action<Cell, Cell> CellsChanged;

        private Cell[,] _cellsTable;
        private Vector2Int _size;
        private Cell _selectedCell;

        public Vector2Int Size => _size;

        public void Generate(Vector2Int size) {
            _size = new Vector2Int(Mathf.Abs(size.x), Mathf.Abs(size.y));

            _cellsTable = new Cell[_size.y, _size.x];

            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    _cellsTable[y, x] = new Cell(this, new Vector2Int(x, y));
                }
            }

            RemoveMatches();
        }

        public bool TryChooseCell(Cell cell) {
            if (_selectedCell == null) {
                _selectedCell = cell;
                return true;
            }

            float cellsDistance = Vector2Int.Distance(cell.Position, _selectedCell.Position);
            if (cellsDistance > 1) {
                _selectedCell.UnChoose();
                _selectedCell = cell;
                return true;
            }
            else {
                Change(_selectedCell, cell);
                return false;
            }
        }

        public void UnChooseCell(Cell cell) {
            if (_selectedCell == cell) {
                _selectedCell = null;
            }
        }

        public Cell GetCell(Vector2Int position) {
            Cell cell;
            try {
                cell = _cellsTable[position.y, position.x];
            }
            catch (System.Exception) {
                return null;
            }

            return cell;
        }

        private void Change(Cell firstCell, Cell secondCell, bool withMatchFinding = true) {
            secondCell.UnChoose();
            firstCell.UnChoose();

            Figure firstFigure = firstCell.Figure;
            firstCell.SetFigure(secondCell.Figure);
            secondCell.SetFigure(firstFigure);

            CellsChanged?.Invoke(firstCell, secondCell);

            if (!withMatchFinding) return;
            Match match = FindFirstMatch();
            if (match != null) {
                MatchFound?.Invoke(match);
                RemoveMatch(match);
            }
            else {
                Change(firstCell, secondCell, false);
            }
        }

        private void RemoveMatch(Match match) {
            int maxY = int.MinValue;
            int minY = int.MaxValue;
            foreach (var cell in match.Cells) {
                cell.SetFigure(null);

                maxY = Math.Max(maxY, cell.Position.y);
                minY = Math.Min(maxY, cell.Position.y);
            }

            List<Cell> emptyCells = new List<Cell>();
            int yDelta = maxY + 1 - minY;
            foreach (var cell in match.Cells) {
                Cell replaceableCell = GetCell(new Vector2Int(cell.Position.x, cell.Position.y + yDelta));
                if (replaceableCell == null) {
                    
                }
            }
        }

        private Match FindFirstMatch() {
            List<Cell> matchedCells = new List<Cell>();
            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    Cell currentCell = GetCell(new Vector2Int(x, y));
                    Match possibleMatch = new Match(currentCell);
                    Match findedMatch = CheckCell(new Vector2Int(x, y), possibleMatch, matchedCells);
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
                foreach (var cell in match.Cells) {
                    cell.SetFigure(new Figure());
                }
                match = FindFirstMatch();
            }
        }

        private Match CheckCell(Vector2Int position, Match possibleMatch, List<Cell> matchedCells) {
            Cell currentCell = GetCell(position);

            if (matchedCells.Contains(currentCell)) return possibleMatch;
            if (!possibleMatch.TryToAdd(currentCell)) return possibleMatch;

            matchedCells.Add(currentCell);

            if (position.y > 0) {
                CheckCell(position + Vector2Int.down, possibleMatch, matchedCells);
            }
            if (position.y < _size.y - 1) {
                CheckCell(position + Vector2Int.up, possibleMatch, matchedCells);
            }
            if (position.x > 0) {
                CheckCell(position + Vector2Int.left, possibleMatch, matchedCells);
            }
            if (position.x < _size.x - 1) {
                CheckCell(position + Vector2Int.right, possibleMatch, matchedCells);
            }

            return possibleMatch;
        }
    }
}