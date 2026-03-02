namespace Binner.SwarmApi.Model
{
    public class NameValuePair<T>
    {
        public string Name { get; set; } = string.Empty;
        public T? Value { get; set; } = default(T);

        public NameValuePair() { }

        public NameValuePair(string name, T value)
        {
            Name = name;
            Value = value;
        }
    }
}
