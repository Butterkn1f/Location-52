using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Environment.PhotoReview;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class PhotoManager : Common.DesignPatterns.SingletonPersistent<PhotoManager>
{
    public const float AngleTolerance = 30;
    public const float DistTooClose = 3;
    public const float DistTooFar = 10;
    public const int MinPointsCapturedFull = 5;
    public const int MaxPhotos = 20;

    [SerializeField] TMP_Text HudPhotoCount, HudMaxPhotos;
    [SerializeField] TMP_Text TextNoFilm;
    Sequence filmYoYo;

    public List<Photo> Photos { get; private set; } = new();

    public List<ExistingPostData> PastPhotoCollection = new();

    int currIndex = -1;

    private void Start()
    {
        HudPhotoCount.text = Photos.Count.ToString("00");
        HudMaxPhotos.text = MaxPhotos.ToString("00");
        TextNoFilm.gameObject.SetActive(false);
    }

    // TODO: Call this on new day as well
    public void ClearAllPhotos()
    {
        string[] fileEntries = Directory.GetFiles(Application.dataPath);
        foreach (string filePath in fileEntries)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            if (fileInfo.Name.StartsWith("Photo"))
            {
                // Scary qwq
                Debug.Log("Deleting File: " + filePath);
                File.Delete(filePath);
            }
        }

        Photos.Clear();
        currIndex = -1;
        HudPhotoCount.text = "00";
        filmYoYo?.Kill();
        TextNoFilm.gameObject.SetActive(false);
    }

    // If save system is implemented, remove this. Keep the photos as save file
    protected override void OnApplicationQuit()
    {
        base.OnApplicationQuit();
        ClearAllPhotos();
    }

    public void AddPhoto(Texture2D texture, Renderer renderer, int pointsDetected)
    {
        Debug.Log("Points detected: " + pointsDetected + "// Required: " + MinPointsCapturedFull);
        PhotoType photoType = !renderer
            ? PhotoType.None
            : renderer.CompareTag("Monster")
            ? PhotoType.Monster
            : PhotoType.Object;

        currIndex++;
        string saveName = "/Photo" + currIndex + ".jpg";
        byte[] bytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(Application.dataPath + saveName, bytes);
        Debug.Log("File written to " + Application.dataPath + saveName);

        Photo newPhoto = photoType switch
        {
            PhotoType.Monster => new Photo()
            {
                AnomalyName = renderer.name,
                AnomalyDifficulty = Difficulty.Medium, //TODO When monster class is implemented
                TextureSaveName = saveName,
                TimeTaken = GetFakeTime(),

                Type = photoType,
                bFacingFront = GetIsFacingFront(renderer.gameObject),
                bPerfectDistance = GetIsPerfectDistance(renderer.gameObject),
                bFullView = pointsDetected >= MinPointsCapturedFull,
                bKeyActionTaken = GetIsKeyActionTaken()

            },

            PhotoType.Object => new Photo()
            {
                AnomalyName = renderer.name,
                AnomalyDifficulty = Difficulty.Easy, // ???
                TextureSaveName = saveName,
                TimeTaken = GetFakeTime(),

                Type = PhotoType.Object,
                bFacingFront = GetIsFacingFront(renderer.gameObject),
                bPerfectDistance = GetIsPerfectDistance(renderer.gameObject),
            },

            _ => new Photo()
            {
                TextureSaveName = saveName,
                TimeTaken = GetFakeTime(),
                Type = PhotoType.None
            }
        };

        Photos.Add(newPhoto);
        HudPhotoCount.text = Photos.Count.ToString("00");

        if (Photos.Count >= MaxPhotos)
        {
            ShowNoFilm();
        }
    }

    private void ShowNoFilm()
    {
        TextNoFilm.gameObject.SetActive(true);
        TextNoFilm.color = Color.white;

        filmYoYo?.Kill();
        filmYoYo = DOTween.Sequence();
        filmYoYo.Join(TextNoFilm.DOFade(0.1f, 0.5f))
            .SetLoops(-1, LoopType.Yoyo);
    }

    private System.DateTime GetFakeTime()
    {
        return new System.DateTime(
            2010, // Fake year
            System.DateTime.Now.Month, 
            System.DateTime.Now.Day, 
            19, // Hour is always 7 pm (twilight)
            System.DateTime.Now.Minute, 
            System.DateTime.Now.Second
        );
    }

    private bool GetIsFacingFront(GameObject gameObject)
    {
        var angle = Vector3.Angle(
            gameObject.transform.forward,
            gameObject.transform.position - Camera.main.transform.position
        );
        Debug.Log("Front angle difference: " + angle + "// Tolerance: " + AngleTolerance);
        return angle < AngleTolerance;
    }

    private bool GetIsPerfectDistance(GameObject gameObject)
    {
        var distance = Vector3.Distance(gameObject.transform.position, Camera.main.transform.position);
        Debug.Log("Curr Dist: " + distance + "// Max: " + DistTooFar + ", Min: " + DistTooClose);
        return distance < DistTooFar && distance > DistTooClose;
    }

    private bool GetIsKeyActionTaken()
    {
        return false; //TODO When monster is in
    }

    public bool GetIsNoFilm()
    {
        return Photos.Count >= MaxPhotos;
    }
}

[System.Serializable]
public class ExistingPostData
{
    public Image photo;
    public List<CommentPrefab> comments;
    public string Likes;
    public int CommentCount;
}
