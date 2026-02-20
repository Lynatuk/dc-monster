namespace Dice.Services
{
    using Dice.Configs;

    public interface IModifierProvider
    {
        void Accumulate(ref FacePerkTable.AccumulatedPerks acc);
    }

    public static class ModifierUtils
    {
        public static void Accumulate(IModifierProvider[] providers, ref FacePerkTable.AccumulatedPerks acc)
        {
            if (providers == null)
                return;
            for (int i = 0; i < providers.Length; i++)
                providers[i]?.Accumulate(ref acc);
        }
    }
}
