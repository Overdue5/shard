using System;
using System.Collections.Generic;
using Server.Regions;

namespace Server.Spells.Eighth
{
    public class EarthquakeSpell : MagerySpell
    {
        public override SpellCircle Circle => SpellCircle.Eighth;
        public override int Sound => 525;

        public override bool HasNoTarget => true;

        private static readonly SpellInfo m_Info = new SpellInfo(
				"Earthquake", "In Vas Por",
				233,
                9012,
                true,
				Reagent.Bloodmoss,
				Reagent.Ginseng,
				Reagent.MandrakeRoot,
				Reagent.SulfurousAsh
			);

		public EarthquakeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool DelayedDamage => !Core.AOS;

        public override void OnCast()
		{
			if ( CheckSequence() )
			{
				List<Mobile> targets = new List<Mobile>();

				Map map = Caster.Map;

				if ( map != null )
					foreach ( Mobile m in Caster.GetMobilesInRange( 1 + (int)(Caster.Skills[SkillName.Magery].Value / 15.0) ) )
						if ( Caster != m && SpellHelper.ValidIndirectTarget( Caster, m ) && Caster.CanBeHarmful( m, false ) && (!Core.AOS || Caster.InLOS( m )) )
							targets.Add( m );

				Caster.PlaySound( Sound ); //0x2F3

				for ( int i = 0; i < targets.Count; ++i )
				{
                    Mobile m = targets[i];

				    CustomRegion cR = m.Region as CustomRegion;

                    if (cR != null && cR.Controller.IsRestrictedSpell(this)) //Taran: Don't allow EQ damage in areas where EQ is not allowed
                        continue;

					int damage;

					if ( Core.AOS )
					{
						damage = m.Hits / 2;

						if ( !m.Player )
							damage = Math.Max( Math.Min( damage, 100 ), 15 );
							damage += Utility.RandomMinMax( 0, 15 );

					}
					else
					{
						damage = (m.Hits * 6) / 10;

						if ( !m.Player && damage < 10 )
							damage = 10;
						else if ( damage > 75 )
							damage = 75;
					}

					Caster.DoHarmful( m );
					SpellHelper.Damage( TimeSpan.Zero, m, Caster, damage, 100, 0, 0, 0, 0 );
				}
			}

			FinishSequence();
		}
	}
}