using System;

namespace Server.Custom.Games
{
    public interface Participant : IComparable<Participant>
    {
        int Score
        {
            get;
            set;
        }
    }
}
