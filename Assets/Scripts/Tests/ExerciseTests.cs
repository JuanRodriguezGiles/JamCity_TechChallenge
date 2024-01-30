using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;

using UnityEditor;

using NUnit.Framework;

using TechChallenge.Scripts.UI;
using TechChallenge.Scripts.Data;
using TechChallenge.Scripts.Exercises;

using Random = UnityEngine.Random;

namespace TechChallenge.Scripts.Tests
{
    public class ExerciseTests 
    {
        #region PRIVATE_FIELDS
        private CompanyData companyData = null;
        private EmployeeManager employeeManager = null;
        private TestsUI testsUI = null;
        private List<Employee> employees = null;
        #endregion
        
        #region PUBLIC_METHODS
        [SetUp]
        public void SetUp()
        {
            LoadCompanyData();
            employees = GenerateEmployees(companyData);
            //Suffle to correctly test grouping and ordering
            Shuffle(employees);
            employeeManager = new EmployeeManager(employees);
        }
        
        [UnityTest, Order(1)]
        public IEnumerator TestLoadScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TechChallenge");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            testsUI = Object.FindObjectOfType<TestsUI>();
            
            Assert.IsNotNull(testsUI);
            Assert.IsNotNull(employeeManager);
        }

        [Test, Order(2)]
        public void TestLoadInputData()
        {
            UpdateTextUI("Running LoadInputData");
            
            Assert.IsNotNull(companyData);
            
            UpdateTextUI("Finished LoadInputData");
        }

        [Test, Order(3)]
        public void TestGenerateEmployees()
        {
            UpdateTextUI("Running TestGenerateEmployees");
            
            //Check if created employees count match with intended count
            Assert.AreEqual(employeeManager.GetEmployeeCount(), companyData.Employees);

            UpdateTextUI("Finished TestGenerateEmployees");
        }

        [Test, Order(4)]
        public void TestGrouping()
        {
            UpdateTextUI("Running TestGrouping");
            
            Assert.AreEqual(employeeManager.GetDepartmentCount(), companyData.DepartmentData.Count);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (employeeManager.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
                {
                    foreach (var employee in departmentEmployees)
                    {
                        Assert.AreEqual(employee.Position, departmentData.DepartmentType);
                    }
                }
                else
                {
                    Assert.Fail("Department: " + departmentData.DepartmentType + " not found");
                }
            }
            
            UpdateTextUI("Finished TestGrouping");
        }
        
        [Test, Order(5)]
        public void TestOrdering()
        {
            UpdateTextUI("Running TestOrdering");
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (employeeManager.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
                {
                    for (int i = 0; i < departmentEmployees.Count - 1; i++)
                    {
                        Assert.That(departmentEmployees[i].Seniority.Level <= departmentEmployees[i + 1].Seniority.Level);
                    }
                }
                else
                {
                    Assert.Fail("Department: " + departmentData.DepartmentType + " not found");
                }
            }
            
            UpdateTextUI("Finished TestOrdering");
        }

        [Test, Order(6)]
        public void TestDepartmentEmployeesCount()
        {
            UpdateTextUI("Running TestDepartmentEmployeesCount");
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                int departmentEmployeesCount = 0;
                foreach (var departmentSeniority in departmentData.Seniorities)
                {
                    departmentEmployeesCount += departmentSeniority.Employees;
                }
                
                if (employeeManager.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
                {
                    Assert.AreEqual(departmentEmployees.Count, departmentEmployeesCount);
                }
                else
                {
                    Assert.Fail("Department: " + departmentData.DepartmentType + " not found");
                }
            }

            UpdateTextUI("Finished TestDepartmentEmployeesCount");
        }

        [Test, Order(7)]
        public void TestDepartmentSenioritiesCount()
        {
            UpdateTextUI("Running TestDepartmentSenioritiesCount");
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                foreach (var departmentSeniority in departmentData.Seniorities)
                {
                    int seniorityCount = employeeManager.GetSeniorityCount(departmentData.DepartmentType, departmentSeniority.Level);

                    Assert.AreEqual(seniorityCount, departmentSeniority.Employees);
                }
            }

            UpdateTextUI("Finished TestDepartmentSenioritiesCount");
        }

        [Test, Order(8)]
        public void TestSalaryIncrementPercentage()
        {
            UpdateTextUI("Running TestSalaryIncrementPercentage");
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                foreach (var departmentSeniority in departmentData.Seniorities)
                {
                    double incrementPercentage = employeeManager.GetEmployeeSalaryIncrement(departmentData.DepartmentType, departmentSeniority.Level);
                    Assert.AreEqual(incrementPercentage, departmentSeniority.IncrementPercentage);
                }
            }
           
            UpdateTextUI("Finished TestSalaryIncrementPercentage");
        }
        
        [UnityTest, Order(9)]
        public IEnumerator TestSalaryIncrement()
        {
            UpdateTextUI("Running TestSalaryIncrement");
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (employeeManager.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
                {
                    foreach (var employee in departmentEmployees)
                    {
                        DepartmentSeniority seniority = departmentData.Seniorities.Find(seniority => seniority.Level == employee.Seniority.Level);

                        double incrementedSalary = employeeManager.CalculateUpdatedSalary(employee.Seniority.BaseSalary, employee.Seniority.IncrementPercentage);
                        Assert.AreEqual(incrementedSalary, seniority.ExpectedSalary);
                    }
                }
            }
            
            UpdateTextUI("Finished TestSalaryIncrement");
            yield return new WaitForSeconds(3f);        
        }
        #endregion
        
        #region PRIVATE_METHODS
        private void UpdateTextUI(string text)
        {
            if (testsUI != null)
            {
                testsUI.UpdateText(text);
            }
        }
        
        private void LoadCompanyData()
        {
            string scriptableObjectName = "Company_TechChallenge";
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CompanyData)} {scriptableObjectName}");
            
            if (guids.Length == 0)
            {                
                Assert.Fail($"No {nameof(CompanyData)} found named {scriptableObjectName}");
            }

            if (guids.Length > 1)
            {
                Debug.LogWarning($"More than one {nameof(CompanyData)} found named {scriptableObjectName}, taking first one");
            }

            companyData = (CompanyData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(CompanyData));
        }
        
        private void Shuffle<T>(List<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Random.Range(0, n + 1); 
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        
        private List<Employee> GenerateEmployees(CompanyData companyData)
        {
            List<Employee> newEmployees = new List<Employee>();
            
            foreach (var department in companyData.DepartmentData)
            {
                foreach (var seniority in department.Seniorities)
                {
                    for (int i = 0; i < seniority.Employees; i++)
                    {
                        newEmployees.Add(new Employee(department.DepartmentType, seniority));
                    }
                }
            }

            return newEmployees;
        }
        #endregion
    }
}