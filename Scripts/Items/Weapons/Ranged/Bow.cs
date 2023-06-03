using System;

namespace Server.Items
{
	[Flipable( 0x13B2, 0x13B1 )]
	public class Bow : BaseRanged
	{
		public override int EffectID => 0xF42;
        public override Type AmmoType => typeof( Arrow );
        public override BaseAmmo Ammo => new Arrow();

        ////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ParalyzingBlow; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MortalStrike; } }

		public override int AosStrengthReq => 30;
        public override int AosMinDamage => Core.ML ? 15 : 16;
        public override int AosMaxDamage => Core.ML ? 19 : 18;
        public override int AosSpeed => 25;

        public override int OldStrengthReq => 20;
        public override int OldMinDamage => 13;
        public override int OldMaxDamage => 20;
        public override int OldSpeed => 338;

        public override int DefMaxRange => 10;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 60;

        [Constructable]
		public Bow() : base( 0x13B2 )
		{
			Weight = 6.0;
			//Name = "bow";
			Layer = Layer.TwoHanded;
		}

		public Bow( Serial serial ) : base( serial )
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
                MinDamage = 17;
                MaxDamage = 24;
            }
		}
	}
}
