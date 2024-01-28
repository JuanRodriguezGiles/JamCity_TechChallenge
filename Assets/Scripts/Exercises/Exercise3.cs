namespace TechChallenge.Scripts.Exercises
{
    public class Exercise3
    {
        #region PUBLIC_METHODS
        public double GetUpdatedSalary(double baseSalary, double incrementPercentage)
        {
            return baseSalary + baseSalary * (incrementPercentage / 100);
        }
        #endregion
    }
}