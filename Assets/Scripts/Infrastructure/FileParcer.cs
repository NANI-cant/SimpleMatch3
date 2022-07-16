using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TableLogic;
using UnityEngine;

namespace Infrastructure {
    public class FileParcer {
        private const char EMPTY = '#';
        private const char CELL = '@';

        private string _path;

        public FileParcer(string path) {
            _path = path;
        }

        public TableScheme GenerateScheme() {
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(_path, false)) {
                string anotherLine;
                while ((anotherLine = reader.ReadLine()) != null) {
                    lines.Add(anotherLine);
                }
            }

            if (!Validate(lines)) throw new System.Exception("Invalid table scheme file: " + _path);

            Vector2Int size = new Vector2Int(lines[0].Length, lines.Count);
            bool[,] map = new bool[size.y, size.x];
            for (int y = 0; y < size.y; y++) {
                for (int x = 0; x < size.x; x++) {
                    if (lines[y][x] == EMPTY) map[size.y - 1 - y, x] = false;
                    if (lines[y][x] == CELL) map[size.y - 1 - y, x] = true;
                    if (lines[y][x] != CELL && lines[y][x] != EMPTY) {
                        Debug.Log("Invalid file");
                        map[size.y - 1 - y, x] = false;
                    }
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

                string testLine = line.Replace('#', ' ').Replace('@', ' ').Trim(' ');
                if (testLine.Length > 0) return false;
            }

            return true;
        }
    }
}
