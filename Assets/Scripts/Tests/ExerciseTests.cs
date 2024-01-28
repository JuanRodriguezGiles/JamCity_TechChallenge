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
        #endregion
        
        #region PUBLIC_METHODS
        [UnityTest]
        public IEnumerator LoadTestScene()
        {
            SceneManager.LoadSceneAsync("TechChallenge");
            yield return null;
        }
        
        [UnityTest]
        public IEnumerator TestExercise1()
        {
            TestsUI testsUI = Object.FindObjectOfType<TestsUI>();
            testsUI.UpdateText("Running test 1");
            
            //Create test data using provided values
            companyData = LoadCompanyDataSO();
            List<Employee> employees = GenerateEmployees();

            //Check if created employees count match with intended count
            Assert.AreEqual(companyData.Employees, employees.Count);
            
            //Shuffle list to correctly test program
            Shuffle(employees);

            //Execute ordering program
            Exercise1 exercise1 = new Exercise1();
            Dictionary<DepartmentType, List<Employee>> groupedAndOrderedEmployees = exercise1.GroupAndOrderEmployees(employees);

            Assert.AreEqual(groupedAndOrderedEmployees.Count, companyData.DepartmentData.Count);
            
            foreach (KeyValuePair<DepartmentType, List<Employee>> employeesKvp in groupedAndOrderedEmployees)
            {
                for (int i = 0; i < companyData.DepartmentData.Count; i++)
                {
                    if (employeesKvp.Key == companyData.DepartmentData[i].DepartmentType)
                    {
                        //Check if department employees count matches with intended value
                        int departmentEmployees = 0;
                        for (int j = 0; j < companyData.DepartmentData[i].Seniorities.Count; j++)
                        {
                            departmentEmployees += companyData.DepartmentData[i].Seniorities[j].Employees;
                        }
                        Assert.AreEqual(employeesKvp.Value.Count, departmentEmployees);

                        Dictionary<SeniorityLevel, int> countBySeniority = exercise1.GroupBySeniority(employeesKvp.Value);
                        //Check if seniority count matches with intended values
                        foreach (KeyValuePair<SeniorityLevel, int> seniorityKvp in countBySeniority)
                        {
                            switch (seniorityKvp.Key)
                            {
                                case SeniorityLevel.JUNIOR:
                                    Assert.AreEqual(seniorityKvp.Value, companyData.DepartmentData[i].Seniorities.Find(seniority => seniority.Level == SeniorityLevel.JUNIOR).Employees);
                                    break;
                                case SeniorityLevel.SEMI_SENIOR:
                                    Assert.AreEqual(seniorityKvp.Value, companyData.DepartmentData[i].Seniorities.Find(seniority => seniority.Level == SeniorityLevel.SEMI_SENIOR).Employees);
                                    break;
                                case SeniorityLevel.SENIOR:
                                    Assert.AreEqual(seniorityKvp.Value, companyData.DepartmentData[i].Seniorities.Find(seniority => seniority.Level == SeniorityLevel.SENIOR).Employees);
                                    break;
                            }
                        }
                        
                        break;
                    }
                }
            }
            
            testsUI.UpdateText("Finished test 1");
            yield return new WaitForSeconds(1f);
        }

        [UnityTest]
        public IEnumerator TestExercise2()
        {
            TestsUI testsUI = Object.FindObjectOfType<TestsUI>();
            testsUI.UpdateText("Running test 2");
            
            companyData = LoadCompanyDataSO();
            List<Employee> employees = GenerateEmployees();

            Exercise1 exercise1 = new Exercise1();
            Dictionary<DepartmentType, List<Employee>> groupedAndOrderedEmployees = exercise1.GroupAndOrderEmployees(employees);

            foreach (KeyValuePair<DepartmentType, List<Employee>> employeesKvp in groupedAndOrderedEmployees)
            {
                for (int i = 0; i < companyData.DepartmentData.Count; i++)
                {
                    if (employeesKvp.Key == companyData.DepartmentData[i].DepartmentType)
                    {
                        for (int j = 0; j < employeesKvp.Value.Count; j++)
                        {
                            Seniority seniority = companyData.DepartmentData[i].Seniorities.Find(departmentSeniority => departmentSeniority.Level == employeesKvp.Value[j].Seniority.Level);
                            Assert.AreEqual(employeesKvp.Value[j].Seniority.IncrementPercentage, seniority.IncrementPercentage);
                        }

                        break;
                    }
                }
            }
            testsUI.UpdateText("Finished test 2");
            yield return new WaitForSeconds(1f);
        }
        
        [UnityTest]
        public IEnumerator TestExercise3()
        {
            TestsUI testsUI = Object.FindObjectOfType<TestsUI>();
            testsUI.UpdateText("Running test 3");
            
            companyData = LoadCompanyDataSO();
            List<Employee> employees = GenerateEmployees();

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
            
            Exercise1 exercise1 = new Exercise1();
            Dictionary<DepartmentType, List<Employee>> groupedAndOrderedEmployees = exercise1.GroupAndOrderEmployees(employees);

            Exercise3 exercise3 = new Exercise3();

            foreach (KeyValuePair<DepartmentType, List<Employee>> employeesKvp in groupedAndOrderedEmployees)
            {
                for (int i = 0; i < companyData.DepartmentData.Count; i++)
                {
                    if (employeesKvp.Key == companyData.DepartmentData[i].DepartmentType)
                    {
                        for (int j = 0; j < employeesKvp.Value.Count; j++)
                        {
                            DepartmentSeniority seniority = companyData.DepartmentData[i].Seniorities.Find(seniority => seniority.Level == employeesKvp.Value[j].Seniority.Level);

                            double salary = exercise3.GetUpdatedSalary(employeesKvp.Value[j].Seniority.BaseSalary, employeesKvp.Value[j].Seniority.IncrementPercentage);
                            Assert.AreEqual(salary, seniority.ExpectedSalary);
                        }
                        
                        break;
                    }
                }
            }
            
            testsUI.UpdateText("Finished test 3");
            yield return new WaitForSeconds(1f);        
        }
        #endregion
        
        #region PRIVATE_METHODS
        private List<Employee> GenerateEmployees()
        {
            List<Employee> employees = new List<Employee>();
            
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

            return employees;
        }
        
        private CompanyData LoadCompanyDataSO()
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

            return (CompanyData)AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guids[0]), typeof(CompanyData));
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