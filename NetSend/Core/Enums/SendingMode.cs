using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Core.Enums {
	public enum SendingMode : byte {
		ToAll,
		ToSingle,
		ToSelected
	}
}
