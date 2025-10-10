using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UIWidgets.Examples;
using UltimateFootballSystem.Core.Tactics;
using UltimateFootballSystem.Gameplay.Tactics;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
	public class RoleSelector : MonoBehaviour
	{
		[SerializeField] protected ListViewEnum RoleListView;
		[SerializeField] protected ListViewEnum DutyListView;

		[SerializeField] protected TacticBoardController Controller;

		ListViewEnum<AdditionalCanvasShaderChannels> WrapperWithFlags;

		ListViewEnum<TacticalRoleOption> RoleListViewWrapper;
		ListViewEnum<TacticalDutyOption> DutyListViewWrapper;

		void RoleValueChanged(int index)
		{
			if (WrapperWithFlags != null)
			{
				Debug.Log(string.Format("selected: {0}",
					EnumHelper<AdditionalCanvasShaderChannels>.Instance.ToString(WrapperWithFlags.Selected)));
			}
			else if (RoleListViewWrapper != null)
			{
				Debug.Log(string.Format("selected: {0}",
					EnumHelper<TacticalRoleOption>.Instance.ToString(RoleListViewWrapper.Selected)));

				Controller.SelectedPlayerItemView.TacticalPosition.SetRoleOption(RoleListViewWrapper.Selected);
				ShowDutyType();

				// Trigger role changed event
				Controller.SelectedPlayerItemView.TriggerRoleChanged(RoleListViewWrapper.Selected);
			}
		}
		
		void DutyValueChanged(int index)
		{
			if (WrapperWithFlags != null)
			{
				Debug.Log(string.Format("selected: {0}",
					EnumHelper<AdditionalCanvasShaderChannels>.Instance.ToString(WrapperWithFlags.Selected)));
			}
			else if (DutyListViewWrapper != null)
			{
				Debug.Log(string.Format("selected: {0}",
					EnumHelper<TacticalDutyOption>.Instance.ToString(DutyListViewWrapper.Selected)));

				// Controller.SelectedPlayerItemView.TacticalPosition.SetSelectedDuty(DutyListViewWrapper.Selected);
				// Controller.SelectedPlayerItemView.TacticalPosition.SetDutyOption(DutyListViewWrapper.Selected);
				Controller.SelectedPlayerItemView.TacticalPosition.SelectedRole.SetSelectedDuty(DutyListViewWrapper.Selected);

				// Trigger duty changed event
				Controller.SelectedPlayerItemView.TriggerDutyChanged(DutyListViewWrapper.Selected);
			}
		}

		void DeleteRoleWrapper()
		{
			RoleListViewWrapper = null;
			WrapperWithFlags = null;
		}
		
		void DeleteDutyWrapper()
		{
			DutyListViewWrapper = null;
			WrapperWithFlags = null;
		}

		void DeleteWrappers()
		{
			DeleteRoleWrapper();
			DeleteDutyWrapper();
		}

		private void Awake()
		{
			// CleanupDialog();
			// throw new NotImplementedException();
		}
		/// <summary>
		/// Show AdditionalCanvasShaderChannels.
		/// </summary>
		// public void ShowCanvasChannels()
		// {
		// 	DeleteWrappers();
		// 	WrapperWithFlags = ListView.UseEnum<AdditionalCanvasShaderChannels>(false, x => (AdditionalCanvasShaderChannels)x);
		// }

		/// <summary>
		/// Select AdditionalCanvasShaderChannels.
		/// </summary>
		// public void SelectCanvasChannels()
		// {
		// 	if (WrapperWithFlags != null)
		// 	{
		// 		WrapperWithFlags.Selected = AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.TexCoord1;
		// 	}
		// }

		/// <summary>
		/// Show RoleType.
		/// </summary>
		public void ShowRoleType()
		{
			if (Controller?.SelectedPlayerItemView?.TacticalPosition == null)
			{
				Debug.LogWarning("RoleSelector: Cannot show roles - missing data");
				return;
			}
			
			// DeleteRoleWrapper();
			// RoleListView.ResetInstanceSize(0);
			// RoleListView.UpdateView();

			var pos = Controller.SelectedPlayerItemView.TacticalPosition;
			var availableRoles = pos.AvailableRoles;

			if (availableRoles == null || availableRoles.Count == 0)
			{
				Debug.LogWarning($"RoleSelector: No available roles for position {pos.Position}");
				return;
			}

			var availableRoleOptions = availableRoles.Select(r => r.RoleOption).ToList();
			Debug.Log($"RoleSelector: Showing {availableRoleOptions.Count} roles for position {pos.Position}");

			RoleListViewWrapper = new ListViewEnum<TacticalRoleOption>(RoleListView, availableRoleOptions);
			SelectRoleType();
		}
		
		/// <summary>
		/// Show Duty Type.
		/// </summary>
		public void ShowDutyType()
		{
			if (Controller?.SelectedPlayerItemView?.TacticalPosition?.SelectedRole == null)
			{
				Debug.LogWarning("RoleSelector: Cannot show duties - missing selected role");
				return;
			}

			DeleteDutyWrapper();
			DutyListView.ResetInstanceSize(0);
			DutyListView.UpdateView();

			var selectedRole = Controller.SelectedPlayerItemView.TacticalPosition.SelectedRole;
			var availableDuties = selectedRole.AvailableDuties;

			if (availableDuties == null || availableDuties.Count == 0)
			{
				Debug.LogWarning($"RoleSelector: No available duties for role {selectedRole.RoleName}");
				return;
			}

			Debug.Log($"RoleSelector: Showing {availableDuties.Count} duties for role {selectedRole.RoleName}");
			DutyListViewWrapper = new ListViewEnum<TacticalDutyOption>(DutyListView, availableDuties);
			SelectDutyType();
		}

		/// <summary>
		/// Select RoleType - Default.
		/// </summary>
		public void SelectRoleType()
		{
			if (RoleListViewWrapper != null)
			{
				RoleListViewWrapper.Selected = Controller.SelectedPlayerItemView.TacticalPosition.SelectedRole?.RoleOption ?? default;
			}
		}
		
		/// <summary>
		/// Select DutyType - Default.
		/// </summary>
		public void SelectDutyType()
		{
			if (DutyListViewWrapper != null)
			{
				DutyListViewWrapper.Selected = Controller.SelectedPlayerItemView.TacticalPosition.SelectedRole.SelectedDuty;
			}
		}

		// private void OnEnable()
		// {
		// 	// Initialize when enabled
		// 	if (Controller?.SelectedPlayerItemView?.TacticalPosition != null)
		// 	{
		// 		ShowRoleType();
		// 		SelectRoleType();
		// 		ShowDutyType();
		// 		SelectDutyType();
		// 	}
		// }
		
		// private void OnDisable()
		// {
		// 	Debug.Log("RoleSelector: OnDisable");
			// CleanupDialog();
			// var roleListViewsItems = RoleListView.Container.GetComponentsInChildren<GameObject>();
			// foreach (var item in roleListViewsItems)
			// {
			// 	Destroy(item);
			// }
		// }
		
		/// <summary>
		/// Clean up all list views and wrappers
		/// </summary>
		// private void CleanupDialog()
		// public void CleanupDialog()
		// {
		// 	DeleteWrappers();
		// 	RoleListView.ResetInstanceSize(0);
		// 	RoleListView.UpdateView();
		// 	DutyListView.ResetInstanceSize(0);
		// 	DutyListView.UpdateView();
		// }

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			if (RoleListView == null)
			{
				Debug.LogError("RoleListView is null");
			}
			if (DutyListView == null)
			{
				Debug.LogError("DutyListView is null");
			}
			
			ShowRoleType();
			ShowDutyType();
			
			// Role ListView
			RoleListView.OnSelectObject.AddListener(RoleValueChanged);
			RoleListView.OnDeselectObject.AddListener(RoleValueChanged);
			// Duty ListView
			DutyListView.OnSelectObject.AddListener(DutyValueChanged);
			DutyListView.OnDeselectObject.AddListener(DutyValueChanged);

		}
		
		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (RoleListView != null)
			{
				RoleListView.OnSelectObject.RemoveListener(RoleValueChanged);
				RoleListView.OnDeselectObject.RemoveListener(RoleValueChanged);
			}
			
			if (DutyListView != null)
			{
				DutyListView.OnSelectObject.RemoveListener(DutyValueChanged);
				DutyListView.OnDeselectObject.RemoveListener(DutyValueChanged);
			}
		}
	}
}
