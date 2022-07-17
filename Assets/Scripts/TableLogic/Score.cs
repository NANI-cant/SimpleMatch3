using System;

namespace TableLogic {
    public class Score {
        public event Action<int> Changed;

        private int _score;
        private int _scoreForFigure;

        public int CurrentScore => _score;

        public Score(int scoreForFigure, Table table) {
            _scoreForFigure = scoreForFigure;
            _score = 0;

            table.MatchRemoved += OnMatchRemoved;
        }

        private void OnMatchRemoved(Match match) {
            _score += match.Count * _scoreForFigure;
            Changed?.Invoke(_score);
        }
    }
}
