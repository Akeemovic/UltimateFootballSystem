using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UltimateFootballSystem.Gameplay.Common
{
    public class GamerManager : MonoBehaviour
    {
        [Header("Configuration")]      
        [SerializeField] private PlayerInputManager playerInputManager;
        [SerializeField] private GameObject gamerPrefab;
        [SerializeField] private int maxGamerCount = 4;

        [Header("Current State")]
        [SerializeField] private List<Gamer> gamers = new List<Gamer>();

        // Events for gamer lifecycle
        public static event Action<Gamer> OnGamerActivated;
        public static event Action<Gamer> OnGamerDeactivated;
        public static event Action<Gamer, InputDevice> OnGamerDeviceChanged;

        public static GamerManager Instance { get; private set; }
        public List<Gamer> GetGamerList() => gamers;
        public List<Gamer> GetActiveGamers() => gamers.Where(g => g.gameObject.activeSelf).ToList();

        private Dictionary<InputDevice, Gamer> deviceToGamerMap = new Dictionary<InputDevice, Gamer>();
        private List<InputDevice> availableDevices = new List<InputDevice>();
        private List<InputDevice> unassignedDevices = new List<InputDevice>();



        private void Awake()
        {
            if(Instance != null)
            {
                Debug.LogWarning("[GamerManager] Trying to instantiate a second instance of a singleton class.");
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            SpawnAllGamers();
            DetectAvailableDevices();
            AssignDevicesToGamers();
            StartDeviceMonitoring();
        }

        private void SpawnAllGamers()
        {
            // Always spawn all 4 gamers at startup
            for (int i = 0; i < maxGamerCount; i++)
            {
                GameObject gamerObj = Instantiate(gamerPrefab, transform);
                Gamer gamer = gamerObj.GetComponent<Gamer>();

                if (gamer != null)
                {
                    GamerId gamerId = (GamerId)i;
                    gamer.Initialize(gamerId, i, null);
                    gamers.Add(gamer);

                    // Start inactive - will activate when device is assigned
                    gamerObj.SetActive(false);

                    Debug.Log($"[GamerManager] Spawned Gamer {gamerId} (inactive)");
                }
            }
        }

        private void DetectAvailableDevices()
        {
            availableDevices.Clear();

            // Get all connected devices
            foreach (var device in InputSystem.devices)
            {
                if (IsUsableDevice(device))
                {
                    availableDevices.Add(device);
                    Debug.Log($"[GamerManager] Found device: {device.displayName} ({device.GetType().Name})");
                }
            }

            // Sort by priority: Gamepads first, then keyboard/mouse
            availableDevices.Sort((a, b) =>
            {
                bool aIsGamepad = a is Gamepad;
                bool bIsGamepad = b is Gamepad;
                if (aIsGamepad && !bIsGamepad) return -1;
                if (!aIsGamepad && bIsGamepad) return 1;
                return 0;
            });
        }

        private bool IsUsableDevice(InputDevice device)
        {
            return device is Gamepad || device is Keyboard || device is Mouse;
        }

        private void AssignDevicesToGamers()
        {
            deviceToGamerMap.Clear();
            UpdateUnassignedDevices();

            var gamepads = GetAvailableGamepads();
            var keyboardMouse = GetAvailableKeyboardMouse();

            int gamerIndex = 0;

            // Assign gamepads first (gamepad-recommended game)
            foreach (var gamepad in gamepads)
            {
                if (gamerIndex < gamers.Count)
                {
                    AssignDeviceToGamer(gamepad, gamers[gamerIndex]);
                    gamerIndex++;
                }
            }

            // Only use keyboard/mouse if NO gamepads are available
            if (gamepads.Count == 0 && keyboardMouse.Count > 0)
            {
                AssignDeviceToGamer(keyboardMouse[0], gamers[0]);
                gamerIndex++;
            }

            // Ensure Player1 gets a device if any exists
            if (gamerIndex == 0 && availableDevices.Count > 0)
            {
                AssignDeviceToGamer(availableDevices[0], gamers[0]);
            }
        }

        private void AssignDeviceToGamer(InputDevice device, Gamer gamer)
        {
            // Remove device from any previous gamer
            if (deviceToGamerMap.ContainsKey(device))
            {
                var previousGamer = deviceToGamerMap[device];
                previousGamer.SetInputDevice(null);
                previousGamer.gameObject.SetActive(false);
                OnGamerDeactivated?.Invoke(previousGamer);
            }

            // Assign to new gamer
            deviceToGamerMap[device] = gamer;
            gamer.SetInputDevice(device);
            gamer.gameObject.SetActive(true);

            OnGamerActivated?.Invoke(gamer);
            OnGamerDeviceChanged?.Invoke(gamer, device);

            Debug.Log($"[GamerManager] Assigned {device.displayName} to {gamer.GamerId}");
        }

        public void HandleChangeInputDevice(Gamer gamer)
        {
            // Find unassigned devices of different type
            var currentDevice = GetGamerDevice(gamer);
            if (currentDevice == null) return;

            bool isUsingGamepad = currentDevice is Gamepad;

            // Look for available device of opposite type
            InputDevice newDevice = null;

            if (isUsingGamepad)
            {
                // Switch to keyboard/mouse if available
                newDevice = availableDevices.FirstOrDefault(d =>
                    (d is Keyboard || d is Mouse) && !deviceToGamerMap.ContainsKey(d));
            }
            else
            {   
                // Switch to gamepad if available
                newDevice = availableDevices.FirstOrDefault(d =>
                    d is Gamepad && !deviceToGamerMap.ContainsKey(d));
            }

            if (newDevice != null)
            {
                // Remove current device mapping
                deviceToGamerMap.Remove(currentDevice);

                // Assign new device
                AssignDeviceToGamer(newDevice, gamer);

                // Reassign old device if possible
                ReassignUnusedDevices();
            }
        }

        private InputDevice GetGamerDevice(Gamer gamer)
        {
            return deviceToGamerMap.FirstOrDefault(kvp => kvp.Value == gamer).Key;
        }

        private void ReassignUnusedDevices()
        {
            UpdateUnassignedDevices();

            var gamepads = unassignedDevices.Where(d => d is Gamepad).ToList();
            var keyboardMouse = unassignedDevices.Where(d => d is Keyboard || d is Mouse).ToList();

            // Get inactive gamers ordered by priority (Player1 last to deactivate = first to reactivate)
            var inactiveGamers = gamers.Where(g => !g.gameObject.activeSelf)
                                      .OrderBy(g => (int)g.GamerId).ToList();

            // Assign gamepads first
            foreach (var gamepad in gamepads)
            {
                if (inactiveGamers.Count == 0) break;
                var gamer = inactiveGamers[0];
                inactiveGamers.RemoveAt(0);
                AssignDeviceToGamer(gamepad, gamer);
            }

            // Only assign keyboard/mouse if no gamepads available and no active gamepads
            if (gamepads.Count == 0 && !HasActiveGamepads() && keyboardMouse.Count > 0 && inactiveGamers.Count > 0)
            {
                AssignDeviceToGamer(keyboardMouse[0], inactiveGamers[0]);
            }
        }

        private void StartDeviceMonitoring()
        {
            InputSystem.onDeviceChange += OnDeviceChange;
            Debug.Log("[GamerManager] Device monitoring started");
        }

        private void OnDeviceChange(InputDevice device, InputDeviceChange change)
        {
            if (change == InputDeviceChange.Added)
            {
                Debug.Log($"[GamerManager] Device connected: {device.displayName}");
                if (IsUsableDevice(device))
                {
                    availableDevices.Add(device);

                    // If it's a gamepad and someone is using keyboard/mouse, prioritize gamepad
                    if (device is Gamepad)
                    {
                        HandleGamepadPriority(device);
                    }
                    else
                    {
                        ReassignUnusedDevices();
                    }
                }
            }
            else if (change == InputDeviceChange.Removed)
            {
                Debug.Log($"[GamerManager] Device disconnected: {device.displayName}");
                availableDevices.Remove(device);

                if (deviceToGamerMap.ContainsKey(device))
                {
                    var affectedGamer = deviceToGamerMap[device];
                    deviceToGamerMap.Remove(device);

                    // Handle disconnection with Player1 priority
                    HandleDeviceDisconnection(affectedGamer, device);
                }
            }
        }

        public Gamer GetGamerById(GamerId id)
        {
            return gamers.FirstOrDefault(g => g.GamerId == id);
        }

        private void UpdateUnassignedDevices()
        {
            unassignedDevices = availableDevices.Where(d => !deviceToGamerMap.ContainsKey(d)).ToList();
        }

        private List<InputDevice> GetAvailableGamepads()
        {
            return unassignedDevices.Where(d => d is Gamepad).ToList();
        }

        private List<InputDevice> GetAvailableKeyboardMouse()
        {
            return unassignedDevices.Where(d => d is Keyboard || d is Mouse).ToList();
        }

        private bool HasActiveGamepads()
        {
            return deviceToGamerMap.Keys.Any(d => d is Gamepad);
        }

        private void HandleGamepadPriority(InputDevice gamepad)
        {
            // If someone is using keyboard/mouse and we have a gamepad, prioritize gamepad
            var keyboardMouseUsers = deviceToGamerMap
                .Where(kvp => kvp.Key is Keyboard || kvp.Key is Mouse)
                .ToList();

            if (keyboardMouseUsers.Count > 0)
            {
                // Find the highest priority gamer using keyboard/mouse (lowest ID)
                var priorityUser = keyboardMouseUsers
                    .OrderBy(kvp => (int)kvp.Value.GamerId)
                    .First();

                var oldDevice = priorityUser.Key;
                var gamer = priorityUser.Value;

                // Remove old mapping
                deviceToGamerMap.Remove(oldDevice);

                // Assign gamepad
                AssignDeviceToGamer(gamepad, gamer);

                Debug.Log($"[GamerManager] Prioritized gamepad for {gamer.GamerId}, freeing {oldDevice.displayName}");
            }
            else
            {
                ReassignUnusedDevices();
            }
        }

        private void HandleDeviceDisconnection(Gamer affectedGamer, InputDevice removedDevice)
        {
            // Deactivate the affected gamer
            affectedGamer.SetInputDevice(null);
            affectedGamer.gameObject.SetActive(false);
            OnGamerDeactivated?.Invoke(affectedGamer);

            Debug.Log($"[GamerManager] {affectedGamer.GamerId} deactivated due to device removal: {removedDevice.displayName}");

            // Find available devices for reassignment
            UpdateUnassignedDevices();
            var availableGamepads = GetAvailableGamepads();
            var availableKeyboardMouse = GetAvailableKeyboardMouse();

            // If Player1 was affected and we have devices, reassign immediately
            if (affectedGamer.GamerId == GamerId.Player1 && (availableGamepads.Count > 0 || availableKeyboardMouse.Count > 0))
            {
                var deviceToAssign = availableGamepads.Count > 0 ? availableGamepads[0] : availableKeyboardMouse[0];
                AssignDeviceToGamer(deviceToAssign, affectedGamer);
                Debug.Log($"[GamerManager] Player1 immediately reassigned to {deviceToAssign.displayName}");
                return;
            }

            // For other players, try standard reassignment
            ReassignUnusedDevices();

            // If Player1 is still inactive but we have devices, ensure they get one
            var player1 = GetGamerById(GamerId.Player1);
            if (player1 != null && !player1.gameObject.activeSelf)
            {
                UpdateUnassignedDevices();
                if (unassignedDevices.Count > 0)
                {
                    // Prioritize gamepad for Player1
                    var deviceForPlayer1 = unassignedDevices.FirstOrDefault(d => d is Gamepad) ?? unassignedDevices[0];
                    AssignDeviceToGamer(deviceForPlayer1, player1);
                    Debug.Log($"[GamerManager] Player1 priority assignment: {deviceForPlayer1.displayName}");
                }
            }
        }

        public List<InputDevice> GetUnassignedDevices()
        {
            UpdateUnassignedDevices();
            return unassignedDevices.ToList();
        }

        public List<InputDevice> GetAssignedDevices()
        {
            return deviceToGamerMap.Keys.ToList();
        }

        public List<InputDevice> GetAllAvailableDevices()
        {
            return availableDevices.ToList();
        }

        private void OnDestroy()
        {
            InputSystem.onDeviceChange -= OnDeviceChange;
        }
    }
}