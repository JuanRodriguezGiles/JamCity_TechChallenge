using System;

using UnityEngine;

namespace TechChallenge.Scripts.Data
{
    #region ENUM
    public enum SeniorityLevel { JUNIOR, SEMI_SENIOR, SENIOR }
    #endregion

    #region PUBLIC_CLASSES
    [Serializable]
    public class Seniority
    {
        #region EXPOSED_FIELDS
        [SerializeField] private SeniorityLevel level = default;
        [SerializeField] private double baseSalary = 0;
        [SerializeField] private double incrementPercentage = 0;
        #endregion

        #region PROPERTIES
        public SeniorityLevel Level { get => level; }
        public double BaseSalary { get => baseSalary; }
        public double IncrementPercentage { get => incrementPercentage; }
        #endregion

        #region CONSTURCTOR
        public Seniority(SeniorityLevel level, double baseSalary, double incrementPercentage, double expectedSalary)
        {
            this.level = level;
            this.baseSalary = baseSalary;
            this.incrementPercentage = incrementPercentage;
        }
        #endregion
    }
    #endregion
}