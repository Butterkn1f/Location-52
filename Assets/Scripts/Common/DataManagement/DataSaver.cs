using Common.DesignPatterns;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

namespace Common.DataManagement
{
    /// <summary>
    /// This class is in charge of handling the reading and writing of the files
    /// Should not do anything with the data, just reading and writing only.
    /// </summary>
    public class DataSaver : Singleton<DataSaver>
    {
        // FileStream used for reading and writing files.
        FileStream dataStream;

        public void writeFile<T>(string filePath, string keyName, T gameData)
        {
            // Update the path once the persistent path exists.
            string persistentfilePath = UnityEngine.Application.persistentDataPath + "/" + filePath + ".json";

            // Create new AES instance.
            Aes iAes = Aes.Create();

            // Update the internal key.
            byte[] savedKey = iAes.Key;

            // Convert the byte[] into a Base64 String.
            string key = System.Convert.ToBase64String(savedKey);

            // Update the PlayerPrefs
            PlayerPrefs.SetString(keyName, key);

            // Create a FileStream for creating files.
            dataStream = new FileStream(persistentfilePath, FileMode.Create);

            // Save the new generated IV.
            byte[] inputIV = iAes.IV;

            // Write the IV to the FileStream unencrypted.
            dataStream.Write(inputIV, 0, inputIV.Length);

            // Create CryptoStream, wrapping FileStream.
            CryptoStream iStream = new CryptoStream(
                    dataStream,
                    iAes.CreateEncryptor(iAes.Key, iAes.IV),
                    CryptoStreamMode.Write);

            // Create StreamWriter, wrapping CryptoStream.
            StreamWriter sWriter = new StreamWriter(iStream);

            // Serialize the object into JSON and save string.
            string jsonString = JsonUtility.ToJson(gameData);


            // Write to the innermost stream (which will encrypt).
            sWriter.Write(jsonString);

            // Close StreamWriter.
            sWriter.Close();

            // Close CryptoStream.
            iStream.Close();

            // Close FileStream.
            dataStream.Close();
        }

        public T ReadFile<T>(string filePath, string keyName)
        {
            // Update the path once the persistent path exists.
            string persistentfilePath = UnityEngine.Application.persistentDataPath + "/" + filePath + ".json";

            // Does the file exist AND does the "key" preference exist?
            if (File.Exists(persistentfilePath) && PlayerPrefs.HasKey(keyName))
            {

                // Update key based on PlayerPrefs
                // (Convert the String into a Base64 byte[] array.)
                byte[] savedKey = System.Convert.FromBase64String(PlayerPrefs.GetString(keyName));

                // Create FileStream for opening files.
                dataStream = new FileStream(persistentfilePath, FileMode.Open);

                // Create new AES instance.
                Aes oAes = Aes.Create();

                // Create an array of correct size based on AES IV.
                byte[] outputIV = new byte[oAes.IV.Length];

                // Read the IV from the file.
                dataStream.Read(outputIV, 0, outputIV.Length);

                // Create CryptoStream, wrapping FileStream
                CryptoStream oStream = new CryptoStream(
                       dataStream,
                       oAes.CreateDecryptor(savedKey, outputIV),
                       CryptoStreamMode.Read);

                // Create a StreamReader, wrapping CryptoStream
                StreamReader reader = new StreamReader(oStream);

                // Read the entire file into a String value.
                string text = reader.ReadToEnd();

                // Always close a stream after usage.
                reader.Close();

                // Deserialize the JSON data 
                // into a pattern matching the GameData class.
                T gameData = JsonUtility.FromJson<T>(text);

                return gameData;
            }

            Debug.LogError("You are trying to access a file that does not exist.");
            return default(T);
        }

        /// <summary>
        /// Checks if a file exists given the file path
        /// </summary>
        public bool CheckIfFileExists(string filePath)
        {
            string persistentfilePath = Application.persistentDataPath + "/" + filePath + ".json";
            return (File.Exists(persistentfilePath));
        }
    }
}
