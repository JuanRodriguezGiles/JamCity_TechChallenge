using System;

using UnityEngine;

namespace TechChallenge.Scripts.Data
{
    [Serializable]
    public class DepartmentSeniority : Seniority
    {
        #region PRIVATE_FIELDS
        [SerializeField] private int employees = 0;
        [SerializeField] private double expectedSalary = 0;
        #endregion

        #region PROPERTIES
        public int Employees { get => employees; }
        public double ExpectedSalary { get => expectedSalary; }
        #endregion

        #region CONSTURCTOR
        public DepartmentSeniority(SeniorityLevel level, double baseSalary, double incrementPercentage, double expectedSalary) : base(level, baseSalary, incrementPercentage, expectedSalary) { }
        #endregion
    }
}