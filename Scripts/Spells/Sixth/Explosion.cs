using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Spells.Sixth
{
    public class ExplosionSpell : MagerySpell
    {
        public override SpellCircle Circle => SpellCircle.Sixth;
        public override int Sound => 519;

        //Iza - explosion range
        private const int _RANGE = 2;
        //Iza - end

        private static readonly SpellInfo m_Info = new SpellInfo(
            "Explosion", "Vas Ort Flam",
            212,
            9041,
            Reagent.Bloodmoss,
            Reagent.MandrakeRoot
            );

        public ExplosionSpell(Mobile caster, Item scroll) : base(caster, scroll, m_Info)
        {
        }

        public override void OnPlayerCast()
        {
            if (SphereSpellTarget is Mobile)
                Target((Mobile)SphereSpellTarget);
            else if (SphereSpellTarget is BaseWand)
            {
                BaseWand bw = SphereSpellTarget as BaseWand;
                bw.RechargeWand(Caster, this);
            }
            else
                DoFizzle();
        }

        public override void OnCast()
        {
            Caster.Target = new InternalTarget(this);
        }

        public override bool DelayedDamage => false;

        public void Target(Mobile m)
        {
            if (!Caster.CanSee(m))
            {
                Caster.SendAsciiMessage("Target is not in line of sight.");
                DoFizzle();
            }

            if (CheckHSequence(m))
            {
                Mobile source = Caster;

                SpellHelper.CheckReflect((int)Circle, ref source, ref m);

                #region Iza - Multi targets
                List<Mobile> targets = new List<Mobile>();
                Map map = Caster.Map;

                if (map != null)
                {
                    IPooledEnumerable eable = map.GetMobilesInRange(new Point3D(m), _RANGE);
                    foreach (Mobile mob in eable)
                    {
                        if (SpellHelper.ValidIndirectTarget(Caster, m) && Caster.CanBeHarmful(mob, false) && Caster.InLOS(mob))
                        {
                            targets.Add(mob);
                        }
                    }
                    eable.Free();
                }
                #endregion

                if (targets.Count > 0)
                {
                    //damage /= targets.Count;

                    for (int i = 0; i < targets.Count; ++i)
                    {
                        Mobile mob = targets[i];

                        double damage = 60 + ((int)(GetDamageSkill(Caster) - GetResistSkill(m)) / 12); //Loki: Was 39-42

                        if (Scroll != null)
                            damage -= 4;

                        #region Taran - Damage based on AR
                        if (mob is PlayerMobile)
                        {
                            double armorRating = ((PlayerMobile)mob).BaseArmorRatingSpells;

                            //Loki edit: New formula for new PvP changes
                            double arR = armorRating / 8.0;
                            damage = damage - 1.5 * ((arR + 1) * arR);

                            if (damage > 60)
                                damage = 60;

                            if (damage < 20)
                                damage = 20;
                        }
                        #endregion

                        if (damage < 1)
                            damage = 1;
                        

                        Caster.DoHarmful(mob);
                        SpellHelper.Damage(this, mob, damage, 0, 100, 0, 0, 0);
                    }
                }

                // Do the effects
                m.FixedParticles(0x36BD, 20, 10, 5044, EffectLayer.Head);
                m.PlaySound(Sound);
            }

            FinishSequence();
        }

        private class InternalTarget : Target
        {
            private readonly ExplosionSpell m_Owner;

            public InternalTarget(ExplosionSpell owner)
                : base(12, false, TargetFlags.Harmful)
            {
                m_Owner = owner;
            }

            protected override void OnTarget(Mobile from, object o)
            {
                if (o is Mobile)
                    m_Owner.Target((Mobile) o);
            }

            protected override void OnTargetFinish(Mobile from)
            {
                m_Owner.FinishSequence();
            }
        }
    }
}