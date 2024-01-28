using System.Linq;
using System.Collections.Generic;

using TechChallenge.Scripts.Data;

namespace TechChallenge.Scripts.Exercises
{
    public class Exercise1 
    {
        #region PUBLIC_METHODS
        public Dictionary<DepartmentType, List<Employee>> GroupAndOrderEmployees(List<Employee> employees)
        {
            Dictionary<DepartmentType, List<Employee>> groupedAndOrderedEmployees = employees
                .GroupBy(employee => employee.Position)
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderBy(employee => employee.Seniority.Level).ToList()
                );

            return groupedAndOrderedEmployees;
        }

        public Dictionary<SeniorityLevel, int> GroupBySeniority(List<Employee> employees)
        {
            Dictionary<SeniorityLevel, int> countBySeniority = employees
                .GroupBy(emp => emp.Seniority.Level)
                .ToDictionary(group => group.Key, group => group.Count());

            return countBySeniority;
        }
        #endregion
    }
}