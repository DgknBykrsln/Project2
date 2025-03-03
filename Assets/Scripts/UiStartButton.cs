using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

public class UiStartButton : Button
{
    private GameManager _gameManager;

    [Inject]
    private void Construct(GameManager gameManager)
    {
        _gameManager = gameManager;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        _gameManager.GameStarted();
    }
}