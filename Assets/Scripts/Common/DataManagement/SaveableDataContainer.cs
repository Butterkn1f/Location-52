using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Common.DataManagement
{
    /// <summary>
    /// This class saves primitive game data (i.e. float, int)
    /// </summary>

    [CreateAssetMenu(menuName = "Custom app data containers/Saveable Data Container", fileName = "New Saveable Data Container", order = 15)]
    public class SaveableDataContainer : ScriptableObject
    {
        [SerializeField] private string _settingsDataFileName = "";
        [SerializeField] private string _settingsDataKeyName = "";

        public List<SaveableDataBaseAsset> GameData;

        public void InitialiseDataContainer()
        {
            // Clear the data so we don't get two instances of the same data (nightmare.)
            GameData.Clear();
            DevUtils.AssertTrue(_settingsDataFileName == "", "File Name is empty. Please include a valid file name.");
            DevUtils.AssertTrue(_settingsDataKeyName == "", "File Name is empty. Please include a valid key name.");
        }

        public ReactiveProp<float> GetSettingsFloatByID(string settingsUID)
        {
            if (GameData.Where(x => x.SettingID == settingsUID).Count() == 0) { return null; }

            if (GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault().Type == SaveableDataType.FLOAT)
            {
                SaveableFloatAsset floatSettings = (SaveableFloatAsset)GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault();
                return floatSettings.Value;
            }

            return null;
        }

        public ReactiveProp<int> GetSettingsIntByID(string settingsUID)
        {
            if (GameData.Where(x => x.SettingID == settingsUID).Count() == 0) { return null; }

            if (GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault().Type == SaveableDataType.INT)
            {
                SaveableIntAsset intSettings = (SaveableIntAsset)GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault();
                return intSettings.Value;
            }

            return null;
        }

        public ReactiveProp<bool> GetSettingsBoolByID(string settingsUID)
        {
            if (GameData.Where(x => x.SettingID == settingsUID).Count() == 0) { return null; }

            if (GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault().Type == SaveableDataType.BOOL)
            {
                SaveableBoolAsset boolSettings = (SaveableBoolAsset)GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault();
                return boolSettings.Value;
            }

            return null;
        }

        public ReactiveProp<string> GetSettingsStringByID(string settingsUID)
        {
            if (GameData.Where(x => x.SettingID == settingsUID).Count() == 0) { return null; }

            if (GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault().Type == SaveableDataType.STRING)
            {
                SaveableStringAsset stringSettings = (SaveableStringAsset)GameData.Where(x => x.SettingID == settingsUID).FirstOrDefault();
                return stringSettings.Value;
            }

            return null;
        }

        public SaveableDataType GetTypeByID(string ID)
        {
            return GameData.Where(x => x.SettingID == ID).FirstOrDefault().Type;
        }

        public void SaveData()
        {
            GameSaveDataContainer saveData = new GameSaveDataContainer();

            for (int i = 0; i < GameData.Count; i++)
            {
                switch (GameData[i].Type)
                {
                    case SaveableDataType.INT:
                        saveData.IntData.Add((SaveableIntAsset)GameData[i]);
                        break;

                    case SaveableDataType.FLOAT:
                        saveData.FloatData.Add((SaveableFloatAsset)GameData[i]);
                        break;

                    case SaveableDataType.BOOL:
                        saveData.BoolData.Add((SaveableBoolAsset)GameData[i]);
                        break;

                    case SaveableDataType.STRING:
                        saveData.StringData.Add((SaveableStringAsset)GameData[i]);
                        break;
                }
            }

            DataSaver.Instance.writeFile<GameSaveDataContainer>(_settingsDataFileName, _settingsDataKeyName, saveData);
        }

        // Load the List !!
        public void LoadData()
        {
            if (!DataSaver.Instance.CheckIfFileExists(_settingsDataFileName)) { return; }

            GameSaveDataContainer saveData = DataSaver.Instance.ReadFile<GameSaveDataContainer>(_settingsDataFileName, _settingsDataKeyName);

            if (saveData == null) { return; }

            foreach (var IntSetting in saveData.IntData)
            {
                GetSettingsIntByID(IntSetting.SettingID).SetValue(IntSetting.ValueReadable);
            }

            foreach (var FloatSetting in saveData.FloatData)
            {
                GetSettingsFloatByID(FloatSetting.SettingID).SetValue(FloatSetting.ValueReadable);
            }

            foreach (var BoolSetting in saveData.BoolData)
            {
                GetSettingsBoolByID(BoolSetting.SettingID).SetValue(BoolSetting.ValueReadable);
            }

            foreach (var StringSetting in saveData.StringData)
            {
                GetSettingsStringByID(StringSetting.SettingID).SetValue(StringSetting.ValueReadable);
            }
        }

        // In case you need a clean wipe of the data. Otherwise, nevermind.
        public void RestoreDefaultData()
        {
            foreach (var setting in GameData)
            {
                switch (setting.Type)
                {
                    case SaveableDataType.INT:
                        GetSettingsIntByID(setting.SettingID).SetValue(((SaveableIntAsset)setting).DefaultValue);
                        break;

                    case SaveableDataType.FLOAT:
                        GetSettingsFloatByID(setting.SettingID).SetValue(((SaveableFloatAsset)setting).DefaultValue);
                        break;

                    case SaveableDataType.BOOL:
                        GetSettingsBoolByID(setting.SettingID).SetValue(((SaveableBoolAsset)setting).DefaultValue);
                        break;
                }
            }

            SaveData();
        }
    }

    [System.Serializable]
    public class GameSaveDataContainer
    {
        public List<SaveableFloatAsset> FloatData = new List<SaveableFloatAsset>();
        public List<SaveableBoolAsset> BoolData = new List<SaveableBoolAsset>();
        public List<SaveableIntAsset> IntData = new List<SaveableIntAsset>();
        public List<SaveableStringAsset> StringData = new List<SaveableStringAsset>();
    }

    // Enumerator to indicate what data type it is
    public enum SaveableDataType
    {
        FLOAT,
        INT,
        BOOL,
        STRING,
    }

    // Base class for settings

    [System.Serializable]
    public class SaveableDataBaseAsset
    {
        public string SettingID;

        // For typecasting tbvh
        public SaveableDataType Type;
    }

    [System.Serializable]
    public class SaveableFloatAsset : SaveableDataBaseAsset
    {
        public ReactiveProp<float> Value = new ReactiveProp<float>();
        public float ValueReadable;
        public float DefaultValue = 0.5f;

        private SaveableFloatAsset() { }
        public SaveableFloatAsset(string SettingID, float DefaultValue, float maxValue = float.MaxValue, float minValue = float.MinValue)
        {
            this.SettingID = SettingID;
            this.Type = SaveableDataType.FLOAT;

            this.DefaultValue = DefaultValue;
            this.ValueReadable = DefaultValue;
            this.Value.SetValue(DefaultValue);
            this.Value.GetObservable().Subscribe(newVal =>
            {
                ValueReadable = newVal;
            });
        }
    }

    [System.Serializable]
    public class SaveableIntAsset : SaveableDataBaseAsset
    {
        public ReactiveProp<int> Value = new ReactiveProp<int>();
        public int ValueReadable;
        public int DefaultValue = 5;

        private SaveableIntAsset() { }
        public SaveableIntAsset(string SettingID, int DefaultValue, int maxValue = int.MaxValue, int minValue = int.MinValue)
        {
            this.SettingID = SettingID;
            this.Type = SaveableDataType.INT;

            this.DefaultValue = DefaultValue;
            this.ValueReadable = DefaultValue;
            this.Value.SetValue(DefaultValue);
            this.Value.GetObservable().Subscribe(newVal =>
            {
                newVal = Mathf.Clamp(newVal, minValue, maxValue);
                ValueReadable = newVal;
            });
        }
    }

    [System.Serializable]
    public class SaveableBoolAsset : SaveableDataBaseAsset
    {
        public ReactiveProp<bool> Value = new ReactiveProp<bool>();
        public bool ValueReadable;
        public bool DefaultValue = false;

        private SaveableBoolAsset() { }
        public SaveableBoolAsset(string SettingID, bool DefaultValue)
        {
            this.SettingID = SettingID;
            this.Type = SaveableDataType.BOOL;

            this.DefaultValue = DefaultValue;
            this.ValueReadable = DefaultValue;
            this.Value.SetValue(DefaultValue);
            this.Value.GetObservable().Subscribe(newVal =>
            {
                ValueReadable = newVal;
            });
        }
    }

    [System.Serializable]
    public class SaveableStringAsset : SaveableDataBaseAsset
    {
        public ReactiveProp<string> Value = new ReactiveProp<string>();
        public string ValueReadable;
        public string DefaultValue = "";

        private SaveableStringAsset() { }
        public SaveableStringAsset(string SettingID, string DefaultValue)
        {
            this.SettingID = SettingID;
            this.Type = SaveableDataType.STRING;

            this.DefaultValue = DefaultValue;
            this.ValueReadable = DefaultValue;
            this.Value.SetValue(DefaultValue);
            this.Value.GetObservable().Subscribe(newVal =>
            {
                ValueReadable = newVal;
            });
        }
    }
}