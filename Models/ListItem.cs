namespace ZentitleOnPremDemo.Models;

public class ListItem<T>
{
    public string Text { get; set; }
    public T Value { get; set; }

    public ListItem(T value, string text)
    {
        Text = text;
        Value = value;
    }

    public override string ToString() => Text;
}

