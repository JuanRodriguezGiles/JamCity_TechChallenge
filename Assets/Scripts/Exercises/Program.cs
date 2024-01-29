using System.Linq;
using System.Collections.Generic;

using TechChallenge.Scripts.Data;

namespace TechChallenge.Scripts.Exercises
{
    public class Program
    {
        #region PRIVATE_FIELDS
        private List<Employee> employees = null;
        private Dictionary<DepartmentType, List<Employee>> groupedAndOrderedEmployees = null;
        #endregion

        #region PUBLIC_METHODS
        public void GroupAndOrderEmployees()
        {
            groupedAndOrderedEmployees = employees
                .GroupBy(employee => employee.Position)
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderBy(employee => employee.Seniority.Level).ToList()
                );
        }

        public double GetUpdatedSalary(double baseSalary, double incrementPercentage) { return baseSalary + baseSalary * (incrementPercentage / 100); }

        public int GetSeniorityCount(DepartmentType departmentType, SeniorityLevel seniorityLevel)
        {
            if (TryGetDepartmentEmployees(departmentType, out List<Employee> departmentEmployees))
            {
                Dictionary<SeniorityLevel, int> countBySeniority = departmentEmployees
                    .GroupBy(emp => emp.Seniority.Level)
                    .ToDictionary(group => group.Key, group => group.Count());

                return countBySeniority.TryGetValue(seniorityLevel, out int seniorityCount) ? seniorityCount : 0;
            }

            return 0;
        }

        public void GenerateEmployees(CompanyData companyData)
        {
            employees = new List<Employee>();

            for (int i = 0; i < companyData.DepartmentData.Count; i++)
            {
                for (int j = 0; j < companyData.DepartmentData[i].Seniorities.Count; j++)
                {
                    for (int k = 0; k < companyData.DepartmentData[i].Seniorities[j].Employees; k++)
                    {
                        employees.Add(new Employee(companyData.DepartmentData[i].DepartmentType, companyData.DepartmentData[i].Seniorities[j]));
                    }
                }
            }

            GroupAndOrderEmployees();
        }

        public int GetEmployeeCount() { return employees.Count; }

        public List<Employee> GetAllEmployees() { return employees; }

        public int GetDepartmentCount() { return groupedAndOrderedEmployees.Count; }

        public bool TryGetDepartmentEmployees(DepartmentType departmentType, out List<Employee> departmentEmployees) { return groupedAndOrderedEmployees.TryGetValue(departmentType, out departmentEmployees); }
        #endregion
    }
}