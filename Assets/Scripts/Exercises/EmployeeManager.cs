using System;
using System.Linq;
using System.Collections.Generic;

using TechChallenge.Scripts.Data;

namespace TechChallenge.Scripts.Exercises
{
    public class EmployeeManager
    {
        #region PRIVATE_FIELDS
        private int employeeCount = 0;
        private Dictionary<DepartmentType, List<Employee>> departmentEmployees = null;
        #endregion

        #region PUBLIC_METHODS
        public EmployeeManager(List<Employee> employees)
        {
            employeeCount = employees.Count;
            
            departmentEmployees = employees
                .GroupBy(employee => employee.Position)
                .ToDictionary(
                    group => group.Key,
                    group => group.OrderBy(employee => employee.Seniority.Level).ToList()
                );
        }

        public double GetUpdatedSalary(double baseSalary, double incrementPercentage)
        {
            return baseSalary + baseSalary * (incrementPercentage / 100);
        }

        public int GetSeniorityCount(DepartmentType departmentType, SeniorityLevel seniorityLevel)
        {
            if (TryGetDepartmentEmployees(departmentType, out List<Employee> departmentEmployees))
            {
                Dictionary<SeniorityLevel, int> countBySeniority = departmentEmployees
                    .GroupBy(emp => emp.Seniority.Level)
                    .ToDictionary(group => group.Key, group => group.Count());

                if (countBySeniority.TryGetValue(seniorityLevel, out int seniorityCount))
                {
                    return seniorityCount;
                }
                
                throw new ArgumentException("Invalid seniority");
            }

            throw new ArgumentException("Invalid department");
        }
        public int GetEmployeeCount()
        {
            return employeeCount;
        }

        public int GetDepartmentCount()
        {
            return departmentEmployees.Count;
        }

        public bool TryGetDepartmentEmployees(DepartmentType departmentType, out List<Employee> departmentEmployees)
        {
            return this.departmentEmployees.TryGetValue(departmentType, out departmentEmployees);
        }
        
        public double GetEmployeeSalaryIncrement(DepartmentType departmentType, SeniorityLevel seniorityLevel)
        {
            if (departmentEmployees.TryGetValue(departmentType, out List<Employee> result))
            {
                Employee employee = result.FirstOrDefault(e => e.Seniority.Level == seniorityLevel);
                if (employee != null)
                {
                    return employee.Seniority.IncrementPercentage;
                }

                throw new ArgumentException("Invalid seniority");
            }

            throw new ArgumentException("Invalid department");
        }

        #endregion
    }
}