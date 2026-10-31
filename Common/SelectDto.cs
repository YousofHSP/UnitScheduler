namespace Common;

public class SelectDto(string value = "", string text = "")
{
        public string Value { get; set; } = value;
        public string Text { get; set; } = text;
        public bool Disabled { get; set; } = false;
}