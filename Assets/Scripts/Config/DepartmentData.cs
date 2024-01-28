using System;
using System.Collections.Generic;

using UnityEngine;

namespace TechChallenge.Scripts.Data
{
    [Serializable]
    public class DepartmentData 
    {
        #region EXPOSED_FIELDS
        [SerializeField] private DepartmentType departmentType = default;
        [SerializeField] private List<DepartmentSeniority> seniorities = null;
        #endregion

        #region PROPERTIES
        public DepartmentType DepartmentType { get => departmentType; }
        public List<DepartmentSeniority> Seniorities { get => seniorities; }
        #endregion

        #region CONSTURCTOR
        public DepartmentData(DepartmentType departmentType)
        {
            this.departmentType = departmentType;
        }
        #endregion
    }
}