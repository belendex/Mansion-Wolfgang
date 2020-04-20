using System.Collections.Generic;
using System.Linq;
using AKAGF.GameArchitecture.Utils.FileManagement;
using UnityEditor;
using UnityEngine;

namespace AKAeditor {

    /// <summary>
    ///  Helper  class with commonly used functions to work with Unity Scriptable Objects.
    /// </summary>
    public static class ScriptableObjectUtility {

        /// <summary>
        /// Generic method to nest a child Scriptable Object to a parent Scriptable Object inside Project Assets folder 
        /// and in the parent's internal array of children (objectArray). <para> It also receives a string for the undo record log
        /// for the new child object. </para>
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <param name="parent"> The parent Scriptable object to which child object is attached in assets folder.</param>
        /// <param name="child"> The object that is attached to the parent object and added to its list of similar objects.</param>
        /// <param name="objectArray"> Array of objects where the child object will be added.</param>
        /// <param name="recordName">The title of the action to appear in the undo history for the child object. </param>
        public static void AddScriptableObject<T>(ScriptableObject parent, ref T child, ref T[] objectArray, string recordName) where T : ScriptableObject {

            // Check if the parent object has a valid reference
            if (parent == null) {
                Debug.LogError(parent.name + " has not been created yet.");
                return;
            }

            // Record all operations on the scriptable object so they can be undone.
            Undo.RecordObject(child, recordName);

            // Attach the child object to the parent asset.
            AssetDatabase.AddObjectToAsset(child, parent);

            // Import the asset so it is recognised as a joined asset.
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(child));

            // Add the child object to the parent array.
            ArrayUtility.Add(ref objectArray, child);

            // Mark parent asset as dirty so the editor knows to save changes to it when a project save happens.
            EditorUtility.SetDirty(parent);
        }

        /// <summary>
        /// Generic method to nest a child Scriptable Object to a parent Scriptable Object inside Project Assets folder 
        /// and in the parent's internal List of children (objectList). <para> It also receives a string for the undo record log
        /// for the new child object. </para>
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <param name="parent"> The parent Scriptable object to which child object is attached in assets folder.</param>
        /// <param name="child"> The object that is attached to the parent object and added to its list of similar objects.</param>
        /// <param name="objectList"> Generic list of objects where the child object will be added.</param>
        /// <param name="recordName">The title of the action to appear in the undo history for the child object. </param>
        public static void AddScriptableObject<T>(ScriptableObject parent, ref T child, ref List<T> objectList, string recordName) where T : ScriptableObject {

            // Check if the parent object has a valid reference
            if (parent == null) {
                Debug.LogError(parent.name + " has not been created yet.");
                return;
            }

            // Record all operations on the scriptable object so they can be undone.
            Undo.RecordObject(child, recordName);

            // Attach the child object to the parent asset.
            AssetDatabase.AddObjectToAsset(child, parent);

            // Import the asset so it is recognised as a joined asset.
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(child));

            // Add the child object to the parent's list.
            objectList.Add(child);

