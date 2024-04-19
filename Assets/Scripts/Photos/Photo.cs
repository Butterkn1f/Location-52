using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[Serializable]
public class Photo
{
    #region Info
    public string AnomalyName = "None";
    public Difficulty AnomalyDifficulty = Difficulty.None;
    public string TextureSaveName;
    public DateTime TimeTaken;
    #endregion

    #region Grading
    public PhotoType Type;
    public bool bFacingFront = false;
    public bool bPerfectDistance = false;

    // Bonus stars only for Monsters
    public bool bFullView = false;
    public bool bKeyActionTaken = false;
    #endregion
}

public enum PhotoType
{
    None,
    Monster,
    Object
}

public enum Difficulty
{
    None,
    Easy,
    Medium,
    Hard
}