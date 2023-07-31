using Microsoft.AspNetCore.Mvc;
using System.Xml.Schema;

namespace EmpInfo.Models
{
    public class EmployeeViewModel
    {
        public EmployeeViewModel() {
            Salary = new EmployeeSalary();
        }
        public EmployeeSalary Salary { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Total { get; set; }
    }
    public class EmployeeSalary
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public DateTime SalaryDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Total { get; set; }
    }
    public class ReturnArgs
    {
        public ReturnArgs()
        {
        }

        public PartialViewResult View { get; set; }
    }
}
