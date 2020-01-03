using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class ShowPanels : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject menuPanel;
    public GameObject multiplayerPanel; 

    private GameObject activePanel;
    private MenuObject activePanelMenuObject;
    private EventSystem eventSystem;

    private void SetSelection(GameObject panelToSetSelected)
    {
        activePanel = panelToSetSelected;
        activePanelMenuObject = activePanel.GetComponent<MenuObject>();
        if (activePanelMenuObject != null)
        {
            activePanelMenuObject.SetFirstSelected();
        }
    }

    public void Start()
    {
        SetSelection(menuPanel);
    }

    //Call this function to activate and display the Options panel during the main menu
    public void ShowOptionsPanel()
    {
        optionsPanel.SetActive(true);
        menuPanel.SetActive(false);
        SetSelection(optionsPanel);
    }

    //Call this function to deactivate and hide the Options panel during the main menu
    public void HideOptionsPanel()
    {
        menuPanel.SetActive(true);
        optionsPanel.SetActive(false);
    }

    //Call this function to activate and display the main menu panel during the main menu
    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        SetSelection(menuPanel);
    }

    //Call this function to deactivate and hide the main menu panel during the main menu
    public void HideMenu()
    {
        menuPanel.SetActive(false);
    }

    public void ShowMultiplayerPanel()
    {
        menuPanel.SetActive(false);
        multiplayerPanel.SetActive(true);
        SetSelection(multiplayerPanel);
    }

    public void HideMultiplayerPanel()
    {
        menuPanel.SetActive(true);
        multiplayerPanel.SetActive(false);
    }
}