using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System;
using TMPro;
using UnityEngine.UI;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField] ClientManager clientManager;
    [SerializeField] GameObject client;
    [SerializeField] GameObject connectingPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] TMP_InputField joinCodeInputField;

    string connection_field;

    private void Awake()
    {
        clientManager = client.GetComponent<ClientManager>();
    }

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Player Id: { AuthenticationService.Instance.PlayerId}");
        }
        catch (Exception e) 
        {
            Debug.Log(e);
            return;
        }
        
        connectingPanel.SetActive( false );
        menuPanel.SetActive( true );
    }

    private void Update()
    {
        connection_field = joinCodeInputField.text;
    }

    public void StartHost()
    {
        HostManager.Instance.StartHost();
    }

    public async void StartClient()
    {
        Debug.Log(connection_field);
        await ClientManager.Instance.StartClient(connection_field);
    }

}
