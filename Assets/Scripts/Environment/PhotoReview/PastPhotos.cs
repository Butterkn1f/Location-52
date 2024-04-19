using ChatSys;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            public int minLikesCount = 1;
            public int maxLikesCount = 5;
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

        private void AddToPhotoManager(ExistingPost newPost)
        {
            ExistingPostData pastPhoto = new ExistingPostData();

            pastPhoto.photo = newPost.DisplayImage;
            pastPhoto.comments = newPost.Comments;

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

                _comments.Add(chats[i].BodyText);
            }

            for (int i = 0; i < _comments.Count; i++)
            {
                GameObject tempPhoto = Instantiate(newPost.CommentPrefab, newPost.CommentParent.transform);
                tempPhoto.GetComponent<CommentPrefab>()._commentText.text = _comments[i];
                tempPhoto.GetComponent<CommentPrefab>()._userName.text = usernames[Random.Range(0, usernames.Count)].BodyText.ToString();
                newPost.Comments.Add(tempPhoto.GetComponent<CommentPrefab>());
            }

            _likesCount = Random.Range(_chatClasses.Where(x => x.chatGrade == currentGrade).First().minLikesCount, _chatClasses.Where(x => x.chatGrade == currentGrade).First().maxLikesCount);
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
