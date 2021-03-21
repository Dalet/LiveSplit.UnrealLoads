﻿using System;
using System.Collections.Generic;
using LiveSplit.ComponentUtil;
using System.Diagnostics;
using System.IO;
using System.Text;


namespace LiveSplit.UnrealLoads.Games
{
	class DS9TheFallen : GameSupport
	{
		public override HashSet<string> GameNames { get; } = new HashSet<string>
		{
			"Star Trek: Deep Space Nine: The Fallen",
			"Deep Space Nine The Fallen"
		};

		public override HashSet<string> ProcessNames { get; } = new HashSet<string>
		{
			"ds9"
		};

		public override HashSet<string> Maps { get; } = new HashSet<string>
		{
			"cm01a_sw",
			"cm01b_sw",
			"cm04_lab",
			"cm05_kira",
			"cm06_sw_capture",
			"cm10_kiral1a",
			"cm10_swk_l3",
			"cm11b_swk",
			"cm11_labritual",
			"cm_ardprison",
			"cm_defiant",
			"cm_endcredits",
			"cm_globalspace",
			"cm_spacestation",
			"cm_worf_endgame",
			"ds9_ops",
			"ds9_promenade",
			"gcm11end_wraithobk",
			"m01_kiral1a",
			"m01_kiral1b",
			"m01_kiral2",
			"m01_siskol1a",
			"m01_siskol1b",
			"m01_worfl1",
			"m03_kiral1a",
			"m03_kiral1b",
			"m03_kiral2",
			"m03_siskol1a",
			"m03_siskol1b",
			"m03_siskol2",
			"m03_worfl1",
			"m03_worfl2",
			"m04_kiral1",
			"m04_siskol1",
			"m04_worfl1a",
			"m04_worfl1b",
			"m05_kiral1",
			"m05_siskol1a",
			"m05_siskol1b",
			"m05_siskol2",
			"m05_worfl1",
			"m05_worfl2",
			"m06_siskol1a",
			"m06_siskol1b",
			"m06_siskol1c",
			"m06_worfl1a",
			"m06_worfl1b",
			"m06_worfl1c",
			"m07_kiral1",
			"m07_kiral2",
			"m07_siskol1a",
			"m07_siskol1b",
			"m07_siskol2",
			"m07_siskol3a",
			"m07_siskol3b",
			"m07_worfl1",
			"m07_worfl2a",
			"m07_worfl2b",
			"m07_worfl3",
			"m10_kiral1b",
			"m10_kiral2",
			"m10_siskol1a",
			"m10_siskol1b",
			"m10_siskol2",
			"m10_worfl1a",
			"m10_worfl1b",
			"m10_worfl2",
			"m11_kiral1",
			"m11_siskol1a",
			"m11_siskol1b",
			"m11_worfl1"
		};

		StringWatcher _map;

		public override LoadMapDetour GetNewLoadMapDetour() => new LoadMapDetour_DS9TheFallen();

		public override SaveGameDetour GetNewSaveGameDetour() => new SaveGameDetour_DS9TheFallen();

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			_map = (StringWatcher)watchers["map"];


			if(_map.Current.ToLower() == "cm01a_sw" || _map.Current.ToLower() == "cm_globalspace")
				return new TimerAction[] { TimerAction.Reset };
			if(_map.Old.ToLower() == "cm01a_sw" && _map.Current.ToLower() == "m01_siskol1a")
				return new TimerAction[] { TimerAction.Start };
			else if(_map.Old.ToLower() == "cm_globalspace" && _map.Current.ToLower() == "m01_kiral1a")
				return new TimerAction[] { TimerAction.Start };
			else if(_map.Old.ToLower() == "cm01a_sw" && _map.Current.ToLower() == "m01_worfl1")
				return new TimerAction[] { TimerAction.Start };


			return null;
		}
	}

	class LoadMapDetour_DS9TheFallen : LoadMapDetour
	{
		public override StringType Encoding => StringType.ASCII;
		protected override int OverwrittenBytes => 8;
	}

	class SaveGameDetour_DS9TheFallen : SaveGameDetour
	{
		public override string Symbol => "?SaveGame@UGameEngine@@UAEXPBD@Z";
	}
}
