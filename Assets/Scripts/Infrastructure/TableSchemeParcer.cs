using System.Collections.Generic;
using TableLogic;
using UnityEngine;

namespace Infrastructure {
    public class TableSchemeParcer {
        private const char EMPTY = '#';
        private const char CELL = '@';

        public TableScheme GenerateScheme(string text) {
            List<string> lines = new List<string>(text.Split("\r\n"));

            Validate(lines);

            Vector2Int size = new Vector2Int(lines[0].Length, lines.Count);
            bool[,] map = new bool[size.y, size.x];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    if (lines[y][x] == EMPTY) map[size.y - 1 - y, x] = false;
                    if (lines[y][x] == CELL) map[size.y - 1 - y, x] = true;
                }
            }

            return new TableScheme(map, size);
        }

        private bool Validate(List<string> lines) {
            if (lines.Count == 0) return false;

            int targetWidth = lines[0].Length;
            if (targetWidth == 0) return false;

            foreach (var line in lines) {
                if (line.Length != targetWidth) return false;

                string testLine = line.Replace(EMPTY, ' ').Replace(CELL, ' ').Trim(' ');
                if (testLine.Length > 0) return false;
            }

            return true;
        }
    }
}