            // Mark parent asset as dirty so the editor knows to save changes to it when a project save happens.
            EditorUtility.SetDirty(parent);
        }

        /// <summary>
        /// Generic method to remove a child Scriptable Object from a parent Scriptable Object inside Assets 
        /// folder and from the parent's internal array of children (objectList).
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <param name="parent"> The parent Scriptable object to which child object is removed in assets folder.</param>
        /// <param name="childToRemove"> The object that is removed from the parent object and from its list of similar objects.</param>
        /// <param name="objectList"> Array of objects where the child object will be removed.</param>
        public static void RemoveScriptableObject<T>(ScriptableObject parent, ref T childToRemove, ref T[] objectList) where T : ScriptableObject {
            // If there isn't a parent asset, do nothing.
            if (parent == null) {
                Debug.LogError(parent.name + " has not been created yet.");
                return;
            }

            // Record all operations on the parent asset so they can be undone.
            // Undo.RecordObject(parent, recordName);

            // Remove the specified child from the parent object list.
            ArrayUtility.Remove(ref objectList, childToRemove);

            // Destroy the child, including it's asset and save the assets to recognise the change.
            GameObject.DestroyImmediate(childToRemove, true);
            AssetDatabase.SaveAssets();

            // Mark the parent asset as dirty so the editor knows to save changes to it when a project save happens.
            EditorUtility.SetDirty(parent);
        }

        /// <summary>
        /// Generic method to get a Scriptable object by asset name from an array of similar objects
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <param name="name"> The name of the scriptable object in the asset folder to search.</param>
        /// <param name="objectList"> The list where the scriptable object name is searched.</param>
        /// <returns> The Scriptable Object in case it exists within the objectList or null in case not.</returns>
        public static T GetScriptableObjectByName<T>(string name, T[] objectList) where T : ScriptableObject {

            ScriptableObject obj;

            for (int i = 0; i < objectList.Length; i++) {
                obj = objectList[i];
                if (obj.name.Equals(name)) {
                    return objectList[i];
                }
            }

            return default(T);
        }

        /// <summary>
        /// Generic method that tries to get a Scriptable Object by the given index inside an array of T objects.
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <param name="index"> The index of the array objectList where the object should be</param>
        /// <param name="objectList"> The array where the scriptable object index is searched.</param>
        /// <returns> 
        /// 1.Null when the objectList array or the first element of this array are null.
        /// 2.The first element of the objectList if the index given is beyond the length of the array.
        /// 3.The T element at the given index inside objectList.
        /// </returns>
        public static T TryGetScriptableObjectAt<T>(int index, T[] objectList) {

            // If it doesn't exist or there are null elements, return null.
            if (objectList == null || objectList[0] == null)
                return default(T);

            // If the given index is beyond the length of the array return the first element.
            if (index >= objectList.Length)
                return objectList[0];

            // Otherwise return the object at the given index.
            return objectList[index];
        }

        /// <summary>
        /// Generic method to get the length of an array of Scriptables Objects of type T.
        /// </summary>
        /// <typeparam name="T">The class that derives from ScriptableObject.</typeparam>
        /// <param name="objectList">The object list which the length should be returned.</param>
        /// <returns>
        /// The length of the array objectList or zero when the objectList is null.
        /// </returns>
        public static int TryGetScriptablesArrayLength<T>(T[] objectList) {
            // If there is no object list, return a length of 0.
            if (objectList == null)
                return 0;

            // Otherwise return the length of the array.
            return objectList.Length;
        }

        /// <summary>
        /// Generic method to create singleton instance of a ScriptableObject of type T
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <param name="ScriptableType"> The instance of the non created singleton</param>
        /// <param name="path">Optional: The Assets path where the asset singleton will be created</param>
        /// <returns> The singleton instance of the ScriptableObject of type T.</returns>
        public static T CreateSingletonScriptableObject<T>(T ScriptableType, string path = null) where T : ScriptableObject {

            // If there's already an  asset of this type, do nothing.
            if (ScriptableType != null) {
                Debug.LogWarning(typeof(T).Name + " has already been created.");
                Selection.activeObject = ScriptableType;
                return ScriptableType;
            }

            // Create the directory only if it doesn't exist yet
            if (path != null)
                FileManager.createDirectory(path);

            // Create an instance of the All object and make an asset for it.
            T instance = ScriptableObject.CreateInstance<T>();

            // Record all operations on the scriptable object so they can be undone.
            Undo.RecordObject(instance, typeof(T).Name + " created.");

            AssetDatabase.CreateAsset(instance, path +  typeof(T).Name + ".asset");
            AddScriptableObjectToPreloadedAssets(instance);

            EditorUtility.SetDirty(instance);

            Selection.activeObject = instance;

            return instance;
        }

        /// <summary>
        /// Generic method to create single instances of Scriptables Objects of Type T.
        /// </summary>
        /// <typeparam name="T">The class that derives from ScriptableObject.</typeparam>
        /// <param name="path">The path where the object will be created</param>
        /// <param name="name">The proposed name for the new Object.</param>
        /// <returns> The new created instance of type T sriptable object.</returns>
        public static T createSingleScriptableObject<T>(string path, string name = null) where T : ScriptableObject {

            FileManager.createDirectory(path);

            // Create the logic instance
            T newScriptable = ScriptableObject.CreateInstance<T>();

            // Record all operations on the scriptable object so they can be undone.
            Undo.RecordObject(newScriptable, typeof(T).Name + " created.");

            // Set the new name. 
            string newName = name == null ? typeof(T).Name: name ;

            // Repeated name, check for a valid one
            if (GetAllInstances<T>(newName).Length > 0) {
                for (int i = 0 ;  ; i++) {
                    if (GetAllInstances<T>(newName+"_"+(i+1)).Length == 0) {
                        newName += "_" + (i + 1);
                        break;
                    }
                }
            }

            // Create the physical instance
            AssetDatabase.CreateAsset(newScriptable, path + newName + ".asset");

            // Mark object as dirty, so editor knows to save the asset state
            EditorUtility.SetDirty(newScriptable);

            // Aware the developer of the asset creation name and path
            Debug.Log(newName + " created in " + path);

            // Focus the project window to the object just created.
            Selection.activeObject = newScriptable;

            return newScriptable;
        }

        /// <summary>
        /// Add an Scriptable Object to Unity Player Settings Preloaded Assets. 
        /// </summary>
        /// <param name="objectToAdd"> The Scriptable Object to be added to the Preloaded Assets List.</param>
        public static void AddScriptableObjectToPreloadedAssets(ScriptableObject objectToAdd) {
            
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();

            // It the asset is not present in the preloaded Assets list yet, add it
            if (!preloadedAssets.Exists(x => x.name.Equals(objectToAdd.name))) {
                preloadedAssets.Add(objectToAdd);
                PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
            }
        }

        /// <summary>
        /// Remove an Scriptable Object to Unity Player Settings Preloaded Assets. 
        /// </summary>
        /// <param name="objectToRemove">The Scriptable Object to be removed from the Preloaded Assets List.</param>
        public static void RemoveScriptableObjectFromPreloadedAssets(ScriptableObject objectToRemove) {
            // Remove the config asset from the build
            var preloadedAssets = PlayerSettings.GetPreloadedAssets().ToList();
            preloadedAssets.Remove(objectToRemove);
            PlayerSettings.SetPreloadedAssets(preloadedAssets.ToArray());
        }

        /// <summary>
        /// Get all instances, loaded or not, of type T ScriptableObjects within the project Assets folder.
        /// </summary>
        /// <typeparam name="T"> The class that derives from ScriptableObject.</typeparam>
        /// <returns>Array of founded ScriptableObjects of type T within the project.</returns>
        public static T[] GetAllInstances<T>(string assetName = "") where T : ScriptableObject {
            //FindAssets uses tags check documentation for more info
            List<string> guids = new List<string>(AssetDatabase.FindAssets(assetName + " t:" + typeof(T).Name));
            //The list to store all founded instances of type T
            List<T> instances = new List<T>(guids.Count);

            // Subassets have the same guid as the parent, so remove duplicates
            guids = guids.Distinct().ToList();

            string path = "";

            for (int i = 0; i < guids.Count; i++) {
                path = AssetDatabase.GUIDToAssetPath(guids[i]);

                // Load all the assets at the given path
                Object[] allObjectsAtPath = AssetDatabase.LoadAllAssetsAtPath(path);

                // Store only the assets of the corresponding type T
                for (int j = 0; j < allObjectsAtPath.Length; j++) {
                    if (allObjectsAtPath[j] is T) {
                        instances.Add(allObjectsAtPath[j] as T);
                    }
                }
            }

            return instances.ToArray();
        }


        /// <summary>
        /// Get all instances names, loaded or not, of type T ScriptableObjects within the project Assets folder.
        /// </summary>
        /// <returns>Array of founded ScriptableObjects names of type T within the project.</returns>
        public static string[] getAllInstancesNames<T>() where T : ScriptableObject {

            T[] a = GetAllInstances<T>();
            string[] names = new string[a.Length];

            for (int i = 0; i < names.Length; i++) {
                names[i] = a[i].name;
            }

            return names;
        }

    }
}
