using System;
using System.IO;
using System.Linq;
using SerializerFree;
using SerializerFree.Serializers;
using UnityEngine;

namespace AKAGF.GameArchitecture.Utils.FileManagement
{
    public static class FileManager {

        public enum SERIALIZATION_TYPE { BINARY, JSON_BSON, JSON_NET, JSON_UNITY, XML }


        public static void createDirectory( string path, bool overrideDir = false ) {
            if (!Directory.Exists(path) || overrideDir) {
                Directory.CreateDirectory(path);
            }
        }

        public static bool SaveTextToFile(string text, string path) {

            bool saved = false;

            try   {
                // Create a file to write to.
                using (StreamWriter sw = File.CreateText(path)) {
                    sw.Write(text);
                }

                saved = true;
            }
            catch (Exception ex) {
                Debug.LogWarning("File writing error: " + ex.Message);
            }

            return saved;
        }

        /// <summary>
        /// Generic method used for saving data
        /// </summary>
        /// <param name="dataToStore">Data to store</param>
        /// <typeparam name="T">Typeof data to store</typeparam>
        public static void SaveToFile<T>(T dataToStore, string fileFullPath, SERIALIZATION_TYPE serializerType = SERIALIZATION_TYPE.JSON_NET) {
        
            // Get the fileformat from filename, if it is invalid, throuhg an error
            // and return the null equivalent for T 
            string fileExtension = getFileFormatFromName(fileFullPath);
            if (fileExtension == null) {
                Debug.LogError("The file " + fileFullPath + " has an invalid file format. No File saved.");
                return;
            }

            try {
                //string serializedData = JsonUtility.ToJson(dataToStore, true);
                string serializedData = Serializer.Serialize(dataToStore, GetSerializerWithIndex(serializerType));
                File.WriteAllText(fileFullPath, serializedData);
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
        public static T LoadFromFile<T>(string fileFullPath) {
            T storedData = default(T);

            // Get the fileformat from filename, if it is invalid, throuhg an error
            // and return the null equivalent for T 
            string fileExtension = getFileFormatFromName(fileFullPath);
            if (fileExtension == null) {
                Debug.LogError("The file " + fileFullPath + " has an invalid file format. No File loaded.");
                return default(T);
            }

            try {
                string serializedData = File.ReadAllText(fileFullPath);
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
        public static void RemoveData(string fileFullPath) {

            try {
                File.Delete(fileFullPath);
            }
            catch (Exception ex) {
                Debug.LogWarning(fileFullPath + " deleting error: " + ex.Message);
            }
        }


        // Method that returns the extension of a xml, json or bin 
        // file name in a fancy way :)
        public static string getFileFormatFromName(string fileName) {

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
        public static string getFileFormat(SERIALIZATION_TYPE serializationType) {
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


        public static ISerializer GetSerializerWithIndex(SERIALIZATION_TYPE serializerType) {
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


        public static ISerializer GetSerializerWithIndex(string fileExtension) {
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

        public static FileInfo[] GetFilesByExtensions(DirectoryInfo dirInfo, params string[] extensions) {
            return dirInfo.GetFiles().Where(f => extensions.Contains(f.Extension.ToLower())).ToArray();
        }

    }
}
