using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace LiveSplit.UnrealLoads.Games
{
	class HarryPotter2 : GameSupport
	{
		public override HashSet<string> GameNames => new HashSet<string>
		{
			"Harry Potter 2",
			"Harry Potter II",
			"Harry Potter and the Chamber of Secrets",
			"HP2",
			"HP 2",
			"HP II"
		};

		public override HashSet<string> ProcessNames => new HashSet<string>
		{
			"game"
		};

		public override string MapExtension { get; } = ".unr";

		public override HashSet<string> Maps => new HashSet<string>
		{

			"Adv1Willow",
			"Adv3DungeonQuest",
			"Adv4Greenhouse",
			"Adv6Goyle",
			"Adv7SlythComRoom",
			"Adv8Forest",
			"Adv9Aragog",
			"Adv11aCorridor",
			"Adv11bSecrets",
			"Adv12Chamber",
			"Arena",
			"BeanRewardRoom",
			"Ch1Rictusempra",
			"Ch2Skurge",
			"Ch3Diffindo",
			"Ch4Spongify",
			"Ch6WizardCard",
			"Ch7Gryffindor",
			"Credits",
			"Duel01",
			"Duel02",
			"Duel03",
			"Duel04",
			"Duel05",
			"Duel06",
			"Duel07",
			"Duel08",
			"Duel09",
			"Duel10",
			"Entryhall_hub",
			"FlyingFordCutScene",
			"Grandstaircase_hub",
			"GreatHall_G",
			"Grounds_hub",
			"Grounds_Night",
			"PrivetDr",
			"Quidditch",
			"Quidditch_Intro",
			"Sepia_Hallway",
			"Transition"
		};

		MemoryWatcher<bool> _isSkippingCut = new MemoryWatcher<bool>(new DeepPointer("Engine.dll", 0x2E2DFC, 0x5C));
		readonly HashSet<int> _moduleMemorySizes = new HashSet<int>
		{
			704512,
			749568, // US
			674234 //no-cd
		};

		public override IdentificationResult IdentifyProcess(Process process)
		{
			if (_moduleMemorySizes.Contains(process.MainModuleWow64Safe().ModuleMemorySize)
				&& GetCommandLine(process).Contains("-SAVESLOT="))
				return IdentificationResult.Success;

			return IdentificationResult.Failure;
		}

		public override TimerAction[] OnUpdate(Process game, MemoryWatcherList watchers)
		{
			_isSkippingCut.Update(game);
			var map = (StringWatcher)watchers["map"];

			if (_isSkippingCut.Changed && _isSkippingCut.Current
				&& (string.IsNullOrEmpty(map.Old) || map.Current.Equals("privetdr.unr", StringComparison.OrdinalIgnoreCase)))
			{
				return new TimerAction[] { TimerAction.Start };
			}

			return null;
		}

		public override bool? IsLoading(MemoryWatcherList watchers)
		{
			if (_isSkippingCut.Current)
				return true;

			return null;
		}

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			var map = (StringWatcher)watchers["map"];
			//reset only if it is the first map loaded
			if (string.IsNullOrEmpty(map.Old) && map.Current.ToLower() == "privetdr.unr")
				return new TimerAction[] { TimerAction.Reset };

			return null;
		}

		static string GetCommandLine(Process process)
		{
			var commandLine = new StringBuilder();
			using (var searcher = new System.Management.ManagementObjectSearcher(
				"SELECT CommandLine FROM Win32_Process WHERE ProcessId = " + process.Id))
			{
				foreach (var @object in searcher.Get())
					commandLine.Append(@object["CommandLine"] + " ");
			}

			return commandLine.ToString();
		}
	}
}
