using LiveSplit.ComponentUtil;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace LiveSplit.UnrealLoads.Games
{
	class MobileForces : GameSupport
	{
		public override HashSet<string> GameNames => new HashSet<string>
		{
			"Mobile Forces",
			"MobileForces"
		};

		public override HashSet<string> ProcessNames => new HashSet<string>
		{
			"mobileforces"
		};

		StringWatcher _map;

		public override HashSet<string> Maps => new HashSet<string>
		{
			"mf-airport",
			"mf-carpark",
			"mf-dockyard",
			"mf-ghetto",
			"mf-hydroworks",
			"mf-polar",
			"mf-rail_quarry",
			"mf-sawmill",
			"mf-warehouse",
			"mf-waterfront",
			"mf-western"
		};

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			var status = (MemoryWatcher<int>)watchers["status"];
			_map = (StringWatcher)watchers["map"];



			if (status.Current == (int)Status.LoadingMap)
			{
				if (_map.Current.ToLower() == "mf-warehouse")
					return new TimerAction[] { TimerAction.Start };
			}

			return null;
		}
	}
}
