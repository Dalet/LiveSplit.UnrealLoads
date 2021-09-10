using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using System;
using System.Diagnostics;

namespace LiveSplit.UnrealLoads.Games
{
	class TheHeatOfWar : GameSupport
	{
		public override HashSet<string> GameNames { get; } = new HashSet<string>
		{
			"THOW",
			"The Heat Of War",
			"WWII Combat - Iwo Jima",
			"Iwo Jima",
			"WWII Combat"
		};

		public override HashSet<string> ProcessNames { get; } = new HashSet<string>
		{
			"iwo"
		};

		public override HashSet<string> Maps { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"B_01_Map",
			"B_02_Map",
			"B_03_Map",
			"B_04_Map",
			"B_05_Map",
			"B_06_Map",
			"B_07_Map",
			"B_08_Map",
			"B_09_Map",
			"B_10_Map"
		};

		
//		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
//		{
//			var map = (StringWatcher)watchers["map"];
//
//			if (map != null)
//			{
//				if (map.Current.Equals("B_01_Map.cmf", StringComparison.OrdinalIgnoreCase))
//				{
//					return new TimerAction[] { TimerAction.Start };
//				}
//			}	
//
//			return null;
//		}
	}
}

