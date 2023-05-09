using UnityEngine;

public class GameList : MonoBehaviour
{
    public IGameListMonoBehaviourBridge List { get; set; }

    public void LateUpdate()
    {
        List.UpdateView();
    }

    public void OnDestroy()
    {
        List.DeactivateAll();
    }
}

public interface IGameListMonoBehaviourBridge
{
    void DeactivateAll();
    void UpdateView();
}
