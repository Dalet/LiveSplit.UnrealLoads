using LiveSplit.ComponentUtil;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace LiveSplit.UnrealLoads.Games
{
	class XCOM_Enforcer : GameSupport
	{
		public override HashSet<string> GameNames => new HashSet<string>
		{
			"XCOM Enforcer",
			"X-COM Enforcer",
			"XCOM: Enforcer",
			"X-COM: Enforcer"
		};

		public override HashSet<string> ProcessNames => new HashSet<string>
		{
			"xcom"
		};

		StringWatcher _map;

		public override HashSet<string> Maps => new HashSet<string>
		{
			"..\\maps\\bonus01",
			"..\\maps\\bonus02",
			"..\\maps\\bonus03",
			"..\\maps\\bonus04",
			"..\\maps\\bonus05",
			"..\\maps\\map00",
			"..\\maps\\map01",
			"..\\maps\\map02",
			"..\\maps\\map03",
			"..\\maps\\map04",
			"..\\maps\\map05",
			"..\\maps\\map06",
			"..\\maps\\map07",
			"..\\maps\\map08",
			"..\\maps\\map09",
			"..\\maps\\map10",
			"..\\maps\\map11",
			"..\\maps\\map12",
			"..\\maps\\map13",
			"..\\maps\\map14",
			"..\\maps\\map15",
			"..\\maps\\map16",
			"..\\maps\\map17",
			"..\\maps\\map18",
			"..\\maps\\map19",
			"..\\maps\\map20",
			"..\\maps\\map21",
			"..\\maps\\map22",
			"..\\maps\\map23",
			"..\\maps\\map24",
			"..\\maps\\map25",
			"..\\maps\\map26",
			"..\\maps\\map27",
			"..\\maps\\map28",
			"..\\maps\\map29",
			"..\\maps\\map30",
			"..\\maps\\map31",
			"..\\maps\\map32",
			"..\\maps\\map33",
			"..\\maps\\map34",
			"..\\maps\\map35",
			"..\\maps\\map36",
			"..\\maps\\map37",
			"..\\maps\\map38",
			"..\\maps\\map39",
			"..\\maps\\map40"
		};

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			var status = (MemoryWatcher<int>)watchers["status"];
			_map = (StringWatcher)watchers["map"];



			if (status.Current == (int)Status.LoadingMap)
			{
				Debug.WriteLine("Wat is this:" + _map.Current);

				if (_map.Current.ToLower() == "..\\maps\\map00")
					return new TimerAction[] { TimerAction.Start };
			}

			return null;
		}
	}
}
