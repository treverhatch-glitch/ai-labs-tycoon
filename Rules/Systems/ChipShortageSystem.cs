using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.State;

namespace AiLabs.Rules.Systems
{
    /// <summary>
    /// When the wider world runs out of graphics cards, everything that rents, buys or fabs compute costs more
    /// and the foundry has less room. Started by dated world events, not dice.
    /// </summary>
    public static class ChipShortageSystem
    {
        public static bool Active(GameState state)
        {
            if (state.ChipShortageStrengthPermille < 1 || state.ChipShortageUntilTick < 0) return false;
            return state.Tick < state.ChipShortageUntilTick;
        }

        public static int PriceMulPermille(GameState state)
        {
            return Active(state) ? state.ChipShortageStrengthPermille : MathI.Permille;
        }

        /// <summary>Foundry capacity shrinks as prices rise (strength 1200 → 800‰ room).</summary>
        public static int FoundryRoomMulPermille(GameState state)
        {
            if (!Active(state)) return MathI.Permille;
            return (int)MathI.Clamp(2000 - state.ChipShortageStrengthPermille, 500, MathI.Permille);
        }

        public static long ScalePrice(GameState state, long price)
        {
            return MathI.MulPermille(price, PriceMulPermille(state));
        }

        public struct PriceScaleBreakdown
        {
            public long Base;
            public long FromShortage;
            public long Total { get { return Base + FromShortage; } }
        }

        public static PriceScaleBreakdown PriceScaleParts(GameState state, long basePrice)
        {
            var b = new PriceScaleBreakdown { Base = basePrice };
            long scaled = ScalePrice(state, basePrice);
            b.FromShortage = scaled - basePrice;
            return b;
        }

        public struct FoundryRoomBreakdown
        {
            public long Base;
            public long FromShortage;
            public long Total { get { return Base + FromShortage; } }
        }

        public static FoundryRoomBreakdown FoundryRoomParts(GameConfig c, GameState state, ChipNodeConfig node)
        {
            long baseRoom = c.Chips.FoundryRoom(node);
            var b = new FoundryRoomBreakdown { Base = baseRoom };
            long effective = EffectiveFoundryRoom(c, state, node);
            b.FromShortage = effective - baseRoom;
            return b;
        }

        /// <summary>Monthly Envydia allocation pool while the crunch is on (stronger crunch → fewer GPUs).</summary>
        public static int VendorSupplyPermille(GameConfig c, GameState state)
        {
            if (!Active(state)) return MathI.Permille;
            int floor = c.Chips.Shortage.VendorSupplyFloorPermille;
            return (int)MathI.Max(floor, 2000 - state.ChipShortageStrengthPermille);
        }

        public static int ScaleVendorSupply(GameConfig c, GameState state, int supply)
        {
            return (int)MathI.MulPermille(supply, VendorSupplyPermille(c, state));
        }

        public static long EffectiveFoundryRoom(GameConfig c, GameState state, ChipNodeConfig node)
        {
            return MathI.MulPermille(c.Chips.FoundryRoom(node), FoundryRoomMulPermille(state));
        }

        public static void OnWorldEvent(GameConfig c, GameState state, WorldEventConfig e)
        {
            if (e.ChipShortageStrengthPermille < 1) return;
            Start(c, state, e.ChipShortageStrengthPermille);
        }

        public static void Start(GameConfig c, GameState state, int strengthPermille)
        {
            int min = c.Chips.Shortage.MinStrengthPermille;
            int max = c.Chips.Shortage.MaxStrengthPermille;
            int strength = (int)MathI.Clamp(strengthPermille, min, max);
            int until = state.Tick + c.Chips.Shortage.DurationWeeks;
            bool wasActive = Active(state);
            if (wasActive)
            {
                if (strength > state.ChipShortageStrengthPermille)
                    state.ChipShortageStrengthPermille = strength;
                if (until > state.ChipShortageUntilTick)
                    state.ChipShortageUntilTick = until;
                return;
            }
            state.ChipShortageStrengthPermille = strength;
            state.ChipShortageUntilTick = until;
            Events.Add(state, EventKind.ChipShortageBegan, -1, -1, strength, "");
        }

        public static void RunWeek(GameConfig c, GameState state)
        {
            if (state.ChipShortageUntilTick < 0) return;
            if (state.Tick < state.ChipShortageUntilTick) return;
            int last = state.ChipShortageStrengthPermille;
            state.ChipShortageUntilTick = -1;
            state.ChipShortageStrengthPermille = 0;
            if (last > 0)
                Events.Add(state, EventKind.ChipShortageEnded, -1, -1, last, "");
        }
    }
}