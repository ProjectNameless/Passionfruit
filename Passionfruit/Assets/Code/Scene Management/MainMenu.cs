using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject creditsGO;
    public int creditsMultiplier;
	public void LoadSaveFile()
    {
        //Instantiate LoadData GO with dont destroy on load
        //
        //Load main scene
        //
        //apply data
    }
    public void LoadScene(int Index)
    {
        SceneManager.LoadSceneAsync(Index);
    }
    public void OptionsMenu()
    {

    }
    public void Exit()
    {
        Application.Quit();
    }
    public void RollCredits()
    {
        StopAllCoroutines();
        StartCoroutine(RollingCredits());
    }
    public void StopRollingCredits()
    {
        StopCoroutine("RollingCredits");
        creditsGO.SetActive(false);
    }
    IEnumerator RollingCredits()
    {
        creditsGO.SetActive(true);
        GameObject credits = GameObject.FindGameObjectWithTag("Credits");
        credits.transform.position = new Vector3(Screen.width / 2, 0 - (credits.GetComponent<RectTransform>().rect.height / 2), 0);
        while (true)
        {
            credits.transform.Translate(0, creditsMultiplier * Time.deltaTime, 0);
            yield return null;
        }
    }
}
