using Server.Engines.VeteranRewards;

namespace Server.Items
{
    public class ZombieStatue : Item, IRewardItem
    {
        private bool m_IsRewardItem = true;

        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsRewardItem
        {
            get => m_IsRewardItem;
            set => m_IsRewardItem = value;
        }
		[Constructable]
		public ZombieStatue() : base( 0x25D5 )
		{
			Weight = 1.0;
		}

        public ZombieStatue(Serial serial) : base(serial) { }

		public override void Serialize( GenericWriter writer ) { base.Serialize( writer ); writer.Write( 0 ); }

		public override void Deserialize( GenericReader reader ) { base.Deserialize( reader ); int version = reader.ReadInt(); }
	}
}