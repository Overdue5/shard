using System;

namespace Server.Custom.PvpToolkit.DMatch
{
    public class ScoreKeeper : IComparable
    {
        private Mobile m_Player;
        private int m_Kills;
        private int m_Deaths;

        public Mobile Player => m_Player;
        public int Kills { get => m_Kills;
            set => m_Kills = value;
        }
        public int Deaths { get => m_Deaths;
            set => m_Deaths = value;
        }

        public double Points { get; set; }

        public ScoreKeeper( Mobile m )
        {
            m_Player = m;
            m_Deaths = 0;
            m_Kills = 0;
            Points = 0.0;
        }

        public ScoreKeeper()
        {

        }

        public void Serialize( GenericWriter writer )
        {
            writer.Write( 1 );

            //ver 1
            writer.Write(Points);

            //ver 0
            writer.Write( m_Player );
            writer.Write( m_Kills );
            writer.Write( m_Deaths );
        }

        public void Deserialize( GenericReader reader )
        {
            int version = reader.ReadInt();

            switch( version )
            {
                case 1:
                    {
                        Points = reader.ReadDouble();
                        goto case 1;
                    }
                case 0:
                    {
                        m_Player = reader.ReadMobile();
                        m_Kills = reader.ReadInt();
                        m_Deaths = reader.ReadInt();
                        break;
                    }
            }
        }

        public int CompareTo(object obj)
        {
            ScoreKeeper scoreKeeper = (ScoreKeeper)obj;

            if (scoreKeeper.Points == Points)
                return scoreKeeper.Kills - Kills;

            return (int)(scoreKeeper.Points - Points);
        }
    }
}
