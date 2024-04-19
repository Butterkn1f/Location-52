using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Environment.PhotoReview;

public class PhotoManager : Common.DesignPatterns.SingletonPersistent<PhotoManager>
{
    public const float AngleTolerance = 30;
    public const float DistTooClose = 3;
    public const float DistTooFar = 10;
    public const int MinPointsCapturedFull = 5;
    public List<Photo> Photos { get; private set; } = new();

    [System.Serializable]
    public class PastPhoto
    {
        public Photo photo;
        public List<CommentPrefab> comments;
        public int Likes;
    }

    // fuck it
    public List<PastPhoto> PastPhotoCollection = new();

    int currIndex = -1;

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
}
