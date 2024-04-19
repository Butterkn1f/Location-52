using ChatSys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using UnityEngine;

namespace Environment.PhotoReview
{
    public class PastPhotos : MonoBehaviour
    {
        [SerializeField] private ChatList _commentsChatList;
        private System.Random _rand;

        [SerializeField] private GameObject ExistingPostPrefab;
        [SerializeField] private GameObject Parent;

        [System.Serializable]
        public class ChatClass
        {
            public Grade chatGrade;
            public string chatID;

            [Space]
            public int minCommentsCount = 10;
            public int maxCommentsCount = 100;

            public int minLikesCount = 10;
            public int maxLikesCount = 100;
            public string likesSuffix = "k";
        }

        [SerializeField] private List<ChatClass> _chatClasses;
        private List<string> _comments;
        private int _likesCount;

        // Start is called before the first frame update
        void Start()
        {
            CSVReader.Instance.ReadCSV(_commentsChatList);

            _rand = new System.Random();
        }

        public void GenerateNewPost()
        {
            _comments = new List<string>();

            GameObject newPost = Instantiate(ExistingPostPrefab, Parent.transform);

            // Display random image
            newPost.GetComponentInParent<ExistingPost>().DisplayImage.sprite = PhotoReviewManager.Instance._selectedImages.OrderBy(_ => _rand.Next()).ToList()[0].GetComponent<ComputerPhotoButton>().Photo.sprite;

            RetrieveComments(newPost.GetComponent<ExistingPost>());
            DisplayComments();

            AddToPhotoManager(newPost.GetComponent<ExistingPost>());

        }

        public void DisplayOldPosts(ExistingPostData postData)
        {
            GameObject newPost = Instantiate(ExistingPostPrefab, Parent.transform);

            // Display random image
            newPost.GetComponentInParent<ExistingPost>().DisplayImage.sprite = postData.photo.sprite;

            for (int i = 0; i < 3; i++)
            {
                GameObject tempPhoto = Instantiate(newPost.GetComponent<ExistingPost>().CommentPrefab, newPost.GetComponent<ExistingPost>().CommentParent.transform);
                tempPhoto.GetComponent<CommentPrefab>()._commentText = postData.comments[i]._commentText;
                tempPhoto.GetComponent<CommentPrefab>()._userName = postData.comments[i]._userName;
                newPost.GetComponent<ExistingPost>().Comments.Add(tempPhoto.GetComponent<CommentPrefab>());
            }

            StartCoroutine(RefreshCoroutine(newPost.GetComponent<ExistingPost>().CommentParent.GetComponent<RectTransform>()));

            newPost.GetComponentInParent<ExistingPost>().CommentCount.text = postData.CommentCount;
            newPost.GetComponentInParent<ExistingPost>().LikeCount.text = postData.Likes;

        }

        private void AddToPhotoManager(ExistingPost newPost)
        {
            ExistingPostData pastPhoto = new ExistingPostData();

            pastPhoto.photo = newPost.DisplayImage;
            pastPhoto.comments = newPost.Comments;

            pastPhoto.Likes = newPost.LikeCount.text;
            pastPhoto.CommentCount = newPost.CommentCount.text;

            PhotoManager.Instance.PastPhotoCollection.Add(pastPhoto);
        }

        public void RetrieveComments(ExistingPost newPost)
        {
            Grade currentGrade = PhotoReviewManager.Instance.PlayerResult;

            string check_ID = _chatClasses.Where(x => x.chatGrade == currentGrade).First().chatID;
            List<ChatNode> chats = _commentsChatList.GetChatNodes(check_ID);
            List<ChatNode> usernames = _commentsChatList.GetChatNodes("#USERNAME");

            chats = chats.OrderBy(_ => _rand.Next()).ToList();

            for (int i = 0; i < 3; i++)
            {
                // if the number of chats exceeds
                // idk how to explain but i think its pretty obvious lolll
                if (i == chats.Count)
                {
                    return;
                }

                GameObject tempPhoto = Instantiate(newPost.CommentPrefab, newPost.CommentParent.transform);
                tempPhoto.GetComponent<CommentPrefab>()._commentText.text = chats[i].BodyText;
                tempPhoto.GetComponent<CommentPrefab>()._userName.text = usernames[Random.Range(0, usernames.Count)].BodyText.ToString();
                newPost.Comments.Add(tempPhoto.GetComponent<CommentPrefab>());
            }

            StartCoroutine(RefreshCoroutine(newPost.CommentParent.GetComponent<RectTransform>()));

            newPost.GetComponentInParent<ExistingPost>().CommentCount.text = "See all comments (" + Random.Range(_chatClasses.Where(x => x.chatGrade == currentGrade).First().minCommentsCount, _chatClasses.Where(x => x.chatGrade == currentGrade).First().maxCommentsCount).ToString() + ")";

            _likesCount = Random.Range(_chatClasses.Where(x => x.chatGrade == currentGrade).First().minLikesCount, _chatClasses.Where(x => x.chatGrade == currentGrade).First().maxLikesCount);
            newPost.GetComponentInParent<ExistingPost>().LikeCount.text = _likesCount.ToString() + _chatClasses.Where(x => x.chatGrade == currentGrade).First().likesSuffix;
        }

        private IEnumerator RefreshCoroutine(RectTransform rt)
        {
            yield return new WaitForEndOfFrame();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
            yield return null;
        }

        public void DisplayComments()
        {
        }
        

        // Update is called once per frame
        void Update()
        {

        }
    }
}
