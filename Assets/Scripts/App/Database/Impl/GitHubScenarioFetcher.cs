using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using App.Domain.Scenario;
using App.Domain.Scenario.Parser;
using Cysharp.Threading.Tasks;
using Domain.Scenario;
using Domain.Scenario.Parser;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Database.Impl
{
    public class GitHubScenarioFetcher
    {
        private sealed class ContentAPIResponse
        {
            public string type;
            public string encoding;
            public int size;
            public string name;
            public string path;
            public string content;
            public string sha;
            public string url;
            public string html_url;
            public string download_url;
        }

        public const string ScenarioAccessTokenLocation = "Config/.github_token";
        
        private static readonly Lazy<string> GitHubAccessToken = new Lazy<string>(() =>
            File.ReadAllText(Path.Combine(Application.dataPath, ScenarioAccessTokenLocation)));

        private static GitHubScenarioFetcher _instance;

        public static GitHubScenarioFetcher Instance
        {
            get
            {
                if (_instance != null) return _instance;
                var database = Resources.Load<SheetScenarioDatabase>("Database/SheetScenarioDatabase");
                _instance = new GitHubScenarioFetcher(database);
                return _instance;
            }
        }

        private readonly SheetScenarioDatabase _database;
        private readonly Dictionary<string, ScenarioData> _scenarioCache = new();

        private GitHubScenarioFetcher(SheetScenarioDatabase database)
        {
            _database = database;
        }

#if UNITY_EDITOR

        [MenuItem(itemName: "Tools/Fetch scenarios")]
        public static void FetchAllScenario()
        {
            Instance.FetchAll().Forget();
        }

#endif

        public async UniTask FetchAll()
        {
            var tasks = Enumerable.Select(_database.All().Keys, Fetch).ToList();
            await UniTask.WhenAll(tasks);
        }

        public async UniTask<ScenarioData> Fetch(string scenarioId)
        {
            if (_scenarioCache.ContainsKey(scenarioId))
            {
                return _scenarioCache[scenarioId];
            }

            var ok = _database.All().TryGetValue(scenarioId, out var path);
            if (!ok)
            {
                return null;
            }

            var content = await FetchContent(path);
            if (content == null)
            {
                return null;
            }
            
            var scenarioData = TryParseScenario(content);

            _scenarioCache[scenarioId] = scenarioData;
            return scenarioData;
        }

        private static ScenarioData TryParseScenario(string csv)
        {
            var parser = new YamlScenarioParser();
            return parser.TryParseScenario(csv);
        }

        private static async UniTask<string> FetchContent(string path)
        {
            const string userName = "Oni-Men";
            const string repoName = "coco_scenario";
            var url = $"https://api.github.com/repos/{userName}/{repoName}/contents/{path}";

            var req = UnityWebRequest.Get(url);
            req.SetRequestHeader("Accept", "application/vnd.github+json");
            req.SetRequestHeader("Authorization", $"token {GitHubAccessToken.Value}");
            req.SetRequestHeader("X-GitHub-Api-Version", "2022-11-28");

            try
            {
                await req.SendWebRequest();
            }
            catch (UnityWebRequestException e)
            {
                return null;
            } 

            if (req.error != null) 
            {
                return null;
            }

            var res = JsonUtility.FromJson<ContentAPIResponse>(req.downloadHandler.text);
            var data = Convert.FromBase64String(res.content);
            var content = Encoding.UTF8.GetString(data);
            return content;
        }
    }
}