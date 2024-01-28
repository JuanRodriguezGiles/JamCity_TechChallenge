using System;
using System.Collections.Generic;

using UnityEngine;

namespace TechChallenge.Scripts.Data
{
    [Serializable]
    [CreateAssetMenu(fileName = "Company_", menuName = "TechChallenge/Company", order = 1)]
    public class CompanyData : ScriptableObject
    {
        #region EXPOSED_FIELDS
        [SerializeField] private int employees = 0;
        [SerializeField] private List<DepartmentData> departmentsData = null;
        #endregion

        #region PROPERTIES
        public int Employees { get => employees; }
        public List<DepartmentData> DepartmentData { get => departmentsData; }
        #endregion

        #region CONSTRUCTOR
        public CompanyData()
        {
            departmentsData = new List<DepartmentData>();
            foreach (DepartmentType department in Enum.GetValues(typeof(DepartmentType)))
            {
                departmentsData.Add(new DepartmentData(department));
            }
        }
        #endregion
    }
}