using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryMenu : Menu
{
    public static VictoryMenu instance = null;
    public GameObject fireworkPrefab = null;
    float timer = 1f;

    void Start()
    {
        if (instance)
        {
            Debug.LogError("Trying to create more than one VictoryMenu!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void OnContinueButton()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    void Update()
    {
        if (ROOT != null && ROOT.gameObject.activeInHierarchy)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = 1f;
                if (fireworkPrefab)
                {
                    float x = Random.Range(-400f, 400f);
                    float y = Random.Range(-550f, 550f);
                    Vector3 pos = new Vector3(x, y, 0);
                    Instantiate(fireworkPrefab, pos, Quaternion.identity);
                }
            }
        }
    }
}
