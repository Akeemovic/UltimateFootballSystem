using System;
using UltimateFootballSystem.Gameplay.InputManagement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UltimateFootballSystem.Gameplay.Common
{
    public class Gamer : MonoBehaviour
    {
        [Header("Gamer State")]
        [SerializeField] private GamerSide gamerSide = GamerSide.Neutral;
        [SerializeField] private GamerId gamerId;
        [SerializeField] private int gamerIndex;

        public int GamerIndex { get; private set; }
        public GamerId GamerId { get; private set; }
        public GamerSide GamerSide => gamerSide;
        public PlayerInput Input { get; private set; }
        public bool HasController => Input != null;
        public GamerInputController InputController => gamerInputController;

        private GamerInputController gamerInputController;

        private void Awake()
        {
            gamerInputController = new GamerInputController();
            // gamerInputController.SetState(GameState.SideSelection);
        }

        void Start()
        {
            Debug.Log($"[Gamer] {GamerId} initialized - HasController: {HasController}");
        }

        public void Initialize(GamerId id, int index, PlayerInput playerInput)
        {
            GamerId = id;
            GamerIndex = index;
            Input = playerInput;
            gamerId = id;
            gamerIndex = index;

            Debug.Log($"[Gamer] {GamerId} initialized at index {GamerIndex}, HasController: {HasController}");

            if (HasController)
            {
                Debug.Log($"[Gamer] {GamerId} assigned device: {Input.devices[0].displayName}");
            }
            else
            {
                Debug.Log($"[Gamer] {GamerId} spawned without controller (waiting for assignment)");
            }
        }

        public void SetInputDevice(InputDevice device)
        {
            if (device != null)
            {
                Debug.Log($"[Gamer] {GamerId} assigned device: {device.displayName}");

                // Bind the input controller to this specific device
                gamerInputController?.SetAssignedDevice(device);
            }
            else
            {
                Debug.Log($"[Gamer] {GamerId} device removed");

                // Clear device binding
                gamerInputController?.SetAssignedDevice(null);
            }
        }

        public void AssignController(PlayerInput playerInput)
        {
            Input = playerInput;
            Debug.Log($"[Gamer] {GamerId} controller assigned: {Input.devices[0].displayName}");
        }

        public void SetGamerSide(GamerSide side)
        {
            gamerSide = side;
            Debug.Log($"[Gamer] {GamerId} side set to: {side}");
        }
    }
}