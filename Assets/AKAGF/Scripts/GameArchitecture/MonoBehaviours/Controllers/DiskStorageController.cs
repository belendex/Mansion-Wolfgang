using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AKAGF.GameArchitecture.MonoBehaviours.SceneControl;
using AKAGF.GameArchitecture.ScriptableObjects.DataPersistence;
using AKAGF.GameArchitecture.ScriptableObjects.Interaction.Conditions;
using SerializerFree;
using SerializerFree.Serializers;
using UnityEngine;

namespace AKAGF.GameArchitecture.MonoBehaviours.Controllers {

    public enum SERIALIZATION_TYPE {BINARY, JSON_BSON, JSON_NET, JSON_UNITY, XML}

    public class DiskStorageController : MonoBehaviour {

        private string SavingLocation = "saveDatas" ;               // Disk path of datasaves
        public string defaultFilePrefix = "savedGame";
        public SERIALIZATION_TYPE serializerType;
        public SceneController sceneController;
        public Inventory.Inventory inventory;
        private List<SaveData> allProjectSaveDatas;
        public SerializedStoredData bufferedGameData;       // GameData state during gameplay
        public List<SerializedStoredData> diskDataSaves 
            = new List<SerializedStoredData>();             // All game data saves found in Saving location directory
    

        private void Start() {

            // Saving folder
            SavingLocation = Application.persistentDataPath + "/" + SavingLocation;

            // Get all SaveData scriptable objects inside the project
            allProjectSaveDatas = new List<SaveData>((SaveData[])Resources.FindObjectsOfTypeAll(typeof(SaveData)));

            // Create the directory if it doesn't exist
            if (!Directory.Exists(SavingLocation))
                Directory.CreateDirectory(SavingLocation);
        
            int totalDiskSaveData = loadDiskDataSaves();
            Debug.Log("Data Saves Directory: " + SavingLocation);
            Debug.Log(totalDiskSaveData + " save data files loaded");
        
        }


        // Method for loading and serializing all game data saves stored into disk. 
        private int loadDiskDataSaves() {

            FileInfo[] savedGameDataFiles = GetFilesByExtensions(new DirectoryInfo(SavingLocation), ".json", ".bson", ".xml", ".bin");

            // No data saves found on disk
            if (savedGameDataFiles.Length == 0)
                return 0;

            // Sort by creation-time descending 
            Array.Sort(savedGameDataFiles, delegate (FileInfo f1, FileInfo f2) {
                return f2.CreationTime.CompareTo(f1.CreationTime);
            });

            for (int i = 0; i < savedGameDataFiles.Length; i++) {
                SerializedStoredData serializedStoredDataAux = LoadFromFile<SerializedStoredData>(savedGameDataFiles[i].Name);

                if (!object.Equals(serializedStoredDataAux, default(SerializedStoredData))) {
                    diskDataSaves.Add(serializedStoredDataAux);
                }
            
            }

            return savedGameDataFiles.Length;
        }


        // Method to clear the list of data saves stored in this class 
        // at runtime and optionally all data saves present on disk
        public void clearDiskDataSaves(bool deleteFromDisk = false) {
            diskDataSaves.Clear();

            if (deleteFromDisk) {
                FileInfo[] savedGameDataFiles = GetFilesByExtensions(new DirectoryInfo(SavingLocation), ".json", ".bson", ".xml", ".bin");
                for (int i = 0; i < savedGameDataFiles.Length; i++) {
                    RemoveData(savedGameDataFiles[i].Name);
                }

                Debug.Log("All disk data saves removed from disk");
            }
        }


        // Return all info from files present in saving directory
        public FileInfo[] GetFilesByExtensions(DirectoryInfo dirInfo, params string[] extensions) {
            return dirInfo.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
        }


        public void newGame(string fileName = null) {
            // Initializing bufferedGameData with all scriptable objects and inventory empty
            bufferedGameData = getGameState();

            // Set file name
            bufferedGameData.gameData.fileName = (fileName != null) ? fileName : getValidFileName();
        }


