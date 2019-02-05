using LiveSplit.ComponentUtil;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace LiveSplit.UnrealLoads.Games
{
	public enum IdentificationResult
	{
		Success,
		Failure,
		Undecisive
	}

	public enum TimerAction
	{
		DoNothing,
		Start,
		Reset,
		Split,
		PauseGameTime,
		UnpauseGameTime
	}

	abstract class GameSupport
	{
		public abstract HashSet<string> GameNames { get; }

		public abstract HashSet<string> ProcessNames { get; }

		public virtual string MapExtension { get; } = null;

		public virtual HashSet<string> Maps { get; } = new HashSet<string>();

		public virtual LoadMapDetour GetNewLoadMapDetour() => new LoadMapDetour();

		public virtual SaveGameDetour GetNewSaveGameDetour() => new SaveGameDetour();

		public string[] GetHookModules()
		{
			var list = new List<string>();

			var loadMap = GetNewLoadMapDetour();
			if (loadMap != null)
			{
				if (!string.IsNullOrEmpty(loadMap.Module))
					list.Add(loadMap.Module);
			}

			var saveGame = GetNewSaveGameDetour();
			if (saveGame != null)
			{
				if (!string.IsNullOrEmpty(saveGame.Module))
					list.Add(saveGame.Module);
			}

			return list.ToArray();
		}

		public virtual IdentificationResult IdentifyProcess(Process process) => IdentificationResult.Success;

		public virtual TimerAction[] OnUpdate(Process game, MemoryWatcherList watchers) => null;

		public virtual TimerAction[] OnMapLoad(MemoryWatcherList watchers) => null;

		public virtual bool? IsLoading(MemoryWatcherList watchers) => null;

		public virtual TimerAction[] OnAttach(Process game) => null;

		public virtual TimerAction[] OnDetach(Process game) => new TimerAction[] { TimerAction.PauseGameTime };
	}

}
