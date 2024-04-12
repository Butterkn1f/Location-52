using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using ChatSys;
using DG.Tweening;
using TMPro;

namespace Environment.PhotoReview
{
    public class PhotoUpload : MonoBehaviour
    {
        [SerializeField] private ChatList _commentsChatList;
        private System.Random _rand;

        [SerializeField] private GameObject _commentSpawnParent;
        [SerializeField] private GameObject _commentPrefab;
        [SerializeField] private List<GameObject> _spawnedComments;

        [SerializeField] private TextMeshProUGUI _likesCount;

        [System.Serializable]
        public class ChatClass
        {
            public Grade chatGrade;
            public string chatID;
            public int minCommentsDisplayed = 1;
            public int maxCommentsDisplayed = 2;

            [Space]
            public int minLikesCount = 1;
            public int maxLikesCount = 5;
        }

        [SerializeField] private List<ChatClass> _chatClasses;
        private List<string> _comments;
        

        // Start is called before the first frame update
        void Start()
        {
            CSVReader.Instance.ReadCSV(_commentsChatList);

            _comments = new List<string>();
            _rand = new System.Random();

            RetrieveComments();

            DisplayComments();
        }

        public void RetrieveComments()
        {
            Grade currentGrade = PhotoReviewManager.Instance.PlayerResult;

            string check_ID = _chatClasses.Where(x => x.chatGrade == currentGrade).First().chatID;
            List<ChatNode> chats = _commentsChatList.GetChatNodes(check_ID);
            List<ChatNode> usernames = _commentsChatList.GetChatNodes("#USERNAME");

            int chatAmount = Random.Range(_chatClasses.Where(x => x.chatGrade == currentGrade).First().minCommentsDisplayed, _chatClasses.Where(x => x.chatGrade == currentGrade).First().maxCommentsDisplayed);

            chats = chats.OrderBy(_ => _rand.Next()).ToList();
            
            for (int i = 0; i < chatAmount; i++)
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
                GameObject tempPhoto = Instantiate(_commentPrefab, _commentSpawnParent.transform);
                tempPhoto.GetComponent<CommentPrefab>()._commentText.text = _comments[i];
                tempPhoto.GetComponent<CommentPrefab>()._userName.text = usernames[Random.Range(0, usernames.Count)].BodyText.ToString();
                tempPhoto.SetActive(false);
                _spawnedComments.Add(tempPhoto);
            }

            _likesCount.text = Random.Range(_chatClasses.Where(x => x.chatGrade == currentGrade).First().minLikesCount, _chatClasses.Where(x => x.chatGrade == currentGrade).First().maxLikesCount).ToString() + " Likes";
        }

        public void DisplayComments()
        {
            StartCoroutine(CommentSpawn());
        }
        public IEnumerator CommentSpawn()
        {
            for (int i = 0; i < _spawnedComments.Count; i++)
            {
                _spawnedComments[i].SetActive(true);
                yield return new WaitForSeconds(0.25f);
            }
        }
    }
}
