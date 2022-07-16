using System.Collections.Generic;
using UnityEngine;

namespace TableLogic {
    public class Helper {
        private Table _table;
        private MatchFinder _finder;

        public Helper(Table table, MatchFinder finder) {
            _table = table;
            _finder = finder;
        }

        public Figure[] GetHelp() {
            List<Match> possibleMatches = new List<Match>();
            possibleMatches.AddRange(_finder.FindAllMatches(2, false));

            foreach (var match in possibleMatches) {
                List<Figure> figuresInMatch = new List<Figure>(match.Figures);
                for (int i = 0; i < figuresInMatch.Count; i++) {
                    Vector2Int directionToEdgeFigure = figuresInMatch[i].Position - figuresInMatch[(i + 1) % figuresInMatch.Count].Position;
                    Vector2Int edgeFigurePosition = figuresInMatch[i].Position + directionToEdgeFigure;

                    Figure edgeFigure = _table.GetFigure(edgeFigurePosition);
                    if (edgeFigure == null) continue;

                    Figure helpfulFigure = edgeFigure.FindAroundById(edgeFigurePosition, match.TargetId, -directionToEdgeFigure);
                    if (helpfulFigure == null) continue;

                    return new Figure[] { edgeFigure, helpfulFigure };
                }
            }
            return new Figure[0];
        }
    }
}
