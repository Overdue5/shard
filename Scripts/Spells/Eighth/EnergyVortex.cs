using System;
using Server.Items;
using Server.Mobiles;
using Server.Regions;
using Server.Targeting;

namespace Server.Spells.Eighth
{
    public class EnergyVortexSpell : MagerySpell
    {
        public override SpellCircle Circle => SpellCircle.Eighth;
        public override int Sound => 0x212;

        public override bool CanTargetGround => true;

        private static readonly SpellInfo m_Info = new SpellInfo(
				"Energy Vortex", "Vas Corp Por",
				263,
                9032,
                true,
				Reagent.Bloodmoss,
				Reagent.BlackPearl,
				Reagent.MandrakeRoot,
				Reagent.Nightshade
			);

		public EnergyVortexSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			return true;
		}

		public override void OnCast()
		{
			if(Caster is PlayerMobile){
				Target( (IPoint3D)SphereSpellTarget );
			}
			else
				Caster.Target = new InternalTarget( this );
		}

		public void Target( IPoint3D p )
		{
			Map map = Caster.Map;

			SpellHelper.GetSurfaceTop( ref p );

            if (SphereSpellTarget is BaseWand)
            {
                BaseWand bw = SphereSpellTarget as BaseWand;
                bw.RechargeWand(Caster, this);
            }
			else if ( (map == null || !map.CanSpawnMobile( p.X, p.Y, p.Z )) && !(SphereSpellTarget is Mobile) )
			{
				Caster.SendLocalizedMessage( 501942 ); // That location is blocked.
			}
            else if (/*SpellHelper.CheckTown(p, Caster) && */CheckSequence())
            {
				TimeSpan duration;

				if ( Core.AOS )
					duration = TimeSpan.FromSeconds( 90.0 );
				else
					duration = TimeSpan.FromSeconds( Utility.Random( 300, 240 ) );

                if (Caster.InLOS(p))
                {
                    GuardedRegion reg = (GuardedRegion)Region.Find(new Point3D(p), Caster.Map).GetRegion(typeof(GuardedRegion));
                    if (reg != null && !reg.Disabled)
                        Caster.CriminalAction(true);
                    BaseCreature.Summon(new EnergyVortex(), false, Caster, new Point3D(p), Sound, duration);
                }
                else
                    Caster.SendAsciiMessage("You can't see that.");
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private EnergyVortexSpell m_Owner;

			public InternalTarget( EnergyVortexSpell owner ) : base( 12, true, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is IPoint3D )
					m_Owner.Target( (IPoint3D)o );
			}

			protected override void OnTargetOutOfLOS( Mobile from, object o )
			{
				from.SendLocalizedMessage( 501943 ); // Target cannot be seen. Try again.
				from.Target = new InternalTarget( m_Owner );
				from.Target.BeginTimeout( from, TimeoutTime - DateTime.UtcNow );
				m_Owner = null;
			}

			protected override void OnTargetFinish( Mobile from )
			{
				if ( m_Owner != null )
					m_Owner.FinishSequence();
			}
		}
	}
}