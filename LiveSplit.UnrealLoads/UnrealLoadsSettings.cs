using LiveSplit.Model;
using LiveSplit.UI;
using LiveSplit.UnrealLoads.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;

namespace LiveSplit.UnrealLoads
{
	public partial class UnrealLoadsSettings : UserControl
	{
		public bool AutoStart { get; set; }
		public bool AutoReset { get; set; }
		public bool AutoSplitOnMapChange { get; set; }
		public bool AutoSplitOncePerMap { get; set; }
		public bool DbgShowMap { get; set; }
		public bool UsePrevMap { get; set; }

		public Dictionary<string, bool> Maps { get; private set; }

		const bool DEFAULT_AUTOSTART = true;
		const bool DEFAULT_AUTORESET = true;
		const bool DEFAULT_AUTOSPLITONMAPCHANGE = false;
		const bool DEFAULT_AUTOSPLITONCEPERMAP = true;
		const bool DEFAULT_USEPREVMAP = false;

		LiveSplitState _state;
		bool _isRefreshingListbox;

		public UnrealLoadsSettings(LiveSplitState state)
		{
			InitializeComponent();

			_state = state;

			Maps = new Dictionary<string, bool>();
			cbGame.DataSource = GameMemory.SupportedGames.Select(s => s.GetType())
				.OrderBy(t => t.Name)
				.ToList();
			cbGame.DisplayMember = "Name";

			chkAutoStart.DataBindings.Add("Checked", this, "AutoStart", false, DataSourceUpdateMode.OnPropertyChanged);
			chkAutoReset.DataBindings.Add("Checked", this, "AutoReset", false, DataSourceUpdateMode.OnPropertyChanged);
			chkSplitOnNewMap.DataBindings.Add("Checked", this, "AutoSplitOnMapChange", false, DataSourceUpdateMode.OnPropertyChanged);
			chkSplitOncePerMap.DataBindings.Add("Checked", this, "AutoSplitOncePerMap", false, DataSourceUpdateMode.OnPropertyChanged);
			chkSplitOncePerMap.DataBindings.Add("Enabled", chkSplitOnNewMap, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
			gbMapWhitelist.DataBindings.Add("Enabled", chkSplitOnNewMap, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
			chkDbgShowMap.DataBindings.Add("Checked", this, "DbgShowMap", false, DataSourceUpdateMode.OnPropertyChanged);
			chkFilterPrevMap.DataBindings.Add("Checked",this,"UsePrevMap",false,DataSourceUpdateMode.OnPropertyChanged);

			// defaults
			AutoStart = DEFAULT_AUTOSTART;
			AutoReset = DEFAULT_AUTORESET;
			AutoSplitOnMapChange = DEFAULT_AUTOSPLITONMAPCHANGE;
			AutoSplitOncePerMap = DEFAULT_AUTOSPLITONCEPERMAP;
			cbGame.SelectedItem = SearchGameSupport(_state.Run.GameName)?.GetType() ?? GameMemory.SupportedGames[0].GetType();
			UsePrevMap = DEFAULT_USEPREVMAP;

#if DEBUG
			chkDbgShowMap.Visible = true;
#endif
		}

		static GameSupport SearchGameSupport(string name)
		{
			name = name.ToLowerInvariant();

			var game = GameMemory.SupportedGames.FirstOrDefault(g =>
				g.GetType().Name.ToLowerInvariant() == name
					|| g.GameNames.Any(n => n.ToLowerInvariant() == name)
			);

			if (game != null)
				return game;

			return GameMemory.SupportedGames.FirstOrDefault(g =>
				g.GameNames.Any(n => name.Contains(n.ToLowerInvariant()))
			);
		}

		void RefreshCheckList()
		{
			_isRefreshingListbox = true;

			chklbMapSet.Items.Clear();
			foreach (var pair in Maps)
			{
				chklbMapSet.Items.Add(pair.Key, pair.Value);
			}

			_isRefreshingListbox = false;
		}

		public XmlNode GetSettings(XmlDocument doc)
		{
			XmlElement settingsNode = doc.CreateElement("Settings");

			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "Version", Assembly.GetExecutingAssembly().GetName().Version.ToString(3)));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoStart", AutoStart));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoReset", AutoReset));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoSplitOnMapChange", AutoSplitOnMapChange));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "AutoSplitOncePerMap", AutoSplitOncePerMap));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "UsePrevMap", UsePrevMap));
			settingsNode.AppendChild(SettingsHelper.ToElement(doc, "Game", ((Type)cbGame.SelectedItem).Name));