        private string getValidFileName() {

            List<FileInfo> currentFilesInDirectory = 
                new List<FileInfo>(GetFilesByExtensions(new DirectoryInfo(SavingLocation), ".json", ".bson", ".xml", ".bin"));

            if(currentFilesInDirectory.Count ==0)
                return defaultFilePrefix + 1.ToString("000");

            for (int i = 0; i < currentFilesInDirectory.Count; i++) {

                string name = defaultFilePrefix + (i + 1).ToString("000");

                // TODO format
                if (currentFilesInDirectory.Find(fileInfo => fileInfo.Name == name + ".*") == null) {
                    // There is no file with this name, so it is valid
                    return name;
                }

                if (i == currentFilesInDirectory.Count-1) {
                    return defaultFilePrefix + (i + 2).ToString("000");
                }
            }

            Debug.LogError("Something went wrong trying to get a valid name for the new file");
            return null;
        }


        public void saveGame() {
            // Buffer current gamestate with the existing filename
            bufferedGameData = getGameState(bufferedGameData.gameData.fileName);
            // Set the save date
            bufferedGameData.gameData.saveDate = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss");
            // Save to file
            SaveToFile(bufferedGameData, bufferedGameData.gameData.fileName);

            Debug.Log("File: " + bufferedGameData.gameData.fileName + " saved in directory: " + SavingLocation);
        }


        public void loadGame(int index) {
            // Set buffered game data to the disk save data state
            bufferedGameData = diskDataSaves[index];
            // Push all the data from the buffered game data
            // to all the scriptable objects of the project
            setGameState(bufferedGameData);
            // Load the next scene once all the data has been loaded
            sceneController.FadeAndLoadScene(bufferedGameData.gameData.sceneName, bufferedGameData.gameData.startingPositionName);
            // clear all the disk data saves from this object (memory performance).
            clearDiskDataSaves();
        }

        // This method gets current game state from scriptable objects
        // inside the project and assigns the references to bufferedGameState
        private SerializedStoredData getGameState(string filename = null) {

            SerializedStoredData gameState = new SerializedStoredData();
            // Scene
            gameState.gameData.sceneName = sceneController.startingSceneName;
            gameState.gameData.startingPositionName = sceneController.initialStartingPositionName;
            // Conditions
            if (AllConditions.Instance != null) {
                gameState.gameData.conditions = AllConditions.Instance.conditions;
            }
            else Debug.Log("No AllConditions Asset Found inside project resources");
        
            // Inventory
            gameState.gameData.itemsNames = inventory.getCurrentItemsNames();
            // Save datas
            gameState.gameData.gameSaveDatas = allProjectSaveDatas.ToArray();

            if (filename != null)
                gameState.gameData.fileName = filename;

            return gameState;
        }

        // This method gets the values of data parameter 
        // and assign them to scriptable objects inside the project
        private void setGameState(SerializedStoredData data) {

            // Allconditions
            if (AllConditions.Instance != null) {
                // Set the conditions to the saved game state
                AllConditions.setConditionsState(data.gameData.conditions);
            }
            else Debug.Log("No AllConditions Asset Found inside project resources");

            // Inventory
            if (inventory) {
                // Set the inventory
                inventory.setInventoryState(data.gameData.itemsNames);
            }
            else Debug.Log("No Inventory found");

            // Data Saves
            for (int i = 0; i < data.gameData.gameSaveDatas.Length; i++) {

                int saveDataIndex = allProjectSaveDatas.IndexOf(allProjectSaveDatas.Find(saveData => saveData.name == data.gameData.gameSaveDatas[i].name));

                if (saveDataIndex != -1) {
                    allProjectSaveDatas[saveDataIndex] = data.gameData.gameSaveDatas[i];
                }
                else {
                    Debug.LogError("Data Save: " + data.gameData.gameSaveDatas[i].name + " found on save data file but not present in all Game Save Datas");
                }
            }
        }


        /// <summary>
        /// Generic method used for saving data
        /// </summary>
        /// <param name="dataToStore">Data to store</param>
        /// <typeparam name="T">Typeof data to store</typeparam>
        private void SaveToFile<T>(T dataToStore, string name) {
            string fileName = SavingLocation + "/" + name + getFileFormat(serializerType);

            try {
                //string serializedData = JsonUtility.ToJson(dataToStore, true);
                string serializedData = Serializer.Serialize(dataToStore, GetSerializerWithIndex(serializerType));
                File.WriteAllText(fileName, serializedData);
            }
            catch (Exception ex) {
                Debug.LogWarning("File writing error: " + ex.Message);
            }
        }


