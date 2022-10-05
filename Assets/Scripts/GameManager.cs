using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public Player playerPrefab;
    public Platform platformPrefab;
    public float minSpawnX;
    public float minSpawnY;
    public float maxSpawnX;
    public float maxSpawnY;

    Player m_player;
    int m_score;
    bool m_isGameStarted;
    public bool IsGameStarted { get => m_isGameStarted;}

    public CameraController mainCam;
    public float powerBarUp;

    public override void Awake()
    {
        MakeSingleton(false);
    }
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        GameUIManager.Ins.UpdateScoreCounting("" + m_score);

        GameUIManager.Ins.UpdatePowerBar(0, 1);

        AudioController.Ins.PlayBackgroundMusic();
    }

    public void PlayGame()
    {
        GameUIManager.Ins.ShowGameGui(true);
        StartCoroutine(PlatformInit());
    }

    IEnumerator PlatformInit()
    {
        Platform platformClone = null;
        if (platformPrefab)
        {
            platformClone = Instantiate(platformPrefab, new Vector2(0, Random.Range(minSpawnY, maxSpawnY)), Quaternion.identity);
            platformClone.id = platformClone.gameObject.GetInstanceID();
        }

        yield return new WaitForSeconds(0.5f);

        if (playerPrefab)
        {
            m_player = Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
            m_player.lastPlatformId = platformClone.id;
        }

        if (platformPrefab)
        {
            float spawnX = m_player.transform.position.x + minSpawnX;
            float spawnY = Random.Range(minSpawnY, maxSpawnY);

            Platform p = Instantiate(platformPrefab, new Vector2(spawnX, spawnY), Quaternion.identity);
        }

        yield return new WaitForSeconds(0.5f);
        m_isGameStarted = true;
    }


    public void CreatePlatform(float playerXPos)
    {
        if (!platformPrefab || !playerPrefab) return;

        if (mainCam)
        {
            mainCam.LerpTrigger(playerXPos+minSpawnX);
        }

        float playerPosX = m_player.transform.position.x;
        float spawnX = Random.Range(playerPosX + minSpawnX, playerPosX + maxSpawnX);
        float spawnY = Random.Range(minSpawnY, maxSpawnY);

        Platform p = Instantiate(platformPrefab, new Vector2(spawnX, spawnY), Quaternion.identity);
        p.id = p.gameObject.GetInstanceID();
    }

    public void IncerementScore()
    {
        m_score++;
        Prefs.bestScore = m_score;
        GameUIManager.Ins.UpdateScoreCounting(""+m_score);
        AudioController.Ins.PlaySound(AudioController.Ins.getScore);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
