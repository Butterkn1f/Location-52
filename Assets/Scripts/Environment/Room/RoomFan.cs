using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Environment.Room
{
    public class RoomFan : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            this.transform.DORotate(new Vector3(0, 180, 0), 4.0f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Incremental);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
