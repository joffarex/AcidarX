namespace AcidarX.Core.Renderer
{
    public readonly struct RendererID
    {
        private readonly uint _value;

        public RendererID(uint value) => _value = value;

        public static explicit operator RendererID(uint value) => new(value);
        public static implicit operator uint(RendererID rendererID) => rendererID._value;

        public override string ToString() => _value.ToString();
    }
}