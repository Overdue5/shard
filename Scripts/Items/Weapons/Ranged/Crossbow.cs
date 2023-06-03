using System;

namespace Server.Items
{
	[Flipable( 0xF50, 0xF4F )]
	public class Crossbow : BaseRanged
	{
		public override int EffectID => 0x1BFE;
        public override Type AmmoType => typeof( Bolt );
        public override BaseAmmo Ammo => new Bolt();

        ////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ConcussionBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 35;
        public override int AosMinDamage => 18;
        public override int AosMaxDamage => Core.ML ? 22 : 20;
        public override int AosSpeed => 24;

        public override int OldStrengthReq => 30;
        public override int OldMinDamage => 14;
        public override int OldMaxDamage => 30;
        public override int OldSpeed => 418;

        public override int DefMaxRange => 8;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 80;

        public override WeaponAnimation DefAnimation => WeaponAnimation.ShootXBow;

        [Constructable]
		public Crossbow() : base( 0xF50 )
		{
			Weight = 7.0;
			Layer = Layer.TwoHanded;
			//Name = "crossbow";
		}

		public Crossbow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            if (version == 0)
            {
                MinDamage = 14;
                MaxDamage = 30;
            }
		}
	}
}
