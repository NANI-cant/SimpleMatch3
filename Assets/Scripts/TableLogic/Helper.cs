using System.Collections.Generic;
using Abstraction;
using UnityEngine;

namespace TableLogic {
    public class Helper {
        private Table _table;
        private IFigureFabric _fabric;

        public Helper(Table table, IFigureFabric figureFabric) {
            _table = table;
            _fabric = figureFabric;
        }

        public Figure[] GetHelp() {
            foreach (var pair in GetFiguresByIds()) {
                Figure[] helpFigures = FindHelpInFigures(pair.Value);
                if (helpFigures.Length == 0) continue;

                return helpFigures;
            }
            return new Figure[0];
        }

        private Dictionary<string, List<Figure>> GetFiguresByIds() {
            Dictionary<string, List<Figure>> container = new Dictionary<string, List<Figure>>();
            foreach (var id in _fabric.AllIds) {
                container[id] = new List<Figure>();
            }

            for (int y = 0; y < _table.Size.y; y++) {
                for (int x = 0; x < _table.Size.x; x++) {
                    Figure figure = _table.GetFigure(new Vector2Int(x, y));
                    if (figure == null) continue;

                    container[figure.Id].Add(figure);
                }
            }

            return container;
        }

        private Figure[] FindHelpInFigures(List<Figure> figures) {
            for (int i = 0; i < figures.Count; i++) {
                for (int j = i + 1; j < figures.Count; j++) {
                    float distance = Vector2Int.Distance(figures[i].Position, figures[j].Position);

                    if (distance > 2) continue;

                    if (distance == 2) {
                        Figure middleFigure = _table.GetFigure((figures[i].Position + figures[j].Position) / 2);
                        if (middleFigure == null) continue;

                        var dontLookDirection = figures[i].Position - middleFigure.Position;
                        Figure helpFigure = middleFigure.FindAroundById(figures[i].Id, new List<Vector2Int>(new Vector2Int[] { dontLookDirection, -dontLookDirection }));
                        if (helpFigure == null) continue;

                        return new Figure[] { middleFigure, helpFigure };
                    }

                    if (distance == 1) {
                        Vector2Int directionToEdgeFigure = figures[i].Position - figures[j].Position;
                        Vector2Int edgeFigurePosition = figures[i].Position + directionToEdgeFigure;

                        Figure edgeFigure = _table.GetFigure(edgeFigurePosition);
                        if (edgeFigure != null) {
                            Figure helpfulFigure = edgeFigure.FindAroundById(figures[i].Id, new List<Vector2Int>(new Vector2Int[] { -directionToEdgeFigure }));
                            if (helpfulFigure != null) {
                                return new Figure[] { edgeFigure, helpfulFigure };
                            }
                        }

                        directionToEdgeFigure = figures[j].Position - figures[i].Position;
                        edgeFigurePosition = figures[j].Position + directionToEdgeFigure;

                        edgeFigure = _table.GetFigure(edgeFigurePosition);
                        if (edgeFigure != null) {
                            Figure helpfulFigure = edgeFigure.FindAroundById(figures[i].Id, new List<Vector2Int>(new Vector2Int[] { -directionToEdgeFigure }));
                            if (helpfulFigure != null) {
                                return new Figure[] { edgeFigure, helpfulFigure };
                            }
                        }
                    }
                }
            }
            return new Figure[0];
        }
    }
}
