using System;

namespace Server.Items
{
	[Flipable( 0x13FD, 0x13FC )]
	public class HeavyCrossbow : BaseRanged
	{
		public override int EffectID => 0x1BFE;
        public override Type AmmoType => typeof( Bolt );
        public override BaseAmmo Ammo => new Bolt();

        ////public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.MovingShot; } }
		//public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Dismount; } }

		public override int AosStrengthReq => 80;
        public override int AosMinDamage => 19;
        public override int AosMaxDamage => 20;
        public override int AosSpeed => 22;

        public override int OldStrengthReq => 40;
        public override int OldMinDamage => 27;
        public override int OldMaxDamage => 39;
        public override int OldSpeed => 683;

        public override int DefMaxRange => 8;

        public override int InitMinHits => 31;
        public override int InitMaxHits => 100;

        public override WeaponAnimation DefAnimation => WeaponAnimation.ShootXBow;

        [Constructable]
		public HeavyCrossbow() : base( 0x13FD )
		{
			Weight = 9.0;
			Layer = Layer.TwoHanded;
			//Name = "heavy crossbow";
		}

		public HeavyCrossbow( Serial serial ) : base( serial )
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
                MinDamage = 27;
                MaxDamage = 39;
            }
		}
	}
}
