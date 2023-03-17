using System.Collections.Generic;
using System.Linq;
using App.Domain.Ingame.Enums;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace App.Domain
{
    public static class GameConst
    {
        private static readonly string EvalDataURL =
            "https://docs.google.com/spreadsheets/d/1EJhRG-INwxs0_TA7UGpP-g9hpLz93eLmzXCnx7VhTOg/export?format=csv&gid=0";

        private static readonly string RankDataURL =
            "https://docs.google.com/spreadsheets/d/1EJhRG-INwxs0_TA7UGpP-g9hpLz93eLmzXCnx7VhTOg/export?format=csv&gid=814094645";

        public const float NoteWidth = 1.1F;

        public const float Lifetime = 3.0f;

        private static Dictionary<JudgementType, int> _evalAndPoints = new()
        {
            { JudgementType.Miss, 0 },
            { JudgementType.Bad, 1 },
            { JudgementType.Good, 3 },
            { JudgementType.Perfect, 5 },
        };

        private static Dictionary<JudgementType, float> _evalAndThresholds = new()
        {
            { JudgementType.Perfect, 1f },
            { JudgementType.Good, 1.5f },
            { JudgementType.Bad, 2.0f },
        };

        private static Dictionary<JudgementType, string> _evalNames =
            new()
            {
                { JudgementType.Perfect, "秀" },
                { JudgementType.Good, "優" },
                { JudgementType.Bad, "可" },
                { JudgementType.Miss, "不可" },
            };

        private static List<(string, int)> _rankAndScores = new()
        {
            ("秀+", 4180),
            ("秀", 3344),
            ("優+", 2508),
            ("優", 1672),
            ("可+", 836),
            ("可", 344),
            ("不可", 0),
        };

        public static IReadOnlyDictionary<JudgementType, int> EvalAndPoints => _evalAndPoints;
        public static IReadOnlyDictionary<JudgementType, float> EvalAndThresholds => _evalAndThresholds;
        public static IReadOnlyDictionary<JudgementType, string> EvalNames => _evalNames;

        public static async void LoadMasterData()
        {
            var reqEval = UnityWebRequest.Get(EvalDataURL);
            var reqRank = UnityWebRequest.Get(RankDataURL);

            var taskEval = reqEval.SendWebRequest().ToUniTask();
            var taskRank = reqRank.SendWebRequest().ToUniTask();

            await UniTask.WhenAll(taskEval, taskRank);

            var list = new List<UnityWebRequest>() { reqEval, reqRank };
            var ok = list.All(req => req.responseCode == 200);
            if (!ok)
            {
                return;
            }

            // マスターデータを正常にDLできたときはビルトイン情報を消去する
            _evalAndPoints.Clear();
            _evalAndThresholds.Clear();
            _evalNames.Clear();
            _rankAndScores.Clear();

            ParseEvalData(reqEval.downloadHandler.text);
            ParseRankData(reqRank.downloadHandler.text);
        }

        private static void ParseEvalData(string text)
        {
            var rows = text.Split("\n").Skip(1);
            foreach (var row in rows)
            {
                var columns = row.Split(",");
                if (columns.Length != 4)
                {
                    continue;
                }

                var type = (JudgementType)int.Parse(columns[0]);
                var name = columns[1];
                var point = int.Parse(columns[2]);
                var threshold = float.Parse(columns[3]);

                _evalNames[type] = name;
                _evalAndPoints[type] = point;
                _evalAndThresholds[type] = threshold;

                Debug.Log($"{type}: {name}, {point}, {threshold}");
            }
        }

        private static void ParseRankData(string text)
        {
            var rows = text.Split("\n").Skip(1);
            foreach (var row in rows)
            {
                var columns = row.Split(",");
                if (columns.Length != 3)
                {
                    continue;
                }

                var name = columns[1];
                var score = int.Parse(columns[2]);

                _rankAndScores.Add((name, score));

                Debug.Log($"{columns[0]}: {name}, {score}");
            }

            _rankAndScores.Sort((a, b) => b.Item2 - a.Item2);
        }

        public static string GetRankText(int score)
        {
            return _rankAndScores.Find(x => score >= x.Item2).Item1;
        }
    }
}