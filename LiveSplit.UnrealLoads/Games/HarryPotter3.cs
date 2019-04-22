using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using System;

namespace LiveSplit.UnrealLoads.Games
{
	class HarryPotter3 : GameSupport
	{
		public override string MapExtension { get; } = ".unr";

		public override HashSet<string> GameNames => new HashSet<string>
		{
			"Harry Potter 3",
			"Harry Potter and the Prisoner of Azkaban",
			"HP3",
			"HP 3"
		};

		public override HashSet<string> ProcessNames => new HashSet<string>
		{
			"hppoa"
		};

		public override HashSet<string> Maps => new HashSet<string>
		{
			"hp3_adv1express",
			"hp3_groundsdada",
			"hp3_ch1carperetractum",
			"hp3_ch1carperetractumb",
			"hp3_insidehub",
			"hp3_groundshub",
			"hp3_comc",
			"hp3_ch2draconifors",
			"hp3_ch2draconiforsb",
			"hp3_quidditch",
			"hp3_adv2exppatronum",
			"hp3_adv3library",
			"hp3_ch3glacius",
			"hp3_ch3glaciusb",
			"hp3_buckyexecuted",
			"hp3_whompingwillow",
			"hp3_adv5shack",
			"hp3_dementorbattle",
			"hp3_adv6paddock",
			"hp3_darktower",
			"hp3_fa1ron",
			"hp3_fa2hermione",
			"hp3_fa3harry",
			"hp3_final",
			"hp3_portraitpassword_b",
			"hp3_portraitpassword_c",
			"hp3_portraitpassword_d",
			"hp3_portraitpassword_e",
			"hp3_portraitpassword_f",
			"hp3_dungeonhub",
			"hp3_beanbonus",
			"hp3_infirmary"
		};

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			var map = (StringWatcher)watchers["map"];

			if (map.Current.Equals("hp3_adv1express.unr", StringComparison.OrdinalIgnoreCase))
				return new TimerAction[] { TimerAction.Start };
			else
				return null;
		}
	}
}

