using System.Collections.Generic;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
    public class TacticBoardManager
    {
        private static TacticBoardManager _instance;
        private static readonly object _lock = new object();

        private readonly List<TacticsBoardController> _controllers = new List<TacticsBoardController>();
        private TacticsBoardController _activeController;

        public static TacticBoardManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                            _instance = new TacticBoardManager();
                    }
                }
                return _instance;
            }
        }

        public TacticsBoardController ActiveController => _activeController;
        public IReadOnlyList<TacticsBoardController> Controllers => _controllers.AsReadOnly();
        public int ControllerCount => _controllers.Count;

        private TacticBoardManager() { }

        public void RegisterController(TacticsBoardController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("Attempted to register null TacticsBoardController");
                return;
            }

            if (!_controllers.Contains(controller))
            {
                _controllers.Add(controller);
                Debug.Log($"Registered TacticsBoardController: {controller.name}");

                // Set as active if it's the first controller or no active controller
                if (_activeController == null)
                {
                    SetActiveController(controller);
                }
            }
        }

        public void UnregisterController(TacticsBoardController controller)
        {
            if (controller == null) return;

            if (_controllers.Remove(controller))
            {
                Debug.Log($"Unregistered TacticsBoardController: {controller.name}");

                // If this was the active controller, set a new one
                if (_activeController == controller)
                {
                    _activeController = _controllers.Count > 0 ? _controllers[0] : null;
                    if (_activeController != null)
                    {
                        Debug.Log($"Active controller changed to: {_activeController.name}");
                    }
                }
            }
        }

        public void SetActiveController(TacticsBoardController controller)
        {
            if (controller == null)
            {
                Debug.LogWarning("Attempted to set null controller as active");
                return;
            }

            if (!_controllers.Contains(controller))
            {
                Debug.LogWarning($"Controller {controller.name} is not registered. Registering now.");
                RegisterController(controller);
            }

            _activeController = controller;
            Debug.Log($"Active controller set to: {controller.name}");
        }

        public TacticsBoardController GetControllerByName(string name)
        {
            return _controllers.Find(c => c.name.Equals(name));
        }

        public TacticsBoardController GetControllerByIndex(int index)
        {
            if (index >= 0 && index < _controllers.Count)
                return _controllers[index];
            return null;
        }

        public void ClearControllers()
        {
            _controllers.Clear();
            _activeController = null;
            Debug.Log("All controllers cleared");
        }

        public bool IsControllerRegistered(TacticsBoardController controller)
        {
            return _controllers.Contains(controller);
        }
    }
}