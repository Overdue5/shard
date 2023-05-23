using System;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.SkillHandlers
{
	public class Provocation
	{
		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.Provocation].Callback = OnUse;
		}

		public static TimeSpan OnUse( Mobile m )
		{
            if (m.BeginAction(typeof(IAction)))
            {
                m.RevealingAction();
                BaseInstrument.PickInstrument(m, OnPickedInstrument);
            }
            else
                m.SendAsciiMessage("You must wait to perform another action.");

			return TimeSpan.FromSeconds( 1.0 ); // Cannot use another skill for 1 second
		}

		public static void OnPickedInstrument( Mobile from, BaseInstrument instrument )
		{
			from.RevealingAction();
			from.SendLocalizedMessage( 501587 ); // Whom do you wish to incite?
			from.Target = new InternalFirstTarget( from, instrument );
		}

		private class InternalFirstTarget : Target, IAction
		{
			private readonly BaseInstrument m_Instrument;

			public InternalFirstTarget( Mobile from, BaseInstrument instrument ) : base( BaseInstrument.GetBardRange( from, SkillName.Provocation ), false, TargetFlags.None )
			{
				m_Instrument = instrument;

                if (from is PlayerMobile)
                    ((PlayerMobile)from).ResetPlayerAction(this);
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
                bool releaseLock = true;
				from.RevealingAction();

				if ( targeted is BaseCreature && from.CanBeHarmful( (Mobile)targeted, true ) )
				{
					BaseCreature creature = (BaseCreature)targeted;

                    if (!m_Instrument.IsChildOf(from.Backpack))
                        from.SendLocalizedMessage(1062488); // The instrument you are trying to play is no longer in your backpack!
                    
                    else if (creature is BaseVendor)
                        from.SendAsciiMessage("You cannot provoke vendors");

                    else if (creature.Controlled)
                        from.SendLocalizedMessage(501590); // They are too loyal to their master to be provoked.
                    
                    else if (creature.IsParagon && BaseInstrument.GetBaseDifficulty(creature) >= 160.0)
                        from.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                   
                    else
                    {
                        releaseLock = false;
                        from.RevealingAction();
                        m_Instrument.PlayInstrumentWell(from);
                        from.SendLocalizedMessage(1008085); // You play your music and your target becomes angered.  Whom do you wish them to attack?
                        from.Target = new InternalSecondTarget(from, m_Instrument, creature);
                    }
				}
				else
				{
					from.SendLocalizedMessage( 501589 ); // You can't incite that!
				}

                if (releaseLock && from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
            }

            #region TargetFailed

            protected override void OnCantSeeTarget(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetDeleted(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetDeleted(from, targeted);
            }

            protected override void OnTargetUntargetable(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetUntargetable(from, targeted);
            }

            protected override void OnNonlocalTarget(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnNonlocalTarget(from, targeted);
            }

            protected override void OnTargetInSecureTrade(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetInSecureTrade(from, targeted);
            }

            protected override void OnTargetNotAccessible(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetNotAccessible(from, targeted);
            }

            protected override void OnTargetOutOfLOS(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetOutOfLOS(from, targeted);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetOutOfRange(from, targeted);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetCancel(from, cancelType);
            }

            #endregion

            #region IAction Members

            public void AbortAction(Mobile from)
            {
            }

            #endregion
		}

		private class InternalSecondTarget : Target, IAction
		{
			private readonly BaseCreature m_Creature;
			private readonly BaseInstrument m_Instrument;

			public InternalSecondTarget( Mobile from, BaseInstrument instrument, BaseCreature creature ) : base( BaseInstrument.GetBardRange( from, SkillName.Provocation ), false, TargetFlags.None )
			{
				m_Instrument = instrument;
				m_Creature = creature;

                if (from is PlayerMobile)
                    ((PlayerMobile)from).ResetPlayerAction(this);
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				from.RevealingAction();
                bool releaseLock = true;

				if ( targeted is BaseCreature )
				{
					BaseCreature creature = (BaseCreature)targeted;

					if ( !m_Instrument.IsChildOf( from.Backpack ) )
					{
						from.SendLocalizedMessage( 1062488 ); // The instrument you are trying to play is no longer in your backpack!
					}
					else if ( m_Creature.Unprovokable  )
					{
						from.SendLocalizedMessage( 1049446 ); // You have no chance of provoking those creatures.
					}
                    else if (creature.Unprovokable && !(creature is DemonKnight))
                    {
                        from.SendLocalizedMessage(1049446); // You have no chance of provoking those creatures.
                    }
					else if ( m_Creature.Map != creature.Map || !m_Creature.InRange( creature, BaseInstrument.GetBardRange( from, SkillName.Provocation ) ) )
					{
						from.SendLocalizedMessage( 1049450 ); // The creatures you are trying to provoke are too far away from each other for your music to have an effect.
					}
					else if ( m_Creature != creature )
					{
						from.NextSkillTime = DateTime.UtcNow + TimeSpan.FromSeconds( 10.0 );

						double diff = ((m_Instrument.GetDifficultyFor( m_Creature ) + m_Instrument.GetDifficultyFor( creature )) * 0.5) - 5.0;
						double music = from.Skills[SkillName.Musicianship].Value;

						if ( music > 100.0 )
							diff -= (music - 100.0) * 0.5;

						if ( from.CanBeHarmful( m_Creature, true ) && from.CanBeHarmful( creature, true ) )
						{
							if ( !BaseInstrument.CheckMusicianship( from ) )
							{
								from.NextSkillTime = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 );
								from.SendLocalizedMessage( 500612 ); // You play poorly, and there is no effect.
								m_Instrument.PlayInstrumentBadly( from );
								m_Instrument.ConsumeUse( from );
							}
							else
							{
								from.DoHarmful( m_Creature, true );
								from.DoHarmful( creature, true );

								if ( !from.CheckTargetSkill( SkillName.Provocation, creature, diff-25.0, diff+25.0 ) )
								{
									from.NextSkillTime = DateTime.UtcNow + TimeSpan.FromSeconds( 5.0 );
									from.SendLocalizedMessage( 501599 ); // Your music fails to incite enough anger.
									m_Instrument.PlayInstrumentBadly( from );
									m_Instrument.ConsumeUse( from );
								}
								else
								{
									from.SendLocalizedMessage( 501602 ); // Your music succeeds, as you start a fight.
									m_Instrument.PlayInstrumentWell( from );
									m_Instrument.ConsumeUse( from );
									m_Creature.Provoke( from, creature, true );
								}
							}
						}
					}
					else
					{
						from.SendLocalizedMessage( 501593 ); // You can't tell someone to attack themselves!
					}
				}
				else
				{
					from.SendLocalizedMessage( 501589 ); // You can't incite that!
				}

                if (releaseLock && from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
            }

            #region TargetFailed

            protected override void OnCantSeeTarget(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnCantSeeTarget(from, targeted);
            }

            protected override void OnTargetDeleted(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetDeleted(from, targeted);
            }

            protected override void OnTargetUntargetable(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetUntargetable(from, targeted);
            }

            protected override void OnNonlocalTarget(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnNonlocalTarget(from, targeted);
            }

            protected override void OnTargetInSecureTrade(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetInSecureTrade(from, targeted);
            }

            protected override void OnTargetNotAccessible(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetNotAccessible(from, targeted);
            }

            protected override void OnTargetOutOfLOS(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetOutOfLOS(from, targeted);
            }

            protected override void OnTargetOutOfRange(Mobile from, object targeted)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetOutOfRange(from, targeted);
            }

            protected override void OnTargetCancel(Mobile from, TargetCancelType cancelType)
            {
                if (from is PlayerMobile)
                    ((PlayerMobile)from).EndPlayerAction();
                else
                    from.EndAction(typeof(IAction));

                base.OnTargetCancel(from, cancelType);
            }

            #endregion

            #region IAction Members

            public void AbortAction(Mobile from)
            {
            }

            #endregion
		}
	}
}