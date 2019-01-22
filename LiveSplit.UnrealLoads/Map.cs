using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiveSplit.UnrealLoads
{
	public class Map
	{
		public string Name { get; }

		public bool SplitOnEnter { get; set; } = false;

		public bool SplitOnLeave { get; set; } = false;

		public Map(string name)
		{
			Name = name;
		}
	}
}
