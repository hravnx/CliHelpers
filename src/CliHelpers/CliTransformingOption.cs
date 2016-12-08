using System;

namespace CliHelpers
{
    public sealed class CliTransformingOption<T, U> : CliOptionWithValueBase<U>
    {
        ICliOptionWithValue<T> m_Prev;
        Func<T, U> m_Transformer;
        internal CliTransformingOption(ICliOptionWithValue<T> prev, Func<T, U> transformer)
            : base(prev)
        {
            m_Prev = prev;
            m_Transformer = transformer;
        }
        public override U Value()
        {
            var v = m_Prev.Value();
            return m_Transformer(v);
        }
    }
}
