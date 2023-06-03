using System;

namespace Server.Items
{
	[Flipable( 0x26C2, 0x26CC )]
	public class CompositeBow : BaseRanged
	{
		public override int EffectID => 0xF42;
        public override Type AmmoType => typeof( Arrow );
        public override BaseAmmo Ammo => new Arrow();

        //public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.MovingShot; } }

		public override int AosStrengthReq => 45;
        public override int AosMinDamage => Core.ML ? 13 : 15;
        public override int AosMaxDamage => 17;
        public override int AosSpeed => 25;

        public override int OldStrengthReq => 45;
        public override int OldMinDamage => 15;
        public override int OldMaxDamage => 17;
        public override int OldSpeed => 520;

        public override int DefMaxRange => 10;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 70;

        //public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootBow; } }

		[Constructable]
		public CompositeBow() : base( 0x26C2 )
		{
			Weight = 5.0;
			//Name = "composite bow";
		}

		public CompositeBow( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}