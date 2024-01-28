namespace TechChallenge.Scripts.Data
{
    public class Employee
    {
        #region PRIVATE_FIELDS
        private DepartmentType position = default;
        private Seniority seniority = null;
        #endregion

        #region PROPERTIES
        public DepartmentType Position { get => position; }
        public Seniority Seniority { get => seniority; }
        #endregion

        #region CONSTRUCTOR
        public Employee(DepartmentType position, Seniority seniority)
        {
            this.position = position;
            this.seniority = seniority;
        }
        #endregion
    }
}