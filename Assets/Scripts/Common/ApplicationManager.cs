using ChatSys;
using Common.AudioManagement;
using Common.DataManagement;
using Common.DesignPatterns;
using Common.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Application
{
    /// <summary>
    /// This class handles the overall functions of the application. 
    /// Should not contain any specific code, just control the indiv components of the game
    /// 
    /// Also is runtime persistent because the application information needs to be kept the same throughout runtime 
    /// </summary>
    public class ApplicationManager : SingletonPersistent<ApplicationManager>
    {
        [Header("Components")]
        public SceneLoader Loader;
        public AudioManager AudioManager;
        public DataSaver DataSaver;
        public CSVReader CSVReader;
    }
}