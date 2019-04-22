using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UI.Components;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UnrealLoads
{
	public class UnrealLoadsComponent : LogicComponent
	{
		public override string ComponentName => "UnrealLoads";

		public UnrealLoadsSettings Settings { get; set; }

		TimerModel _timer;
		GameMemory _gameMemory;
		LiveSplitState _state;
		HashSet<string> _splitHistory;

		public UnrealLoadsComponent(LiveSplitState state)
		{
			bool debug = false;
#if DEBUG
			debug = true;
#endif
			Trace.WriteLine("[NoLoads] Using LiveSplit.UnrealLoads component version " + Assembly.GetExecutingAssembly().GetName().Version + " " + ((debug) ? "Debug" : "Release") + " build");

			_state = state;
			_timer = new TimerModel { CurrentState = state };
			_splitHistory = new HashSet<string>();

			Settings = new UnrealLoadsSettings(_state);

			_state.OnStart += _state_OnStart;

			_gameMemory = new GameMemory();
			_gameMemory.OnReset += gameMemory_OnReset;
			_gameMemory.OnStart += gameMemory_OnStart;
			_gameMemory.OnSplit += _gameMemory_OnSplit;
			_gameMemory.OnLoadStarted += gameMemory_OnLoadStarted;
			_gameMemory.OnLoadEnded += gameMemory_OnLoadEnded;
			_gameMemory.OnMapChange += _gameMemory_OnMapChange;

			_gameMemory.StartMonitoring();
		}

		void _state_OnStart(object sender, EventArgs e)
		{
			_timer.InitializeGameTime();
			_splitHistory.Clear();
		}

		void _gameMemory_OnSplit(object sender, EventArgs e)
		{
			_timer.Split();
		}

		void _gameMemory_OnMapChange(object sender, string prevMap, string nextMap)
		{
			if (Settings.AutoSplitOnMapChange)
			{
				prevMap = prevMap.ToLower();
				nextMap = nextMap.ToLower();

				var shouldSplit = false;
				if (Settings.Maps.Count == 0)
					shouldSplit = true;
				else
				{
					var leaveMap = Settings.Maps
						.FirstOrDefault(map => map.SplitOnLeave && string.Equals(map.Name, prevMap, StringComparison.OrdinalIgnoreCase));

					var enterMap = Settings.Maps
						.FirstOrDefault(map => map.SplitOnEnter && string.Equals(map.Name, nextMap, StringComparison.OrdinalIgnoreCase));

					if (leaveMap != null && ShouldSplitMap(leaveMap.Name))
					{
						shouldSplit = true;
						_splitHistory.Add(prevMap);
					}

					if (enterMap != null && ShouldSplitMap(enterMap.Name))
					{
						shouldSplit = true;
						_splitHistory.Add(nextMap);
					}
				}

				if (shouldSplit)
				{
					_timer.Split();
				}
			}

#if DEBUG
			if (Settings.DbgShowMap)
				MessageBox.Show(_state.Form, prevMap + " -> " + nextMap, "LiveSplit.UnrealLoads",
					MessageBoxButtons.OK, MessageBoxIcon.Information);
#endif
		}

		bool ShouldSplitMap(string mapName)
		{
			return !Settings.AutoSplitOncePerMap || !_splitHistory.Contains(mapName, StringComparer.OrdinalIgnoreCase);
		}

		public override void Dispose()
		{
			_gameMemory?.Stop();
			_state.OnStart -= _state_OnStart;
		}

		void gameMemory_OnReset(object sender, EventArgs e)
		{
			if (Settings.AutoReset)
			{
				_timer.Reset();
			}
		}

		void gameMemory_OnStart(object sender, EventArgs e)
		{
			if (_state.CurrentPhase == TimerPhase.NotRunning && Settings.AutoStart)
			{
				_timer.Start();
			}
		}

		void gameMemory_OnLoadStarted(object sender, EventArgs e)
		{
			_state.IsGameTimePaused = true;
		}

		void gameMemory_OnLoadEnded(object sender, EventArgs e)
		{
			_state.IsGameTimePaused = false;
		}

		public override XmlNode GetSettings(XmlDocument document) => Settings.GetSettings(document);

		public override Control GetSettingsControl(LayoutMode mode) => Settings;

		public override void SetSettings(XmlNode settings) => Settings.SetSettings(settings);

		public override void Update(IInvalidator invalidator, LiveSplitState state, float width, float height, LayoutMode mode) { }
	}
}
