using UnityEngine;

public class SceneManager_Main : MonoBehaviour
{
    public Instancer playerInstancer = new Instancer();
    private void Start()
    {
        playerInstancer.Instance(transform);
    }
}
