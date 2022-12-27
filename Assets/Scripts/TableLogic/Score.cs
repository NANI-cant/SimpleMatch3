using System;
using PersistentProgress;

namespace TableLogic {
    public class Score {
        public event Action<int> Changed;

        private int _score;
        private readonly int _scoreForFigure;
        private readonly PersistentProgressService _persistentProgressService;

        public int CurrentScore => _score;

        public Score(int scoreForFigure, Table table, PersistentProgressService persistentProgressService) {
            _scoreForFigure = scoreForFigure;
            _persistentProgressService = persistentProgressService;
            _score = 0;

            table.MatchRemoved += OnMatchRemoved;
            table.Generated += Clear;
        }

        private void Clear() {
            _score = 0;
            Changed?.Invoke(_score);
        }

        private void OnMatchRemoved(Match match) {
            _score += match.Count * _scoreForFigure;
            _persistentProgressService.TryUpdateBestScore(CurrentScore);
            Changed?.Invoke(_score);
        }
    }
}
