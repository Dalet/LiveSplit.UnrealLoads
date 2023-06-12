using LiveSplit.ComponentUtil;
using System.Collections.Generic;
using System.Diagnostics;

namespace LiveSplit.UnrealLoads.Games
{
	class Shrek2 : GameSupport
	{
		public override HashSet<string> GameNames { get; } = new HashSet<string>
		{
			"Shrek 2 (PC)",
			"Shrek 2 PC",
			"Shrek 2: The Game",
			"Shrek 2",
			"S2PC"
		};

		public override HashSet<string> ProcessNames { get; } = new HashSet<string>
		{
			"game"
		};
		
		public override HashSet<string> Maps { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
		{
			"1_shreks_swamp",
			"2_carriage_hijack",
			"3_the_hunt_part1",
			"3_the_hunt_part2",
			"3_the_hunt_part3",
			"3_the_hunt_part4",
			"4_fgm_pib",
			"5_fgm_donkey",
			"6_hamlet",
			"6_hamlet_mine",
			"6_hamlet_end",
			"7_prison_donkey",
			"8_prison_pib",
			"9_prison_shrek",
			"10_castle_siege",
			"11_fgm_battle",
			"beanstalk_bonus",
			"beanstalk_bonus_dawn",
			"beanstalk_bonus_knight",
			"book_story_1",
			"book_story_2",
			"book_story_3",
			"book_story_4",
			"credits",
			"4_fgm_office",
			"book_frontend",
			"book_storybook",
			"sh2_preamble",
			"entry"
		};

		public override IdentificationResult IdentifyProcess(Process process)
		{
			return process.MainModuleWow64Safe().ModuleMemorySize == 438272
				? IdentificationResult.Success
				: IdentificationResult.Failure;
		}

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			var map = (StringWatcher)watchers["map"];
			if (map.Current.ToLower() == "book_story_1.unr")
				return new TimerAction[] { TimerAction.Reset, TimerAction.Start };

			return null;
		}
	}
}