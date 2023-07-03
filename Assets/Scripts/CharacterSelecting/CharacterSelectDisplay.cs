using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using UnityEngine.UI;

public class CharacterSelectDisplay : NetworkBehaviour
{
    [SerializeField] CharacterDatabase characterDatabase;
    [SerializeField] Transform charactersHolder;
    [SerializeField] CharacterSelectButton characterSelectButton;
    [SerializeField] PlayerCard[] playerCards;
 
    [SerializeField] TMP_Text characterNameText;
    [SerializeField] GameObject characterInfoPanel;
    [SerializeField] Transform introSpawnPoint;
    [SerializeField] TMP_Text joinCodeText;
    [SerializeField] Button lockInButton;

    private GameObject introInstance;
    private List<CharacterSelectButton> characterButtons = new List<CharacterSelectButton>();

    private NetworkList<CharacterSelectState> players;

    private void Awake()
    {
        players = new NetworkList<CharacterSelectState>();
    }
    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = characterDatabase.GetAllCharacters();

            foreach(var character in allCharacters)
            {
                var selectbuttonInstance = Instantiate(characterSelectButton, charactersHolder);
                selectbuttonInstance.SetCharacter(this, character);
                characterButtons.Add(selectbuttonInstance);
            }

            players.OnListChanged += HandlePlayerStateChanged;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;

            foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }
        }

        if (IsHost)
        {
            joinCodeText.text = HostManager.Instance.JoinCode;
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged -= HandlePlayerStateChanged;
        }

        if(IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        players.Add(new CharacterSelectState(clientId));
    }

    private void HandleClientDisconnected(ulong clientId)
    {
        for (int i = 0; i < players.Count; i++)
        {
            /*
            if (players[i].ClientId == clientId)
            {
                players.RemoveAt(i);
                break;
            }*/

            if (players[i].ClientId != clientId) { continue; }

            players.RemoveAt(i);
            break;
        }
    }

    public void Select(Character character)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != NetworkManager.Singleton.LocalClientId){ continue; }

            if (players[i].IsLockedIn) { return; }

            if (players[i].CharacterId == character.Id) { return; }

            if(IsCharacterTaken(character.Id, false)) { return; }
        }

        characterNameText.text = character.DisplayName;

        characterInfoPanel.SetActive(true);

        if(introInstance != null)
        {
            Destroy(introInstance);
        }

        introInstance = Instantiate(character.IntroPrefab, introSpawnPoint);

        SelectServerRpc(character.Id);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            //
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterID(characterId)) { return; }

            if (IsCharacterTaken(characterId, true)) { return; }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                characterId,
                players[i].IsLockedIn);
            
        }
    }

    public void LockIn()
    {
        LockInServerRpc();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientId != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterID(players[i].CharacterId)) { return; }

            if (IsCharacterTaken(players[i].CharacterId, true)) { return; }

            players[i] = new CharacterSelectState(
                players[i].ClientId,
                players[i].CharacterId,
                true);            
        }
        foreach (var player in players)
        {
            if (!player.IsLockedIn) { return; }
        }
        foreach (var player in players)
        {
            HostManager.Instance.SetCharacter(player.ClientId, player.CharacterId);
        }

        HostManager.Instance.StartGame();
    }

    private void HandlePlayerStateChanged(NetworkListEvent<CharacterSelectState> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i)
            {
                playerCards[i].UpdateDisplay(players[i]);
            }
            else
            {
                playerCards[i].DisabeleDisplay();
            }
        }

        foreach (var button in characterButtons)
        {
            if (button.IsDisabled) { continue; }

            if(IsCharacterTaken(button.Character.Id, false))
            {
                button.SetDisabled();
            }
        }

        foreach (var player in players)
        {
            if(player.ClientId != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.IsLockedIn)
            {
                lockInButton.interactable = false;
                break;
            }

            if(IsCharacterTaken(player.CharacterId, false))
            {
                lockInButton.interactable = false;
                break;
            }

            lockInButton.interactable = true;
            break;
        }
    }

    private bool IsCharacterTaken(int characterId, bool checkAll)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!checkAll)
            {
                if (players[i].ClientId == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (players[i].IsLockedIn && players[i].CharacterId == characterId) { return true; }
        }

        return false;
    }
}
