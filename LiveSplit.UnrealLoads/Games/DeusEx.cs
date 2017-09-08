using LiveSplit.ComponentUtil;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace LiveSplit.UnrealLoads.Games
{
	class DeusEx : GameSupport
	{
		public override HashSet<string> GameNames => new HashSet<string>
		{
			"DeusEx",
			"Deus Ex"
		};

		public override HashSet<string> ProcessNames => new HashSet<string>
		{
			"deusex"
		};

		public override Type SaveGameDetourT => typeof(SaveGameDetour_DeusEx);

		StringWatcher _map;

		public override HashSet<string> Maps => new HashSet<string>
		{
			"01_nyc_unatcoisland",
			"01_nyc_unatcohq",
			"02_nyc_batterypark",
			"02_nyc_street",
			"02_nyc_warehouse",
			"02_nyc_bar",
			"02_nyc_freeclinic",
			"02_nyc_hotel",
			"02_nyc_smug",
			"02_nyc_underground",
			"03_nyc_unatcoisland",
			"03_nyc_unatcohq",
			"03_nyc_batterypark",
			"03_nyc_brooklynbridgestation",
			"03_nyc_molepeople",
			"03_nyc_airfieldhelibase",
			"03_nyc_airfield",
			"03_nyc_hangar",
			"03_nyc_airfield",
			"04_nyc_unatcoisland",
			"04_nyc_unatcohq",
			"04_nyc_street",
			"04_nyc_hotel",
			"04_nyc_bar",
			"04_nyc_batterypark",
			"04_nyc_nsfhq",
			"04_nyc_smug",
			"04_nyc_underground",
			"05_nyc_unatcomj12lab",
			"05_nyc_unatcohq",
			"05_nyc_unatcoisland",
			"06_hongkong_helibase",
			"06_hongkong_wanchai_market",
			"06_hongkong_versalife",
			"06_hongkong_mj12lab",
			"06_hongkong_storage",
			"06_hongkong_wanchai_canal",
			"06_hongkong_tongbase",
			"06_hongkong_wanchai_garage",
			"06_hongkong_wanchai_street",
			"06_hongkong_wanchai_underworld",
			"08_nyc_street",
			"08_nyc_bar",
			"08_nyc_freeclinic",
			"08_nyc_hotel",
			"08_nyc_smug",
			"08_nyc_underground",
			"09_nyc_dockyard",
			"09_nyc_shipfan",
			"09_nyc_ship",
			"09_nyc_shipbelow",
			"09_nyc_graveyard",
			"10_paris_catacombs",
			"10_paris_catacombs_tunnels",
			"10_paris_metro",
			"10_paris_club",
			"10_paris_metro",
			"10_paris_chateau",
			"11_paris_cathedral",
			"11_paris_underground",
			"11_paris_everett",
			"12_vandenberg_cmd",
			"12_vandenberg_gas",
			"14_vandenberg_sub",
			"12_vandenberg_computer",
			"12_vandenberg_tunnels",
			"14_oceanlab_lab",
			"14_oceanlab_uc",
			"14_oceanlab_silo",
			"15_area51_bunker",
			"15_area51_entrance",
			"15_area51_final",
			"15_area51_page",
			"99_endgame1",
			"99_endgame2",
			"99_endgame3",
			"99_endgame4"
		};

		public override TimerAction[] OnMapLoad(MemoryWatcherList watchers)
		{
			var status = (MemoryWatcher<int>)watchers["status"];
			_map = (StringWatcher)watchers["map"];



			if (status.Current == (int)Status.LoadingMap)
			{
				if (_map.Current.ToLower() == "00_intro")
					return new TimerAction[] { TimerAction.Reset };
				else if (_map.Current.ToLower() == "01_nyc_unatcoisland")
					return new TimerAction[] { TimerAction.Start };
			}


			return null;
		}

		class SaveGameDetour_DeusEx : SaveGameDetour
		{
			public new static string Symbol => "?SaveCurrentLevel@DDeusExGameEngine@@QAEXH_N@Z";
			public new static string Module => "DeusEx.dll";

			public SaveGameDetour_DeusEx(IntPtr statusAddr) : base(statusAddr)
			{
				_statusPtr = statusAddr;
			}

			public override byte[] GetBytes()
			{
				var status = _statusPtr.ToBytes().ToHex();
				var none = Status.None.ToBytes().ToHex();
				var saving = Status.Saving.ToBytes().ToHex();


				var str = string.Join("\n",
					"55",                            //push ebp
					"8B EC",                         //mov ebp,esp
					"83 EC 08",                      //sub esp,8
					"89 55 F8",                      //mov dword ptr ss:[ebp-8],edx
					"89 4D FC",                      //mov dword ptr ss:[ebp-4],ecx
					"C7 05 " + status + saving,      //mov dword ptr ds:[<g_status>],2
					"0F B6 45 0C",                   //movzx eax,byte ptr ss:[ebp+C]
					"50",                            //push eax
					"8B 4D 08",                      //mov ecx,dword ptr ss:[ebp+8]
					"51",                            //push ecx
					"8B 4D FC",                      //mov ecx,dword ptr ss:[ebp-4]
					"#FF FF FF FF FF",               //call dword ptr ds:[118378C]
					"C7 05 " + status + none,		 //mov dword ptr ds:[<g_status>],0
					"8B E5",                         //mov esp,ebp
					"5D",                            //pop ebp
					"C2 08 00"                       //ret 8
				);

				int[] offsets;
				var bytes = Utils.ParseBytes(str, out offsets);
				_originalFuncCallOffset = offsets[0];

				return bytes.ToArray();
			}
		}
	}
}
