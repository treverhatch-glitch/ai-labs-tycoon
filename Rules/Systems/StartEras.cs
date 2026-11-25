using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Core;
using AiLabs.Rules.Sim;

namespace AiLabs.Rules.Systems
{
    /// <summary>A lab that is in the world when a game starts in an era, or comes later.</summary>
    public struct StartCastMember
    {
        public string RivalId;
        public string Name;
        /// <summary>Cash the lab starts with; 0 for a lab that only joins later.</summary>
        public long Cash;
        /// <summary>The year the lab appears; the start year or earlier for one that is already there.</summary>
        public int JoinsYear;
        public bool PresentAtStart;
    }

    /// <summary>What a game started in an era hands the player and who is already in the world. The numbers a start-era picker shows.</summary>
    public sealed class StartEraOffer
    {
        public string EraId;
        public string Name;
        public int StartYear;
        public string Tagline;
        public string ModelsCanDo;
        public bool Expansion;
        public long PlayerCash;
        public long GrantPerWeek;
        public string LotId;
        public int TechCount;
        public List<StartCastMember> Cast = new List<StartCastMember>();
        public int PresentRivals;
    }

    /// <summary>
    /// Starting in any era: the picker's list, built from the config alone. It restates what
    /// <see cref="WorldSetup.NewGame"/> will do (the player's cash, lot and techs, and which rivals are already there with how much),
    /// so a screen never has to start a game to preview one. Nothing is stored.
    /// </summary>
    public static class StartEras
    {
        public static List<StartEraOffer> Offers(GameConfig c)
        {
            var offers = new List<StartEraOffer>();
            for (int i = 0; i < c.Eras.Count; i++) if (c.Eras[i].Startable) offers.Add(Offer(c, c.Eras[i]));
            return offers;
        }

        public static StartEraOffer Offer(GameConfig c, EraConfig era)
        {
            bool first = c.Eras[0] == era;
            var o = new StartEraOffer();
            o.EraId = era.Id;
            o.Name = era.Name;
            o.StartYear = era.StartYear;
            o.Tagline = era.Tagline;
            o.ModelsCanDo = era.ModelsCanDo;
            o.Expansion = era.Expansion;
            o.PlayerCash = MathI.MulPermille(c.PlayerStart.Cash, era.StartCashPermille);
            o.GrantPerWeek = MathI.MulPermille(c.PlayerStart.GrantPerWeek, era.StartGrantPermille);
            o.LotId = first ? c.PlayerStart.LotId : era.StartLotId;
            o.TechCount = era.StartTechs.Count;
            for (int i = 0; i < c.Rivals.Count; i++)
            {
                RivalConfig r = c.Rivals[i];
                if (r.JoinsYear >= RivalConfig.NeverJoins) continue;
                var m = new StartCastMember { RivalId = r.Id, Name = r.Name, JoinsYear = r.JoinsYear };
                m.PresentAtStart = r.JoinsYear <= era.StartYear;
                m.Cash = !m.PresentAtStart ? 0 : r.JoinsYear == 0 ? MathI.MulPermille(r.StartCash, era.StartCashPermille) : WorldSetup.EstablishedCash(c, r, era);
                if (m.PresentAtStart) o.PresentRivals++;
                o.Cast.Add(m);
            }
            return o;
        }
    }
}
