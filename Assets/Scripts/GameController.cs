using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yarn.Unity;

public class GameController : MonoBehaviour
{
    //singletonnn
    public static GameController instance { get; private set; }

    public Transform startingLocation; //only used for the very first spawn
    public GameObject ThirdpersonRig;
    public DialogueRunner dialogue; // PRESISTEANNNCEEE BECUZ THIS MF DOESNT DIE

    private bool firstSceneLoaded = false;

    //id of the spawnPoint to use in the next scene, set by a door right before loading
    private string nextSpawnId = "";

    //quest stufff
    public List<KidCapsuleData> followers = new List<KidCapsuleData>();
    private List<KidFollower> liveFollowers = new List<KidFollower>(); //rescued kids

    //for the rubble
    public bool rubbleCleared = false;

    //singletooon stuff
    public void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this);

            //fires every time a new scene loads
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!firstSceneLoaded && scene.buildIndex == 0)
        {
            SpawnCharacter(startingLocation.position, startingLocation.rotation);
            firstSceneLoaded = true;
        }
        else if (!string.IsNullOrEmpty(nextSpawnId))
        {
            //door gave us an id, find that spawnPoint in the new scene
            foreach (spawnPoint sp in FindObjectsByType<spawnPoint>())
            {
                if (sp.id == nextSpawnId)
                {
                    SpawnCharacter(sp.transform.position, sp.transform.rotation);
                    nextSpawnId = "";
                    return;
                }
            }

            Debug.LogWarning("No spawnPoint with id '" + nextSpawnId + "' in scene " + scene.name);
            nextSpawnId = "";
        }
    }

    public void SpawnCharacter(Vector3 location, Quaternion rotation)
    {
        Instantiate(ThirdpersonRig, location, rotation);
        LineUpFollowers();
    }

    //doors call this right before loading the next scene
    public void setSpawnId(string id)
    {
        nextSpawnId = id;
    }

    private Transform GetPlayer()
    {
        return GameObject.FindGameObjectWithTag("Player").transform;
    }

    //called when a kid is saved!!!
    public void AddFollower(KidCapsuleData kid, KidFollower follower)
    {
        if (followers.Contains(kid)) return;
        followers.Add(kid);

        //if no followers, will trail behind player, else, they are gonna trail behind the last guy LOL
        Transform leader = liveFollowers.Count == 0 ? GetPlayer() : liveFollowers[liveFollowers.Count - 1].transform;
        follower.Setup(leader, kid);
        liveFollowers.Add(follower);
    }

    //after a scene loads there's a brand new player, so line everyone back up behind them!!!
    private void LineUpFollowers()
    {
        if (liveFollowers.Count == 0) return;
        Transform player = GetPlayer();

        for (int i = 0; i < liveFollowers.Count; i++)
        {
            KidFollower f = liveFollowers[i];
            Transform leader = i == 0 ? player : liveFollowers[i - 1].transform;

            f.transform.position = player.position - player.forward * f.spacing * (i + 1);
            f.SetLeader(leader);
        }
    }

    // yarn: <<clear_rubble>>
    [YarnCommand("clear_rubble")]
    public static void ClearRubble()
    {
        instance.rubbleCleared = true;
    }
}