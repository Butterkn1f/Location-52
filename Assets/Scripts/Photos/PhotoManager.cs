using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoManager : Common.DesignPatterns.SingletonPersistent<PhotoManager>
{
    public const float AngleTolerance = 30;
    public const float DistTooClose = 3;
    public const float DistTooFar = 10;
    public const int MinPointsCapturedFull = 5;
    public List<Photo> Photos { get; private set; } = new();
    
    public void AddPhoto(Sprite sprite, Renderer renderer, int pointsDetected)
    {
        Debug.Log("Points detected: " + pointsDetected + "// Required: " + MinPointsCapturedFull);
        PhotoType photoType = !renderer
            ? PhotoType.None
            : renderer.CompareTag("Monster")
            ? PhotoType.Monster
            : PhotoType.Object;

        Photo newPhoto = photoType switch
        {
            PhotoType.Monster => new Photo()
            {
                AnomalyName = renderer.name,
                AnomalyDifficulty = Difficulty.Medium, //TODO When monster class is implemented
                PhotoSprite = sprite,
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
                PhotoSprite = sprite,
                TimeTaken = GetFakeTime(),

                Type = PhotoType.Object,
                bFacingFront = GetIsFacingFront(renderer.gameObject),
                bPerfectDistance = GetIsPerfectDistance(renderer.gameObject),
            },

            _ => new Photo()
            {
                PhotoSprite = sprite,
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
