using System;

namespace AiLabs.Rules.Core
{
    /// <summary>
    /// A seeded random stream (SplitMix64). Its whole state is one number kept in the save,
    /// so a loaded game rolls exactly the dice the original would have rolled.
    /// </summary>
    public sealed class Rng : ISync
    {
        /// <summary>The 64-bit state, stored as a signed number so it fits a JSON whole number.</summary>
        public long State;

        public Rng() { }

        public Rng(long seed) { State = seed; }

        public void Sync(Syncer s) { s.Long("state", ref State); }

        public ulong NextU64()
        {
            unchecked
            {
                ulong z = (ulong)State + 0x9E3779B97F4A7C15UL;
                State = (long)z;
                z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
                z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
                return z ^ (z >> 31);
            }
        }

        /// <summary>A whole number from 0 up to, not including, <paramref name="maxExclusive"/>, with no modulo bias.</summary>
        public int Next(int maxExclusive)
        {
            if (maxExclusive <= 0) throw new ArgumentOutOfRangeException("maxExclusive", "must be above zero");
            ulong n = (ulong)maxExclusive;
            ulong threshold = unchecked(0UL - n) % n;
            while (true)
            {
                ulong r = NextU64();
                if (r >= threshold) return (int)(r % n);
            }
        }

        /// <summary>A whole number from <paramref name="min"/> to <paramref name="max"/>, both included.</summary>
        public int Range(int min, int max)
        {
            if (max < min) throw new ArgumentOutOfRangeException("max", "must not be below min");
            return min + Next(max - min + 1);
        }

        /// <summary>True <paramref name="permille"/> times in a thousand.</summary>
        public bool Chance(int permille)
        {
            if (permille <= 0) return false;
            if (permille >= 1000) return true;
            return Next(1000) < permille;
        }
    }

    /// <summary>
    /// One stream per system. A new roll added to one system never shifts the dice of another,
    /// which keeps balance changes local and recorded runs comparable.
    /// </summary>
    public sealed class RngSet : ISync
    {
        public Rng Training = new Rng();
        public Rng Safety = new Rng();
        public Rng Ranking = new Rng();
        public Rng Rivals = new Rng();
        /// <summary>Sites, weather and town votes.</summary>
        public Rng World = new Rng();
        /// <summary>Chip respins and yield crashes.</summary>
        public Rng Chips = new Rng();
        /// <summary>Shady operations and investigations.</summary>
        public Rng Shady = new Rng();

        /// <summary>The numbers a save migration mixes into the game's seed to start a stream an old save never had.</summary>
        public const long WorldMigrationKey = 0x57_0000_0001L;
        public const long ChipsMigrationKey = 0x57_0000_0002L;
        public const long ShadyMigrationKey = 0x57_0000_0003L;

        /// <summary>The first state of a stream added after the save was made: one draw from the seed mixed with the stream's key.</summary>
        public static long MigratedState(long seed, long key) { return (long)new Rng(seed ^ key).NextU64(); }

        /// <summary>The four original streams are drawn first and the newer ones after them, so adding a stream never moves an old one.</summary>
        public static RngSet FromSeed(long seed)
        {
            var root = new Rng(seed);
            var set = new RngSet();
            set.Training.State = (long)root.NextU64();
            set.Safety.State = (long)root.NextU64();
            set.Ranking.State = (long)root.NextU64();
            set.Rivals.State = (long)root.NextU64();
            set.World.State = (long)root.NextU64();
            set.Chips.State = (long)root.NextU64();
            set.Shady.State = (long)root.NextU64();
            return set;
        }

        public void Sync(Syncer s)
        {
            s.Obj("training", ref Training);
            s.Obj("safety", ref Safety);
            s.Obj("ranking", ref Ranking);
            s.Obj("rivals", ref Rivals);
            s.Obj("world", ref World);
            s.Obj("chips", ref Chips);
            s.Obj("shady", ref Shady);
        }
    }
}
