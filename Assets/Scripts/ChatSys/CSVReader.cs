using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Common.DesignPatterns;

namespace ChatSys
{
    // This class reads different CSV files
    public class CSVReader : Singleton<CSVReader>
    {
        // Function to read CSV files
        public void ReadCSV(ChatList chatList)
        {
            //string filePath;
            //filePath = Application.persistentDataPath + _chatFileName;
            //// Read and write files from the filepath
            //StreamReader reader = new StreamReader(filePath, true);

            // using textAsset implementation
            TextAsset CSVFile = Resources.Load<TextAsset>(chatList.ChatFileName);
            if (CSVFile != null)
            {
                StreamReader reader = new StreamReader(new MemoryStream(CSVFile.bytes));
                bool EOF = false;

                while (!EOF)
                {
                    string data_string = reader.ReadLine();
                    if (data_string == null)
                    {
                        EOF = true;
                        break;
                    }

                    // store in a chat node
                    ChatNode temp_chatNode = new ChatNode();
                    string[] dataValues = data_string.Split(",");

                    if (dataValues[1] == null || dataValues[2] == null || dataValues[3] == null)
                    {
                        Debug.LogWarning("Incorrect formatting of CSV files, certain fields missing. Check your formatting of the file.");
                        continue;
                    }


                    if (dataValues[1] != null)
                    {
                        // Get ID
                        // Check if there is a proper ID (starting with pound sign)
                        if (dataValues[1][0] == '#')
                        {
                            temp_chatNode.ID = dataValues[1];
                        }
                    }
                    else
                    {
                        // invalid ID, therefore there is no chat here return 
                        return;
                    }

                    if (dataValues[2] != null)
                    {
                        // Get sprite mood
                        temp_chatNode.Mood = (int.Parse(dataValues[2]));
                    }

                    if (dataValues[3] != null)
                    {
                        // Get main body
                        dataValues[3] = dataValues[3].Replace("//", ",");
                        temp_chatNode.BodyText = dataValues[3];
                    }

                    if (temp_chatNode != null)
                    {
                        chatList.ChatNodeList.Add(temp_chatNode);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Text file given not found");
            }
        }
    }

}