        /// <summary>
        /// Generic method used for reading save data
        /// </summary>
        /// <returns>Read data</returns>
        /// <typeparam name="T">Typeof data to store</typeparam>
        private T LoadFromFile<T>(string name) {
            T storedData = default(T);

            // Get the fileformat from filename, if it is invalid, throuhg an error
            // and return the null equivalent for T 
            string fileExtension = getFileFormatFromName(name);
            if (fileExtension == null) {
                Debug.LogError("The file " + name + " has an invalid file format. No Saved Game loaded.");
                return default(T);
            }

            string fileName = SavingLocation + "/" + name;

            try {
                string serializedData = File.ReadAllText(fileName);
                storedData = Serializer.Deserialize<T>(serializedData, GetSerializerWithIndex(fileExtension));
            }
            catch (Exception ex) {
                Debug.LogWarning("File reading error: " + ex.Message);
            }

            return storedData;
        }


        /// <summary>
        /// This method removes all the data
        /// </summary>
        private void RemoveData(string name) {

            string fileName = SavingLocation + "/" + name + ".json";

            try {
                File.Delete(fileName);
            }
            catch (Exception ex) {
                Debug.LogWarning(fileName + " deleting error: " + ex.Message);
            }
        }


        // Method that returns the extension of a xml, json or bin 
        // file name in a fancy way :)
        private string getFileFormatFromName(string fileName) {

            // Lower case letters
            fileName = fileName.ToLower();
        
            string fileFormat = "";

            for (int i = fileName.Length - 1; i > fileName.Length - 6; i--) {

                fileFormat += fileName[i];

                if (fileFormat.Equals("nosj."))
                    return ".json";

                if (fileFormat.Equals("nosb."))
                    return ".bson";

                if (fileFormat.Equals("nib."))
                    return ".bin";

                if (fileFormat.Equals("lmx."))
                    return ".xml";
            }

            // If we get here, something went wrong or the
            // fileName has a non identified file extension.
            // Leave the responsability of handle it to
            // the calling method by returning null
            return null;
        }


        // Method that returns the corresponding string format
        // for file to save based on SERIALIZATION_TYPE param
        private string getFileFormat(SERIALIZATION_TYPE serializationType) {
            switch (serializationType) {
                default:
                case SERIALIZATION_TYPE.BINARY:
                    return ".bin";

                case SERIALIZATION_TYPE.JSON_BSON:
                    return ".bson";

                case SERIALIZATION_TYPE.JSON_NET:
                case SERIALIZATION_TYPE.JSON_UNITY:
                    return ".json";

                case SERIALIZATION_TYPE.XML:
                    return ".xml";
            }
        }


        private ISerializer GetSerializerWithIndex(SERIALIZATION_TYPE serializerType) {
            switch (serializerType) {
                default:
                case SERIALIZATION_TYPE.BINARY:
                    return new BinarySerializer();
        
                case SERIALIZATION_TYPE.JSON_BSON:
                    return new JsonDotNetBSONSerializer();
  
                case SERIALIZATION_TYPE.JSON_NET:
                    return new JsonDotNetSerializer();
    
                case SERIALIZATION_TYPE.JSON_UNITY:
                    return new UnityJsonSerializer();

                case SERIALIZATION_TYPE.XML:
                    return new XmlSerializerFree();
            }
        }


        private ISerializer GetSerializerWithIndex(string fileExtension) {
            switch (fileExtension) {
                default:
                case ".bin":
                    return new BinarySerializer();

                case ".bson":
                    return new JsonDotNetBSONSerializer();

                case ".json":
                    return new JsonDotNetSerializer();

                case ".xml":
                    return new XmlSerializerFree();
            }
        }
    }


    [Serializable]
    public class SerializedStoredData {

        [Serializable]
        public class GameData {
            public string fileName;
            public string saveDate = "Not saved";
            public string sceneName;
            public string startingPositionName;
            public Condition[] conditions;
            public string[] itemsNames;
            public SaveData[] gameSaveDatas;
        }

        public GameData gameData = new GameData();
    }
}