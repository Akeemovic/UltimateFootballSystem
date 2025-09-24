using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIWidgets;
using UIWidgets.Examples;
using UltimateFootballSystem.Core.TacticsEngine;
using UltimateFootballSystem.Gameplay.Tactics;
using UnityEngine;

namespace UltimateFootballSystem.Gameplay.Tactics
{
	public class TacticalRoleSelector : MonoBehaviour
	{
		[SerializeField] protected ListViewEnum RoleListView;
		[SerializeField] protected ListViewEnum DutyListView;

		[SerializeField] protected TacticsBoardController Controller;

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
			}
			
			Controller.SelectedPlayerItemView.TacticalPosition.SetRole(RoleListViewWrapper.Selected);
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
			}
			
			Controller.SelectedPlayerItemView.TacticalPosition.SetSelectedDuty(DutyListViewWrapper.Selected);
		}

		void DeleteWrappers()
		{
			RoleListViewWrapper = null;
			WrapperWithFlags = null;
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
			DeleteWrappers();
			// Wrapper = ListView.UseEnum<TacticalRoleOption>(false, x => (TacticalRoleOption)x);
			var availableRoleOptions = Controller.SelectedPlayerItemView.TacticalPosition.AvailableRoles
				.Select(r => r.RoleOption).ToList();
			RoleListViewWrapper = new ListViewEnum<TacticalRoleOption>(RoleListView, availableRoleOptions);
		}
		
		/// <summary>
		/// Show Duty Type.
		/// </summary>
		public void ShowDutyType()
		{
			DeleteWrappers();
			// Wrapper = ListView.UseEnum<TacticalRoleOption>(false, x => (TacticalRoleOption)x);
			DutyListViewWrapper = new ListViewEnum<TacticalDutyOption>(DutyListView,
				Controller.SelectedPlayerItemView.TacticalPosition.SelectedRole.AvailableDuties);
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
		
		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			if (RoleListView != null)
			{
				Debug.LogError("RoleListView is null");
			}
			if (DutyListView != null)
			{
				Debug.LogError("DutyListView is null");
			}
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
