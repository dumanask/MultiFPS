using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField] int id = -1;
    [SerializeField] string displayName = "New Display Name";
    [SerializeField] Sprite icon;
    [SerializeField] GameObject introPrefab;
    [SerializeField] NetworkObject gameplayPrefab;

    public int Id => id;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public GameObject IntroPrefab => introPrefab;
    public NetworkObject GameplayPrefab => gameplayPrefab;
}
