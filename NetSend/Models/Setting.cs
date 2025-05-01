namespace NetSend.Models;

public class Setting(string name, string value, bool isEnabled) : Model {
    public Setting() : this(string.Empty, string.Empty, false) {
    }

    public bool isEnabled { get; set; } = isEnabled;

    public string Name { get; set; } = name;
    public string Value { get; set; } = value;

    public bool isEmpty() {
        return Value == string.Empty;
    }
}