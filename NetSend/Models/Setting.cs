namespace NetSend.Models;

public class Setting(string name, string value, bool isEnabled) {
    public Setting() : this(string.Empty, string.Empty, false) {
    }

    public int Id { get; set; }

    public bool isEnabled { get; set; } = isEnabled;

    public string Name { get; set; } = name;
    public string Value { get; set; } = value;

    public bool isEmpty() {
        return Value == string.Empty;
    }
}