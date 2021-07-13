
namespace Scripts.SpecialSystems
{
    public class PvPSystem: PvXSystem
    {
        public static PvXStatistic Statistic
        {
            get { return StatDict[typeof(PvPSystem)]; }
        }

        public static void CalculateStat()
        {
            PvXSystem.CalculateMaxValues(typeof(PvPSystem));
        }
    }
}