			var mapsNode = settingsNode.AppendChild(doc.CreateElement("MapWhitelist"));
			foreach (var pair in Maps)
			{
				var elem = (XmlElement)mapsNode.AppendChild(doc.CreateElement("Map"));
				elem.InnerText = pair.Key;
				elem.SetAttribute("enabled", pair.Value.ToString());
			}

			return settingsNode;
		}

		public void SetSettings(XmlNode settings)
		{
			var element = (XmlElement)settings;

			AutoStart = SettingsHelper.ParseBool(settings["AutoStart"], DEFAULT_AUTOSTART);
			AutoReset = SettingsHelper.ParseBool(settings["AutoReset"], DEFAULT_AUTOSTART);
			AutoSplitOnMapChange = SettingsHelper.ParseBool(settings["AutoSplitOnMapChange"], DEFAULT_AUTOSPLITONMAPCHANGE);
			AutoSplitOncePerMap = SettingsHelper.ParseBool(settings["AutoSplitOncePerMap"], DEFAULT_AUTOSPLITONCEPERMAP);
			UsePrevMap = SettingsHelper.ParseBool(settings["UsePrevMap"], DEFAULT_USEPREVMAP);

			GameSupport game = null;
			if (!string.IsNullOrWhiteSpace(settings["Game"]?.InnerText))
				game = SearchGameSupport(settings["Game"].InnerText);

			if (game == null)
				game = SearchGameSupport(_state.Run.GameName) ?? GameMemory.SupportedGames[0];

			cbGame.SelectedItem = game.GetType();

			if (settings["MapWhitelist"] != null)
			{
				foreach (XmlElement elem in settings["MapWhitelist"].ChildNodes)
				{
					if (Maps.ContainsKey(elem.InnerText))
						Maps[elem.InnerText] = bool.Parse(elem.GetAttribute("enabled"));
				}
			}
			RefreshCheckList();
        }

		void btnAddMap_Click(object sender, EventArgs e)
		{
			txtMap.Text = txtMap.Text.Trim();

			if (!string.IsNullOrWhiteSpace(txtMap.Text))
			{
				Maps.Add(txtMap.Text, true);
				chklbMapSet.Items.Add(txtMap.Text, true);
				txtMap.Clear();
			}
		}

		void btnRemoveMap_Click(object sender, EventArgs e)
		{
			if (chklbMapSet.SelectedIndex < 0)
				return;

			var selectedIndex = chklbMapSet.SelectedIndex;
            Maps.Remove((string)chklbMapSet.SelectedItem);
			chklbMapSet.Items.RemoveAt(selectedIndex);

			var count = chklbMapSet.Items.Count;
            if (count > 0)
				chklbMapSet.SelectedIndex = selectedIndex <= count - 1 ? selectedIndex : selectedIndex - 1;
		}

		void cbGame_SelectedIndexChanged(object sender, EventArgs e)
		{
            var selected = (GameSupport)Activator.CreateInstance((Type)cbGame.SelectedItem);

			var copy = new Dictionary<string, bool>(Maps);
			Maps.Clear();
			if (selected?.Maps != null)
			{
				foreach (var map in selected.Maps)
				{
					Maps[map] = copy.ContainsKey(map) ? copy[map] : true;
				}
			}

			RefreshCheckList();
		}

		void chklbMapSet_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			if (_isRefreshingListbox)
				return;

			var map = (string)chklbMapSet.Items[e.Index];
			Maps[map] = CheckState.Checked == e.NewValue;
		}
	}
}
