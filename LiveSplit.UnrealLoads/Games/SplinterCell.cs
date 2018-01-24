using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace LiveSplit.UnrealLoads.Games
{
	class SplinterCell : GameSupport
	{
		public override HashSet<string> GameNames => new HashSet<string>
		{
			"Splinter Cell",
		};

		public override HashSet<string> ProcessNames => new HashSet<string>
		{
			"splintercell"
		};

		public override HashSet<string> Maps => new HashSet<string>
		{
			"0_0_2_training",
			"0_0_3_training",
			"1_1_0tbilisi",
			"1_1_1tbilisi",
			"1_1_2tbilisi",
			"1_2_1defenseministry",
			"1_2_2defenseministry",
			"1_3_2caspianoilrefinery",
			"1_3_3caspianoilrefinery",
			"2_1_0cia",
			"2_1_1cia",
			"2_1_2cia",
			"2_2_1_kalinatek",
			"2_2_2_kalinatek",
			"2_2_3_kalinatek",
			"4_1_1chineseembassy",
			"4_1_2chineseembassy",
			"4_2_1_abattoir",
			"4_2_2_abattoir",
			"4_3_0chineseembassy",
			"4_3_1chineseembassy",
			"4_3_2chineseembassy",
			"5_1_1_presidentialpalace",
			"5_1_2_presidentialpalace"
		};

		public override Type LoadMapDetourT => typeof(LoadMapDetour_SplinterCell);

		public override Type SaveGameDetourT => typeof(SaveGameDetour_SplinterCell);
	}

	class LoadMapDetour_SplinterCell : LoadMapDetour
	{
		public new static string Symbol => "?LoadMap@UGameEngine@@UAEPAVULevel@@ABVFURL@@AAVFString@@@Z";

		public LoadMapDetour_SplinterCell(IntPtr setMapAddr, IntPtr statusAddr)
			: base(setMapAddr, statusAddr)
		{
			_overwrittenBytes = 6;
		}

		public override byte[] GetBytes()
		{
			var status = _statusPtr.ToBytes().ToHex();
			var none = Status.None.ToBytes().ToHex();
			var loadingMap = Status.LoadingMap.ToBytes().ToHex();

			var str = string.Join("\n",
				"55",                           // push ebp
				"8B EC",                        // mov ebp,esp
				"83 EC 10",                     // sub esp,10
				"89 55 F0",                     // mov dword ptr ds:[ebp-10],edx
				"89 4D F8",                     // mov dword ptr ds:[ebp-8],ecx
				"8B 45 08",                     // mov eax,dword ptr ds:[ebp+8]
				"8B 48 1C",                     // mov ecx,dword ptr ds:[eax+1C]
				"89 4D FC",                     // mov dword ptr ds:[ebp-4],ecx
				"8B 55 FC",                     // mov edx,dword ptr ds:[ebp-4]
				"52",                           // push edx
				"#FF FF FF FF FF",              // call <hooks.?set_map@@YAXPB_W@Z>
				"83 C4 04",                     // add esp,4
				"C7 05 " + status + loadingMap, // mov dword ptr ds:[<?g_status@@3HA>],1
				"8B 45 0C",                     // mov eax,dword ptr ds:[ebp+C]
				"50",                           // push eax
				"8B 4D 08",                     // mov ecx,dword ptr ds:[ebp+8]
				"51",                           // push ecx
				"8B 4D F8",                     // mov ecx,dword ptr ds:[ebp-8]
				"#FF FF FF FF FF",              // call dword ptr ds:[BF3788]
				"89 45 F4",                     // mov dword ptr ds:[ebp-C],eax
				"C7 05 " + status + none,       // mov dword ptr ds:[<?g_status@@3HA>],0
				"8B 45 F4",                     // mov eax,dword ptr ds:[ebp-C]
				"8B E5",                        // mov esp,ebp
				"5D",                           // pop ebp
				"C2 08 00"                      // ret 8
			);

			int[] offsets;
			var bytes = Utils.ParseBytes(str, out offsets);
			_setMapCallOffset = offsets[0];
			_originalFuncCallOffset = offsets[1];

			return bytes.ToArray();
		}
	}

	class SaveGameDetour_SplinterCell : SaveGameDetour
	{
		public new static string Symbol => "?SaveGame@UGameEngine@@UAEHPBG@Z";

		public SaveGameDetour_SplinterCell(IntPtr statusAddr)
			: base(statusAddr)
		{
			_overwrittenBytes = 8;
		}

		public override byte[] GetBytes()
		{
			var status = _statusPtr.ToBytes().ToHex();
			var none = Status.None.ToBytes().ToHex();
			var saving = Status.Saving.ToBytes().ToHex();

			var str = string.Join("\n",
				"55",                           // push ebp
				"8B EC",                        // mov ebp,esp
				"83 EC 0C",                     // sub esp,C
				"89 55 F4",                     // mov dword ptr ds:[ebp-C],edx
				"89 4D FC",                     // mov dword ptr ds:[ebp-4],ecx
				"C7 05 " + status + saving,     // mov dword ptr ds:[<?g_status@@3HA>],2
				"8B 45 08",                     // mov eax,dword ptr ds:[ebp+8]
				"50",                           // push eax
				"8B 4D FC",                     // mov ecx,dword ptr ds:[ebp-4]
				"#FF FF FF FF FF",              // call dword ptr ds:[BF3790]
				"89 45 F8",                     // mov dword ptr ds:[ebp-8],eax
				"C7 05 " + status + none,       // mov dword ptr ds:[<?g_status@@3HA>],0
				"8B 45 F8",                     // mov eax,dword ptr ds:[ebp-8]
				"8B E5",                        // mov esp,ebp
				"5D",                           // pop ebp
				"C2 04 00"                      // ret 4
			);

			int[] offsets;
			var bytes = Utils.ParseBytes(str, out offsets);
			_originalFuncCallOffset = offsets[0];
			return bytes.ToArray();
		}
	}
}
