using ChatSys;
using Environment.PhotoReview;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Environment.PhotoReview
{
    public class ExistingPost : MonoBehaviour
    {
        public Image DisplayImage;
        public List<CommentPrefab> Comments;

        public GameObject CommentPrefab;
        public GameObject CommentParent;

        public TextMeshProUGUI LikeCount;
        public TextMeshProUGUI CommentCount;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
