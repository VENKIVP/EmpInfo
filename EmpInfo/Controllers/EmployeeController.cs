using EmpInfo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using System.Xml.Linq;

namespace EmpInfo.Controllers { 
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            string connectionString = "Server=localhost;Database=Employee;Trusted_Connection=True";

            SqlConnection sqlConnection = new SqlConnection(connectionString);

            sqlConnection.Open();

            SqlCommand sqlCommand = new SqlCommand("SELECT COUNT(*) FROM [dbo].[EmpInfoTable]");

            sqlCommand.Connection = sqlConnection;

            int RecordCount = Convert.ToInt32(sqlCommand.ExecuteScalar());
            SqlCommand sqlCommand1 = new SqlCommand("SELECT COUNT(*) FROM [dbo].[EmpSalary1]");

            sqlCommand1.Connection = sqlConnection;

            int RecordCount1 = Convert.ToInt32(sqlCommand1.ExecuteScalar());

            EmployeeViewModel viewModel = new EmployeeViewModel();
            viewModel.Total = RecordCount;
            viewModel.Salary.Total = RecordCount1;
            
            return View(viewModel);
        }
        [HttpGet]
        public List<EmployeeViewModel> DatatableSearch()
        {
            string query = "SELECT * FROM dbo.EmpInfoTable";

            using (SqlConnection sqlConn = new SqlConnection("Server=localhost;Database=Employee;Trusted_Connection=True;"))
            using (SqlCommand cmd = new SqlCommand(query, sqlConn))
            {
                sqlConn.Open();
                DataTable dt = new DataTable();
                dt.Load(cmd.ExecuteReader());
                List<EmployeeViewModel> data = new List<EmployeeViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    EmployeeViewModel dataItem = new EmployeeViewModel
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        City = row["City"].ToString(),
                        Zip = row["Zip"].ToString(),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    };
                    data.Add(dataItem);
                }
                return data;
            }
        }
        
        [HttpGet]
        public IActionResult AddEmployee(int id)
        {
            ReturnArgs r = new ReturnArgs();
            EmployeeViewModel employee = new EmployeeViewModel();
            if (id > 0)
            {
                SqlConnection conn = new SqlConnection("Server=localhost;Database=Employee;Trusted_Connection=True;");
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM dbo.EmpInfoTable where ID ="+id, conn);
                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                List<EmployeeViewModel> data = new List<EmployeeViewModel>();
                foreach (DataRow row in dt.Rows)
                {
                    EmployeeViewModel dataItem = new EmployeeViewModel
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        FirstName = row["FirstName"].ToString(),
                        LastName = row["LastName"].ToString(),
                        City = row["City"].ToString(),
                        Zip = row["Zip"].ToString(),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    };
                    data.Add(dataItem);
                }
                employee = data.FirstOrDefault();
            }
            r.View = PartialView("Partial/AddEmployee",employee);
            
            return Json(r);
        }


        [HttpGet]
        public IActionResult AddSalary(int id)
        {
            ReturnArgs r = new ReturnArgs();
            EmployeeViewModel employee = new EmployeeViewModel();
            if (id > 0)
            {
                SqlConnection conn = new SqlConnection("Server=localhost;Database=Employee;Trusted_Connection=True;");
                conn.Open();

                SqlCommand command = new SqlCommand("SELECT * FROM dbo.EmpSalary1 where Id =" + id, conn);
                DataTable dt = new DataTable();
                dt.Load(command.ExecuteReader());
                List<EmployeeSalary> data = new List<EmployeeSalary>();
                foreach (DataRow row in dt.Rows)
                {
                    EmployeeSalary dataItem = new EmployeeSalary
                    {
                        Id = Convert.ToInt32(row["Id"]),
                        EmployeeId = Convert.ToInt32(row["EmployeeId"]),
                        Amount = Convert.ToDecimal(row["Amount"]),
                        SalaryDate = Convert.ToDateTime(row["SalaryDate"]),
                        CreatedDate = Convert.ToDateTime(row["CreatedDate"])
                    };
                    data.Add(dataItem);
                }
                employee.Salary = data.FirstOrDefault();
            }
            r.View = PartialView("Partial/AddEmployeeSalary", employee);

            return Json(r);
        }
        [HttpPost]
        public int saveEmployee(EmployeeViewModel employeeViewModel)
        {
            int id = 0;
            employeeViewModel.Id = employeeViewModel.Id + 1;
            try
            {
                using (SqlConnection connection = new("Server=localhost;Database=Employee;Trusted_Connection=True;"))
                {
                    connection.Open();

                    SqlCommand command = new("dbo.EmployeeList", connection)
                    {
                        CommandTimeout = 600,
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@id", employeeViewModel.Id);
                    command.Parameters.AddWithValue("@first_name", employeeViewModel.FirstName);
                    command.Parameters.AddWithValue("@last_name", employeeViewModel.LastName);
                    command.Parameters.AddWithValue("@City", employeeViewModel.City);
                    command.Parameters.AddWithValue("@Zip", employeeViewModel.Zip);
                    command.Parameters.AddWithValue("@CreateDate", employeeViewModel.CreatedDate);
                    id = command.ExecuteNonQuery();
                    connection.Close();
                    if (id == 1)
                        id = employeeViewModel.Id + 1;
                }

                return id;
            }
            catch (Exception ex)
            {
                return id;
            }
        }

        [HttpPost]
        public int saveSalary(EmployeeViewModel employeeViewModel)
        {
            int id = 0;
            employeeViewModel.Salary.Id = employeeViewModel.Salary.Id + 1;
            try
            {
                using (SqlConnection connection = new("Server=localhost;Database=Employee;Trusted_Connection=True;"))
                {
                    connection.Open();

                    SqlCommand command = new("dbo.EmployeeSalaryList", connection)
                    {
                        CommandTimeout = 600,
                        CommandType = CommandType.StoredProcedure
                    };
                    command.Parameters.AddWithValue("@Id", employeeViewModel.Salary.Id);
                    command.Parameters.AddWithValue("@EmployeeId", employeeViewModel.Salary.EmployeeId);
                    command.Parameters.AddWithValue("@Amount", employeeViewModel.Salary.Amount);
                    command.Parameters.AddWithValue("@SalaryDate", employeeViewModel.Salary.SalaryDate);
                    command.Parameters.AddWithValue("@CreateDate", employeeViewModel.Salary.CreatedDate);
                    id = command.ExecuteNonQuery();
                    connection.Close();
                    if (id == 1)
                        id = employeeViewModel.Salary.Id + 1;
                }

                return id;
            }
            catch (Exception ex)
            {
                return id;
            }
        }
    }
}
