using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetSend.Models {
	public class Template(string name, string author, string text, List<TemplateParameter> parameters) {
		public int Id { get; set; }

		public string Name { get; set; } = name;
		public string Author { get; set; } = author;
		public string Text { get; set; } = text;
		public List<TemplateParameter> Parameters { get; set; } = parameters;
	}

	public record TemplateParameter(string name, string value);
}
