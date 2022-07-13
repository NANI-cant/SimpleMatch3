using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TableLogic {
    [System.Serializable]
    public class Table {
        private Cell[,] _cellsTable;
        private Vector2Int _size;

        public Vector2Int Size => _size;

        public void Generate(Vector2Int size) {
            _size = new Vector2Int(Mathf.Max(0, Mathf.Abs(size.x)), Mathf.Max(0, Mathf.Abs(size.y)));

            _cellsTable = new Cell[_size.y, _size.x];

            for (int y = 0; y < _size.y; y++) {
                for (int x = 0; x < _size.x; x++) {
                    _cellsTable[y, x] = new Cell(this, new Vector2Int(x, y));
                }
            }
        }

        public Cell GetCell(Vector2Int position) {
            return _cellsTable[position.y, position.x];
        }

        public List<Match> FindMatches() {
            List<Match> rowMatches = FindInRows();
            List<Match> colMatches = FindInCols();

            Debug.Log("Row " + rowMatches.Count + " Col " + colMatches.Count);

            List<Match> resultMatches = new List<Match>();
            resultMatches.AddRange(rowMatches);
            resultMatches.AddRange(colMatches);

            // for (int i = 0; i < resultMatches.Count - 1; i++) {
            //     List<Match> removableMatches = new List<Match>();
            //     for (int j = i; j < resultMatches.Count; j++) {
            //         if (resultMatches[i].TryToMerge(resultMatches[j])) {
            //             removableMatches.Add(resultMatches[j]);
            //         }
            //         resultMatches.RemoveAll(match => removableMatches.Contains(match));
            //     }
            // }
            return resultMatches;

            // foreach (var rowMatch in rowMatches) {
            //     List<Match> removableMatches = new List<Match>();
            //     foreach (var colsMatch in colsMatches) {
            //         if (rowMatch.TryToMerge(colsMatch)) {
            //             removableMatches.Add(colsMatch);
            //         }
            //     }
            //     colsMatches.RemoveAll(match => removableMatches.Contains(match));
            // }

            // for (int i = 0; i < rowMatches.Count - 1; i++) {
            //     List<Match> removableMatches = new List<Match>();
            //     for (int j = i; j < rowMatches.Count; j++) {
            //         if (rowMatches[i].TryToMerge(rowMatches[j])) {
            //             removableMatches.Add(rowMatches[j]);
            //         }
            //         rowMatches.RemoveAll(match => removableMatches.Contains(match));
            //     }
            // }
        }

        private List<Match> FindInRows() {
            List<Match> matches = new List<Match>();
            for (int y = 0; y < _size.y; y++) {
                Match possibleMatch = new Match(_cellsTable[y, 0]);
                for (int x = 1; x < _size.x; x++) {
                    Cell currentCell = _cellsTable[y, x];

                    if (possibleMatch.TryToAdd(currentCell)) continue;

                    if (possibleMatch.Count >= 3) {
                        matches.Add(possibleMatch);
                    }
                    possibleMatch = new Match(currentCell);
                }
            }
            return matches;
        }

        private List<Match> FindInCols() {
            List<Match> matches = new List<Match>();
            for (int x = 0; x < _size.x; x++) {
                Match possibleMatch = new Match(_cellsTable[0, x]);
                for (int y = 1; y < _size.y; y++) {
                    Cell currentCell = _cellsTable[y, x];

                    if (possibleMatch.TryToAdd(currentCell)) continue;

                    if (possibleMatch.Count >= 3) {
                        matches.Add(possibleMatch);
                    }
                    possibleMatch = new Match(currentCell);
                }
            }
            return matches;
        }
    }
}