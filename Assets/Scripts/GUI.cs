using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUI 
{
    public bool imposterGuiEnabled = false;
    public void updateRoom(string roomName)
    {
        GameObject.Find("Canvas/panelRoom/text").GetComponent<TextMeshProUGUI>().SetText(roomName);
    }

    public void updateKillCooldown(float killCooldown)
    {
        Debug.Log(killCooldown);
        GameObject.Find("Canvas/panelKillCooldown/panelProgressBar/progressBar").GetComponent<RectTransform>().sizeDelta = new Vector2(390f * killCooldown, 88f);
        GameObject.Find("Canvas/panelKillCooldown/panelProgressBar/progressBarRest").GetComponent<RectTransform>().position = GameObject.Find("Canvas/panelKillCooldown/panelProgressBar/progressBar").GetComponent<RectTransform>().position + new Vector3(390f * killCooldown, 0, 0);
        GameObject.Find("Canvas/panelKillCooldown/panelProgressBar/progressBarRest").GetComponent<RectTransform>().sizeDelta = new Vector2(390f * (1f - killCooldown), 88f);
    }

    public void updateTaskProgress(float taskProgress)
    {
        Debug.Log(new Vector2(390f * (float) taskProgress, 88f));
        GameObject.Find("Canvas/panelTaskProgress/panelProgressBar/progressBar").GetComponent<RectTransform>().sizeDelta = new Vector2(390f * taskProgress, 88f);
        GameObject.Find("Canvas/panelTaskProgress/panelProgressBar/progressBarRest").GetComponent<RectTransform>().position = GameObject.Find("Canvas/panelTaskProgress/panelProgressBar/progressBar").GetComponent<RectTransform>().position + new Vector3(390f * taskProgress, 0, 0);
        GameObject.Find("Canvas/panelTaskProgress/panelProgressBar/progressBarRest").GetComponent<RectTransform>().sizeDelta = new Vector2(390f * (1f - taskProgress), 88f);
    }
    public void updateSabortageCountdown(float sabotageCountdown)
    {
        Debug.Log(sabotageCountdown);
        GameObject.Find("Canvas/panelSabotageCooldown/panelProgressBar/progressBar").GetComponent<RectTransform>().sizeDelta = new Vector2(390f * sabotageCountdown, 88f);
        GameObject.Find("Canvas/panelSabotageCooldown/panelProgressBar/progressBarRest").GetComponent<RectTransform>().position = GameObject.Find("Canvas/panelSabotageCooldown/panelProgressBar/progressBar").GetComponent<RectTransform>().position + new Vector3(390f * sabotageCountdown, 0, 0);
        GameObject.Find("Canvas/panelSabotageCooldown/panelProgressBar/progressBarRest").GetComponent<RectTransform>().sizeDelta = new Vector2(390f * (1f - sabotageCountdown), 88f);
    }

    public void setImposterGui(bool active) {
        if (active != imposterGuiEnabled) {
            setActiveRecursive(GameObject.Find("Canvas/panelSabotageCooldown"), active);
            setActiveRecursive(GameObject.Find("Canvas/panelKillCooldown"), active);
            imposterGuiEnabled = !imposterGuiEnabled;
        }
    }

    private void setActiveRecursive(GameObject obj, bool active) {
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
