using Characters.Player;
using MainGame;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.Events;
using Environment;
using DG.Tweening;

public class RoomTutorial : MonoBehaviour
{
    [SerializeField] private GameObject _cameraIntroSpawnArea;
    [SerializeField] private GameObject _cameraSpawn;
    [SerializeField] private GameObject _cameraFinal;
    [SerializeField] private GameObject _computerArticle;

    [SerializeField] private GameObject _movementInstruction;
    [SerializeField] private GameObject _interactionInstruction;
    [SerializeField] private GameObject _inventoryInstruction;
    [SerializeField] private GameObject _QAMInstruction;

    [SerializeField] private GameObject _doorWarning;

    private UnityEvent _cameraDollyOver = new UnityEvent();
    private UnityEvent _cameraSwivelOver = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(MainGameManager.Instance.GetHasFinishedTutorial());

        _movementInstruction.SetActive(false);
        _interactionInstruction.SetActive(false);
        _inventoryInstruction.SetActive(false);
        _QAMInstruction.SetActive(false);
        _doorWarning.SetActive(false);

        if (!MainGameManager.Instance.GetHasFinishedTutorial())
        {
            // Subscribe to events
            _computerArticle.SetActive(true);
            MainGameManager.Instance.TutorialManager.CurrentTutorialState.GetObservable().Subscribe(newState => { SubscribeToNewChanges(newState); });
        }
        else
        {
            _computerArticle.SetActive(false);
        }
    }

    private void SubscribeToNewChanges(TutorialState newState)
    {
        switch (newState)
        {
            case TutorialState.INTRO_CUTSCENE:
                PlayerManager.Instance.Camera.SetCameraToPosition(_cameraIntroSpawnArea);

                _cameraDollyOver.AddListener(Swivel);
                _cameraSwivelOver.AddListener(FinishIntroCutScene);

                PlayerUIManager.Instance.SetHideHUD(true);

                PlayerManager.Instance.Camera.LockCameraToPosition(_cameraSpawn, _cameraDollyOver, 5.0f);
                break;
            case TutorialState.MOVEMENT_TUTORIAL:
                Sequence seq = DOTween.Sequence();
                seq.PrependCallback(() => _movementInstruction.SetActive(true));
                seq.PrependCallback(() => _movementInstruction.GetComponent<CanvasGroup>().alpha = 0);
                seq.Append(_movementInstruction.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
                seq.AppendInterval(5.0f);
                seq.Append(_movementInstruction.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
                seq.AppendCallback(() => _movementInstruction.SetActive(false));
                seq.AppendInterval(1.0f);
                seq.AppendCallback(() => MainGameManager.Instance.TutorialManager.GetNextTutorialStage());
                break;
            case TutorialState.INTERACTION_TUTORIAL:
                Sequence seq1 = DOTween.Sequence();
                seq1.PrependCallback(() => _interactionInstruction.SetActive(true));
                seq1.PrependCallback(() => _interactionInstruction.GetComponent<CanvasGroup>().alpha = 0);
                seq1.Append(_interactionInstruction.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
                seq1.AppendInterval(5.0f);
                seq1.Append(_interactionInstruction.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
                seq1.AppendCallback(() => _interactionInstruction.SetActive(false));
                break;
            case TutorialState.INVENTORY_TUTORIAL:
                Sequence seq2 = DOTween.Sequence();
                seq2.PrependCallback(() => _inventoryInstruction.SetActive(true));
                seq2.PrependCallback(() => _inventoryInstruction.GetComponent<CanvasGroup>().alpha = 0);
                seq2.Append(_inventoryInstruction.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
                seq2.AppendInterval(5.0f);
                seq2.Append(_inventoryInstruction.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
                seq2.AppendCallback(() => _inventoryInstruction.SetActive(false));
                break;
            case TutorialState.POST_INVENTORY_TUTORIAL:
                Sequence seq3 = DOTween.Sequence();
                seq3.PrependCallback(() => _QAMInstruction.SetActive(true));
                seq3.PrependCallback(() => _QAMInstruction.GetComponent<CanvasGroup>().alpha = 0);
                seq3.Append(_QAMInstruction.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
                seq3.AppendInterval(5.0f);
                seq3.Append(_QAMInstruction.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
                seq3.AppendCallback(() => _QAMInstruction.SetActive(false));
                break;
        }
    }

    public void DisplayDoorWarning()
    {
        Sequence seq = DOTween.Sequence();

        seq.PrependCallback(() => _doorWarning.SetActive(true));
        seq.PrependCallback(() => _doorWarning.GetComponent<CanvasGroup>().alpha = 0);
        seq.Append(_doorWarning.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
        seq.AppendInterval(1.0f);
        seq.Append(_doorWarning.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
        seq.AppendCallback(() => _doorWarning.SetActive(false));
    }

    private void Swivel()
    {
        PlayerManager.Instance.Camera.LockCameraToPosition(_cameraFinal, _cameraSwivelOver, 2.0f);
    }

    public void InventoryTutorial()
    {
        MainGameManager.Instance.TutorialManager.CurrentTutorialState.SetValue(TutorialState.INVENTORY_TUTORIAL);
    }

    public void CloseInventoryTutorial()
    {
        MainGameManager.Instance.TutorialManager.CurrentTutorialState.SetValue(TutorialState.POST_INVENTORY_TUTORIAL);
    }

    private void FinishIntroCutScene()
    {
        PlayerManager.Instance.Camera.UnlockMouseCursor(false);
        PlayerManager.Instance.Movement.MovePlayer(new Vector3(_cameraFinal.transform.position.x, PlayerManager.Instance.Movement.gameObject.transform.position.y, _cameraFinal.transform.position.z));
        MainGameManager.Instance.TutorialManager.GetNextTutorialStage();
    }


}
