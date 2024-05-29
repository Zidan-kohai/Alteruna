using Alteruna;
using Alteruna.Trinity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ServersWindow : CommunicationBridge
{
    [SerializeField] private GameObject LANEntryPrefab;
    [SerializeField] private GameObject WANEntryPrefab;
    [SerializeField] private GameObject ContentContainer;

    public bool ShowUserCount = false;

    // manual refresh can be done by calling Multiplayer.RefreshRoomList();
    public bool AutomaticallyRefresh = true;
    public float RefreshInterval = 5.0f;

    private readonly List<RoomObject> _roomObjects = new List<RoomObject>();
    private float _refreshTime;

    private int _count;
    private string _connectionMessage = "Connecting";
    private float _statusTextTime;
    private int _roomI = -1;

    private void Awake()
    {
        if (Multiplayer == null)
        {
            Multiplayer = FindObjectOfType<Multiplayer>();
        }
    }

    private void OnEnable()
    {
        base.OnEnable();

        Multiplayer.OnRoomListUpdated.AddListener(UpdateList);
    }
    private void FixedUpdate()
    {
        if (Multiplayer.IsConnected)
        {
            if (!AutomaticallyRefresh || (_refreshTime += Time.fixedDeltaTime) < RefreshInterval) return;
            _refreshTime -= RefreshInterval;

            Multiplayer.RefreshRoomList();


            ResponseCode blockedReason = Multiplayer.GetLastBlockResponse();

            if (blockedReason == ResponseCode.NaN) return;

            string str = blockedReason.ToString();
            str = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
        }
        else if ((_statusTextTime += Time.fixedDeltaTime) >= 1)
        {
            _statusTextTime -= 1;
            ResponseCode blockedReason = Multiplayer.GetLastBlockResponse();
            if (blockedReason != ResponseCode.NaN)
            {
                string str = blockedReason.ToString();
                str = string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? " " + x : x.ToString()));
                return;
            }
            _count++;
        }
    }
    private void UpdateList(Multiplayer multiplayer)
    {
        if (ContentContainer == null) return;

        RemoveExtraRooms(multiplayer);

        for (int i = 0; i < multiplayer.AvailableRooms.Count; i++)
        {
            Room room = multiplayer.AvailableRooms[i];
            RoomObject entry;

            if (_roomObjects.Count > i)
            {
                if (room.Local != _roomObjects[i].Lan)
                {
                    Destroy(_roomObjects[i].GameObject);
                    entry = new RoomObject(Instantiate(WANEntryPrefab, ContentContainer.transform), room.ID, room.Local);
                    _roomObjects[i] = entry;
                }
                else
                {
                    entry = _roomObjects[i];
                    entry.Button.onClick.RemoveAllListeners();
                }
            }
            else
            {
                entry = new RoomObject(Instantiate(WANEntryPrefab, ContentContainer.transform), room.ID, room.Local);
                _roomObjects.Add(entry);
            }

            if (
            // Hide private rooms.
                room.InviteOnly && room.ID != _roomI ||
                // Hide locked rooms.
                room.IsLocked ||
                // Hide full rooms.
                room.GetUserCount() > room.MaxUsers
            )
            {
                entry.GameObject.SetActive(false);
                entry.GameObject.name = room.Name;
                continue;
            }

            string newName = room.Name;

            if (ShowUserCount)
            {
                newName += " (" + room.GetUserCount() + "/" + room.MaxUsers + ")";
            }

            if (entry.GameObject.name != newName)
            {
                entry.GameObject.name = newName;
                entry.Text.text = newName;
            }

            entry.GameObject.SetActive(true);

            if (room.ID == _roomI)
            {
                entry.Button.interactable = false;
            }
            else
            {
                entry.Button.interactable = true;
                entry.Button.onClick.AddListener(() =>
                {
                    room.Join();
                    UpdateList(multiplayer);
                });
            }
        }
    }
    private void RemoveExtraRooms(Multiplayer multiplayer)
    {
        int l = _roomObjects.Count;
        if (multiplayer.AvailableRooms.Count < l)
        {
            for (int i = 0; i < l; i++)
            {
                if (multiplayer.AvailableRooms.All(t => t.ID != _roomObjects[i].ID))
                {
                    Destroy(_roomObjects[i].GameObject);
                    _roomObjects.RemoveAt(i);
                    i--;
                    l--;
                    if (multiplayer.AvailableRooms.Count >= l) return;
                }
            }
        }
    }

    private void OnDisable()
    {
        Multiplayer.OnRoomListUpdated.RemoveListener(UpdateList);
    }

    private struct RoomObject
    {
        public readonly GameObject GameObject;
        public readonly TextMeshProUGUI Text;
        public readonly Button Button;
        public readonly uint ID;
        public readonly bool Lan;

        public RoomObject(GameObject obj, uint id, bool lan = false)
        {
            GameObject = obj;
            Text = obj.GetComponentInChildren<TextMeshProUGUI>();
            Button = obj.GetComponentInChildren<Button>();
            ID = id;
            Lan = lan;
        }
    }
}
