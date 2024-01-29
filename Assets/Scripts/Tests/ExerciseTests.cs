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

using Random = System.Random;

namespace TechChallenge.Scripts.Tests
{
    public class ExerciseTests 
    {
        #region PRIVATE_FIELDS
        private CompanyData companyData = null;
        private Program program = null;
        private TestsUI testsUI = null;
        #endregion
        
        #region PUBLIC_METHODS
        [UnityTest, Order(1)]
        public IEnumerator TestLoadScene()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("TechChallenge");

            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            
            testsUI = Object.FindObjectOfType<TestsUI>();
            program = new Program();
            
            Assert.IsNotNull(testsUI);
            Assert.IsNotNull(program);
        }

        [Test, Order(2)]
        public void TestLoadInputData()
        {
            UpdateTextUI("Running LoadInputData");
            
            LoadCompanyData();
            
            Assert.IsNotNull(companyData);
            
            UpdateTextUI("Finished LoadInputData");
        }

        [Test, Order(3)]
        public void TestGenerateEmployees()
        {
            UpdateTextUI("Running TestGenerateEmployees");

            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            //Check if created employees count match with intended count
            Assert.AreEqual(program.GetEmployeeCount(), companyData.Employees);

            UpdateTextUI("Finished TestGenerateEmployees");
        }

        [Test, Order(4)]
        public void TestGrouping()
        {
            UpdateTextUI("Running TestGrouping");

            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            //Shuffle list to correctly test program
            Shuffle(program.GetAllEmployees());

            program.GroupAndOrderEmployees();
            
            Assert.AreEqual(program.GetDepartmentCount(), companyData.DepartmentData.Count);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (program.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
                {
                    for (int i = 0; i < departmentEmployees.Count; i++)
                    {
                        Assert.AreEqual(departmentEmployees[i].Position, departmentData.DepartmentType);
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
            
            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (program.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
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
            
            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                int departmentEmployeesCount = 0;
                foreach (var departmentSeniority in departmentData.Seniorities)
                {
                    departmentEmployeesCount += departmentSeniority.Employees;
                }
                
                if (program.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
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
            
            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                foreach (var departmentSeniority in departmentData.Seniorities)
                {
                    int seniorityCount = program.GetSeniorityCount(departmentData.DepartmentType, departmentSeniority.Level);

                    Assert.AreEqual(seniorityCount, departmentSeniority.Employees);
                }
            }

            UpdateTextUI("Finished TestDepartmentSenioritiesCount");
        }

        [Test, Order(8)]
        public void TestSalaryIncrementPercentage()
        {
            UpdateTextUI("Running TestSalaryIncrementPercentage");
            
            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (program.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees)) 
                {
                    foreach (var employee in departmentEmployees)
                    {
                        Seniority seniority = departmentData.Seniorities.Find(departmentSeniority => departmentSeniority.Level == employee.Seniority.Level);
                        Assert.IsNotNull(seniority);
                        Assert.AreEqual(employee.Seniority.IncrementPercentage, seniority.IncrementPercentage);
                    }
                }
                else
                {
                    Assert.Fail("Department: " + departmentData.DepartmentType + " not found");
                }
            }
           
            UpdateTextUI("Finished TestSalaryIncrementPercentage");
        }
        
        [UnityTest, Order(9)]
        public IEnumerator TestSalaryIncrement()
        {
            UpdateTextUI("Running TestSalaryIncrement");
            
            LoadCompanyData();
            program = new Program();
            program.GenerateEmployees(companyData);
            
            foreach (var departmentData in companyData.DepartmentData)
            {
                if (program.TryGetDepartmentEmployees(departmentData.DepartmentType, out List<Employee> departmentEmployees))
                {
                    foreach (var employee in departmentEmployees)
                    {
                        DepartmentSeniority seniority = departmentData.Seniorities.Find(seniority => seniority.Level == employee.Seniority.Level);

                        double incrementedSalary = program.GetUpdatedSalary(employee.Seniority.BaseSalary, employee.Seniority.IncrementPercentage);
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
            //In real world scenario we can use Resources.Load 
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
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
        #endregion
    }
}