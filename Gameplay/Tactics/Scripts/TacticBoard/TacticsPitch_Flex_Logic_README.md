pitch - justify between
zonesContainer: 1 and 6 - justify center
zonesContainer: 2 - 4 - justify between
zone: flex shrink (so they don't try to occupy freed space)

pitch > positionZonesContainer (16.6%) > positionZone (20%) > PlayerItem (w65 h46)

OnDrag -> Show all usable fields
OnDrop -> Hide all except those that are in formation

public void ShowUsablePlayerItemViews()
{
// using( new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2() )
// {
for (var i = 0; i < zoneContainerViews.Length; i++)
{
// exclude first zone
// as it only contains one zone/playerItemView that is always available
if (i == 0) continue;
foreach (var zoneView in zoneContainerViews[i].ZoneViews)
{
if(zoneView == null) continue;
// zoneView.gameObject.SetActive(true);
// zoneView.childPlayerItemView.gameObject.SetActive(true);
zoneView.Show();
zoneView.childPlayerItemView.Show();

				if (!zoneView.InUseForFormation || !zoneView.childPlayerItemView.InUseForFormation)
				{
					zoneView.childPlayerItemView.mainView.gameObject.SetActive(false);
					zoneView.childPlayerItemView.placeholderView.gameObject.SetActive(true);
					// zoneView.childPlayerItemView.mainView.Hide();
					// zoneView.childPlayerItemView.placeholderView.Show();
				}
			}
		}
	// }
}

// public static void HideUnusedPlayerItemViews(PositionZonesContainerView[] zoneContainerViews)
public void HideUnusedPlayerItemViews()
{
// using (new NinjaTools.FlexBuilder.LayoutAlgorithms.ExperimentalDelayUpdates2())
// {
for (var i = 0; i < zoneContainerViews.Length; i++)
{
// Exclude the first zone as it only contains one zone/playerItemView that is always available
if (i == 0) continue;

			var zoneViews = zoneContainerViews[i].ZoneViews;

			if (i >= 1 && i <= 4) // For 2nd to 5th containers (index 1 to 4)
			{
				if (zoneViews.Length >= 3) // Ensure there is a 3rd child zone
				{
					var thirdZoneView = zoneViews[2];
					if (thirdZoneView != null && !thirdZoneView.childPlayerItemView.InUseForFormation)
					{
						// thirdZoneView.gameObject.SetActive(false);
						thirdZoneView.Hide();
					}
					else
					{
						// thirdZoneView.gameObject.SetActive(true);
						thirdZoneView.Show();
					}
				}

				// Hide only the childPlayerItemView for other zones if they are not in formation
				for (int j = 0; j < zoneViews.Length; j++)
				{
					if (j != 2) // Skip the 3rd zone here as it's handled above
					{
						var zoneView = zoneViews[j];
						if (zoneView != null && !zoneView.childPlayerItemView.InUseForFormation)
						{
							// zoneView.childPlayerItemView.gameObject.SetActive(false);
							zoneView.childPlayerItemView.Hide();
						}
					}
				}
			}
			else if (i == zoneContainerViews.Length - 1) // For the last container
			{
				foreach (var zoneView in zoneViews)
				{
					if (zoneView != null && !zoneView.childPlayerItemView.InUseForFormation)
					{
						// zoneView.childPlayerItemView.gameObject.SetActive(false);
						zoneView.childPlayerItemView.Hide();
					}
				}
			}
		}
	// }
}