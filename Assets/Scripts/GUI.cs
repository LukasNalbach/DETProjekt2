using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.SceneManagement;
using TMPro;

public class GUI : MonoBehaviour
{
    public bool imposterGuiEnabled = true;
    public bool sabotageGuiEnabled = true;
    public bool standardGuiEnabled = true;

    public float scaleFactor;
    private bool messageShown = false;
    public void Awake() {
        SceneManager.LoadSceneAsync("IngameGUI", LoadSceneMode.Additive);
    }
    public void updateRoom(string roomName)
    {
        GameObject.Find("Canvas/panelRoom/text").GetComponent<TextMeshProUGUI>().SetText(roomName);
    }

    public void showMessage(string text, int duration) {
        StartCoroutine(coShowMessage(text, duration));
    }

    private IEnumerator coShowMessage(string text, int duration) {
        while (messageShown) {
            yield return new WaitForEndOfFrame();
        }
        messageShown = true;
        setActiveRecursive(GameObject.Find("Canvas2/panelInfo"), true);
        GameObject.Find("Canvas2/panelInfo").GetComponent<Image>().color = new Color(0.39f, 0.39f, 0.39f, 1);
        GameObject.Find("Canvas2/panelInfo/textInfo").GetComponent<TextMeshProUGUI>().SetText(text);
        yield return new WaitForSeconds(duration);
        GameObject.Find("Canvas2/panelInfo/textInfo").GetComponent<TextMeshProUGUI>().SetText("");
        setActiveRecursive(GameObject.Find("Canvas2/panelInfo"), false);
        messageShown = false;
        yield return null;
    }

    public void updateKillCooldown(float killCooldown)
    {
        GameObject.Find("Canvas/panelKillCooldown/panelProgressBar/progressBar").GetComponent<RectTransform>().sizeDelta = new Vector2(443f * killCooldown, 88f);
    }

    public void updateTaskProgress(float taskProgress)
    {
        GameObject.Find("Canvas/panelTaskProgress/panelProgressBar/progressBar").GetComponent<RectTransform>().sizeDelta = new Vector2(443f * taskProgress, 88f);
    }
    public void updateSabortageCountdown(float sabotageCountdown)
    {
        GameObject.Find("Canvas/panelSabotageCooldown/panelProgressBar/progressBar").GetComponent<RectTransform>().sizeDelta = new Vector2(443f * sabotageCountdown, 88f);
    }

    public void setStandardGui(bool active) {
        if (GameObject.Find("Canvas") != null && active != standardGuiEnabled) {
            setActiveRecursive(GameObject.Find("Canvas/panelTaskProgress"), active);
            setActiveRecursive(GameObject.Find("Canvas/panelRoom"), active);
            setActiveRecursive(GameObject.Find("Canvas2/controlsInfo"), active);
            standardGuiEnabled = active;
        }
    }

    public void setImposterGui(bool active) {
        if (GameObject.Find("Canvas") != null && active != imposterGuiEnabled) {
            setActiveRecursive(GameObject.Find("Canvas/panelKillCooldown"), active);
            imposterGuiEnabled = active;
        }
    }
    public void setSabotageGui(bool active) {
        if (GameObject.Find("Canvas") != null && active != sabotageGuiEnabled) {
            setActiveRecursive(GameObject.Find("Canvas/panelSabotageCooldown"), active);
            sabotageGuiEnabled = active;
        }
    }

    public static void setActiveRecursive(GameObject obj, bool active) {
        if (obj != null) {
                foreach (Transform child in obj.transform) {
                setActiveRecursive(child.gameObject, active);
            }
            if (obj.GetComponent<Image>() != null) {
                obj.GetComponent<Image>().enabled = active;
            }
            if (obj.GetComponent<TextMeshProUGUI>() != null) {
                obj.GetComponent<TextMeshProUGUI>().enabled = active;
            }
        }
    }
}
