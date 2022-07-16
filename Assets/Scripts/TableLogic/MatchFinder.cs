using System.Collections.Generic;
using UnityEngine;

namespace TableLogic {
    public class MatchFinder {
        private Table _table;

        public MatchFinder(Table table) {
            _table = table;
        }

        public Match FindStrongestMatch(int targetCount, bool withCrossing = true) {
            List<Match> allMatches = FindAllMatches(targetCount, withCrossing);
            if (allMatches.Count == 0) return null;

            Match strongestMatch = allMatches[0];
            foreach (var match in allMatches) {
                if (match.Count > strongestMatch.Count) {
                    strongestMatch = match;
                }
            }

            return strongestMatch;
        }

        public List<Match> FindAllMatches(int targetCount, bool withCrossing = true) {
            targetCount = Mathf.Max(0, targetCount);
            List<Match> matches = new List<Match>();

            matches.AddRange(FindVerticalMatches(targetCount));
            matches.AddRange(FindHorizontalMatches(targetCount));
            if (!withCrossing) return matches;

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
            for (int y = 0; y < _table.Size.y; y++) {
                Match possibleMatch = new Match();
                for (int x = 0; x < _table.Size.x; x++) {
                    Figure figure = _table.GetFigure(new Vector2Int(x, y));
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
            for (int x = 0; x < _table.Size.x; x++) {
                Match possibleMatch = new Match();
                for (int y = 0; y < _table.Size.y; y++) {
                    Figure figure = _table.GetFigure(new Vector2Int(x, y));
                    if (possibleMatch.TryToAdd(figure)) continue;

                    if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);

                    possibleMatch = figure == null ? new Match() : new Match(figure);
                }
                if (possibleMatch.Count >= targetCount) matches.Add(possibleMatch);
            }
            return matches;
        }
    }
}
