using System.Collections;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using SerializedData;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Networking;

namespace Persistent_Singletons
{
    public class SavingSystem : MonoBehaviour
    {
        public static SavingSystem Instance { get; private set; }

        [ShowInInspector] private string LEVELS_FILE_PATH;
        [ShowInInspector] private string LEVEL_DICT_FILE_PATH;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                Debug.Log("THERES MORE THAN 1 SavingSystem");   
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        
            LEVELS_FILE_PATH = Path.Combine(Application.persistentDataPath, "sudokuDataContainers.json");
            LEVEL_DICT_FILE_PATH = Path.Combine(Application.persistentDataPath, "levelDictionary.json");

            Debug.Log($"Level data file path: {LEVELS_FILE_PATH}");

            // Start copying data if needed
            StartCoroutine(InitializeData());
        }
    
        private IEnumerator InitializeData()
        {
            yield return CopyLevelsDataIfNotExists(); // Await until the levels data copying is completed
        }

        private IEnumerator CopyLevelsDataIfNotExists()
        {
            if(File.Exists(LEVELS_FILE_PATH)) 
            {   
                Debug.Log("The file already exists."); 
                yield break;
            }
             
            var sourcePath = Path.Combine(Application.streamingAssetsPath, "sudokuDataContainers.json");

            using UnityWebRequest request = UnityWebRequest.Get(sourcePath);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                File.WriteAllText(LEVELS_FILE_PATH, request.downloadHandler.text);
                Debug.Log("Copied sudokuDataContainers.json to persistent data path.");
            }
            else
            {
                Debug.Log("ERROR: Failed to load sudokuDataContainers.json from StreamingAssets.");
            }
        }

        public List<SudokuDataContainer> LoadLevelsData()
        {
            List<SudokuDataContainer> data = new();

            if (File.Exists(LEVELS_FILE_PATH))
            {
                string json = File.ReadAllText(LEVELS_FILE_PATH);
                data = JsonConvert.DeserializeObject<List<SudokuDataContainer>>(json);
            }

            return data;
        }

        public void SaveDataLevelsDictionary(Dictionary<int, int> levelStarsDic)
        {
            var JSON_LEVEL_DICT = JsonConvert.SerializeObject(levelStarsDic, Formatting.Indented); 
            File.WriteAllText(LEVEL_DICT_FILE_PATH, JSON_LEVEL_DICT);

            Debug.Log($"Saved level dictionary: {JSON_LEVEL_DICT}");
        }

        public Dictionary<int, int> LoadLevelDictionary()
        {
            Dictionary<int, int> data = new();

            if (File.Exists(LEVEL_DICT_FILE_PATH))
            {
                var json = File.ReadAllText(LEVEL_DICT_FILE_PATH); 
                data = JsonConvert.DeserializeObject<Dictionary<int, int>>(json);
            }

            return data;
        }

        // This function was used to save the levels data during the creation of the levels - it is not used anymore
        /*
        // public void SaveDataLevels(List<SudokuDataContainer> sudokuDataList)
        // {
        //     string JSON_LEVELS = JsonConvert.SerializeObject(sudokuDataList, Formatting.Indented);
        //     File.WriteAllText(LEVELS_FILE_PATH, JSON_LEVELS);
        // }
        */
    }
}

