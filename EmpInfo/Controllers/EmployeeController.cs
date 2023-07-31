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
            return View();
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
        public async Task<IActionResult> AddEmployee(int selectedid)
        {
            
            EmployeeViewModel employee = new EmployeeViewModel();
            return PartialView("Partial/_AddEmployee", employee);
        }
        [HttpPost]
        public async Task<IActionResult> saveEmployee(EmployeeViewModel employeeViewModel)
        {
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
                    command.Parameters.AddWithValue("id", employeeViewModel.Id == 0 ? employeeViewModel.Id + 2 : 0);
                    command.Parameters.AddWithValue("@first_name", employeeViewModel.FirstName);
                    command.Parameters.AddWithValue("@last_name", employeeViewModel.LastName);
                    command.Parameters.AddWithValue("@City", employeeViewModel.City);
                    command.Parameters.AddWithValue("@Zip", employeeViewModel.Zip);
                    command.Parameters.AddWithValue("@CreateDate", employeeViewModel.CreatedDate);
                    command.ExecuteNonQuery();
                    connection.Close();
                }

                return Ok("DataSaved");
            }
            catch (Exception ex)
            {
                return Ok(ex.Message.ToString());
            }
        }
    }
}
