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

    [SerializeField] private GameObject _movementInstruction;
    [SerializeField] private GameObject _interactionInstruction;

    private UnityEvent _cameraDollyOver = new UnityEvent();
    private UnityEvent _cameraSwivelOver = new UnityEvent();

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(MainGameManager.Instance.GetHasFinishedTutorial());

        _movementInstruction.SetActive(false);
        _interactionInstruction.SetActive(false);

        if (!MainGameManager.Instance.GetHasFinishedTutorial())
        {
            // Subscribe to events
            MainGameManager.Instance.TutorialManager.CurrentTutorialState.GetObservable().Subscribe(newState => { SubscribeToNewChanges(newState); });
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

                PlayerManager.Instance.Camera.LockCameraToPosition(_cameraSpawn, _cameraDollyOver, 3.0f);
                break;
            case TutorialState.MOVEMENT_TUTORIAL:
                Sequence seq = DOTween.Sequence();
                seq.PrependCallback(() => _movementInstruction.SetActive(true));
                seq.PrependCallback(() => _movementInstruction.GetComponent<CanvasGroup>().alpha = 0);
                seq.Append(_movementInstruction.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
                seq.AppendInterval(4.0f);
                seq.Append(_movementInstruction.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
                seq.AppendCallback(() => _movementInstruction.SetActive(false));
                seq.AppendInterval(1.0f);
                seq.AppendCallback(() => MainGameManager.Instance.TutorialManager.GetNextTutorialStage());
                break;
            case TutorialState.INTERACTION_TUTORIAL:
                Sequence seq1 = DOTween.Sequence();
                seq1.PrependCallback(() => _movementInstruction.SetActive(true));
                seq1.PrependCallback(() => _movementInstruction.GetComponent<CanvasGroup>().alpha = 0);
                seq1.Append(_movementInstruction.GetComponent<CanvasGroup>().DOFade(1, 0.5f));
                seq1.AppendInterval(4.0f);
                seq1.Append(_movementInstruction.GetComponent<CanvasGroup>().DOFade(0, 0.5f));
                seq1.AppendCallback(() => _movementInstruction.SetActive(false));
                seq1.AppendInterval(1.0f);
                seq1.AppendCallback(() => MainGameManager.Instance.TutorialManager.GetNextTutorialStage());
                break;
        }
    }

    private void Swivel()
    {
        PlayerManager.Instance.Camera.LockCameraToPosition(_cameraFinal, _cameraSwivelOver, 1.0f);
    }

    private void FinishIntroCutScene()
    {
        PlayerManager.Instance.Camera.UnlockMouseCursor(false);
        PlayerManager.Instance.Movement.MovePlayer(new Vector3(_cameraFinal.transform.position.x, PlayerManager.Instance.Movement.gameObject.transform.position.y, _cameraFinal.transform.position.z));
        MainGameManager.Instance.TutorialManager.GetNextTutorialStage();
    }


}
