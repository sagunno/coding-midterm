using UnityEngine;
using UnityEngine.SceneManagement;

// Put on a trigger collider.
// targetScene   = scene to load
// targetSpawnId = id of a SpawnPoint (empty object) in THAT scene
public class sceneChanger : MonoBehaviour
{
    public string targetScene;
    public string targetSpawnId;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        GameController.instance.setSpawnId(targetSpawnId);
        SceneManager.LoadScene(targetScene);
    }
}