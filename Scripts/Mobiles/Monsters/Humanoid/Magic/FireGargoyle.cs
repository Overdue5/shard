using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a charred corpse" )]
	public class FireGargoyle : BaseCreature
	{
		[Constructable]
		public FireGargoyle() : base( AIType.AI_SphereMage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "Fire Gargoyle" );
			Body = 130;
			BaseSoundID = 0x174;

			SetStr( 351, 400 );
			SetDex( 126, 145 );
			SetInt( 226, 250 );

			SetHits( 211, 240 );

			SetDamage( 7, 14 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 80 );

			SetResistance( ResistanceType.Physical, 30, 35 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.Anatomy, 75.1, 85.0 );
			SetSkill( SkillName.EvalInt, 90.1, 105.0 );
			SetSkill( SkillName.Magery, 90.1, 105.0 );
			SetSkill( SkillName.Meditation, 90.1, 105.0 );
			SetSkill( SkillName.MagicResist, 90.1, 105.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 40.1, 80.0 );

			Fame = 3500;

			VirtualArmor = 32;

            if (Utility.RandomDouble() <= 0.7)
            {
                Spellbook book = new Spellbook();
                book.Content = ulong.MaxValue;
                book.LootType = LootType.Regular;
                AddItem(book);
            }
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			AddLoot( LootPack.Gems );
		}

		public override bool HasBreath => true; // fire breath enabled
		public override int TreasureMapLevel => 1;
        public override int Meat => 1;

        public FireGargoyle( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}