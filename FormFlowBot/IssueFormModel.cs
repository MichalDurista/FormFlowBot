using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FormFlowBot
{
	[Serializable]
	public class IssueFormModel
	{
		public string Description { get; set; }

		public string ProductName { get; set; }

		public string ProductPlatform { get; set; }
	}
}