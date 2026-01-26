using BlazorApp1.Data;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data;

namespace BlazorApp1.Services
{
    public class ProjectManagementService
    {
        private readonly string _connectionString;

        public ProjectManagementService(string connectionString)
        {
            _connectionString = connectionString ?? string.Empty;
        }

        /// <summary>
        /// 获取所有项目数据（同步版本）
        /// </summary>
        /// <returns></returns>
        public IEnumerable<MESProjectManagement> GetAllProjects()
        {
            Console.WriteLine("GetAllProjects called");
            
            // 验证连接字符串是否有效
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("ERROR: Connection string is null or empty");
                return Array.Empty<MESProjectManagement>();
            }
            
            try
            {
                Console.WriteLine("=== GetAllProjects Execution Start ===");
                Console.WriteLine($"Connection string: {_connectionString}");
                
                using var connection = new SqlConnection(_connectionString);
                connection.Open();
                
                var sql = "SELECT * FROM MESProjectManagement";
                var command = new SqlCommand(sql, connection);
                
                Console.WriteLine($"Executing SQL: {sql}");
                
                var projects = new List<MESProjectManagement>();
                using var reader = command.ExecuteReader();
                
                Console.WriteLine($"Reader has rows: {reader.HasRows}");
                
                // 使用固定列索引，根据日志确认的列顺序
                while (reader.Read())
                {
                    var project = new MESProjectManagement
                    {
                        ProjectName = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim(),
                        ProjectCode = reader.IsDBNull(1) ? "" : reader.GetString(1).Trim(),
                        MoldCode = reader.IsDBNull(2) ? "" : reader.GetString(2).Trim(),
                        Industry = reader.IsDBNull(3) ? "" : reader.GetString(3).Trim(),
                        CustomerInfo = reader.IsDBNull(4) ? "" : reader.GetString(4).Trim(),
                        ProductName = reader.IsDBNull(5) ? "" : reader.GetString(5).Trim(),
                        InternalLeadTime = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                        CustomerLeadTime = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                        AccuracyLevel = reader.IsDBNull(8) ? "" : reader.GetString(8).Trim(),
                        Priority = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                        ProjectStatus = reader.IsDBNull(10) ? "" : reader.GetString(10).Trim()
                    };
                    projects.Add(project);
                    Console.WriteLine($"Read project: ProjectName={project.ProjectName}, ProjectCode={project.ProjectCode}, ProjectStatus={project.ProjectStatus}");
                }
                
                Console.WriteLine($"Successfully retrieved {projects.Count} projects from database");
                Console.WriteLine("=== GetAllProjects Execution End ===");
                return projects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllProjects: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Array.Empty<MESProjectManagement>();
            }
        }

        /// <summary>
        /// 获取所有项目数据（异步版本）
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<MESProjectManagement>> GetAllProjectsAsync()
        {
            Console.WriteLine("GetAllProjectsAsync called");
            
            // 验证连接字符串是否有效
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("ERROR: Connection string is null or empty");
                return Array.Empty<MESProjectManagement>();
            }
            
            try
            {
                Console.WriteLine("=== GetAllProjectsAsync Execution Start ===");
                Console.WriteLine($"Connection string: {_connectionString}");
                
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var sql = "SELECT * FROM MESProjectManagement";
                var command = new SqlCommand(sql, connection);
                
                Console.WriteLine($"Executing SQL: {sql}");
                
                var projects = new List<MESProjectManagement>();
                using var reader = await command.ExecuteReaderAsync();
                
                Console.WriteLine($"Reader has rows: {reader.HasRows}");
                
                // 使用固定列索引，根据日志确认的列顺序
                while (await reader.ReadAsync())
                {
                    var project = new MESProjectManagement
                    {
                        ProjectName = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim(),
                        ProjectCode = reader.IsDBNull(1) ? "" : reader.GetString(1).Trim(),
                        MoldCode = reader.IsDBNull(2) ? "" : reader.GetString(2).Trim(),
                        Industry = reader.IsDBNull(3) ? "" : reader.GetString(3).Trim(),
                        CustomerInfo = reader.IsDBNull(4) ? "" : reader.GetString(4).Trim(),
                        ProductName = reader.IsDBNull(5) ? "" : reader.GetString(5).Trim(),
                        InternalLeadTime = reader.IsDBNull(6) ? null : reader.GetDateTime(6),
                        CustomerLeadTime = reader.IsDBNull(7) ? null : reader.GetDateTime(7),
                        AccuracyLevel = reader.IsDBNull(8) ? "" : reader.GetString(8).Trim(),
                        Priority = reader.IsDBNull(9) ? null : reader.GetInt32(9),
                        ProjectStatus = reader.IsDBNull(10) ? "" : reader.GetString(10).Trim()
                    };
                    projects.Add(project);
                    Console.WriteLine($"Read project: ProjectName={project.ProjectName}, ProjectCode={project.ProjectCode}, ProjectStatus={project.ProjectStatus}");
                }
                
                Console.WriteLine($"Successfully retrieved {projects.Count} projects from database");
                Console.WriteLine("=== GetAllProjectsAsync Execution End ===");
                return projects;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllProjectsAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return Array.Empty<MESProjectManagement>();
            }
        }

        /// <summary>
        /// 新增项目
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public async Task<int> AddProjectAsync(MESProjectManagement project)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return 0;
            }
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var sql = @"INSERT INTO MESProjectManagement 
                           (ProjectName, ProjectCode, MoldCode, Industry, CustomerInfo, ProductName, InternalLeadTime, CustomerLeadTime, AccuracyLevel, Priority, ProjectStatus)
                           VALUES (@ProjectName, @ProjectCode, @MoldCode, @Industry, @CustomerInfo, @ProductName, @InternalLeadTime, @CustomerLeadTime, @AccuracyLevel, @Priority, @ProjectStatus)";
                
                using var command = new SqlCommand(sql, connection);
                
                // 手动添加参数，确保类型匹配
                command.Parameters.AddWithValue("@ProjectName", project.ProjectName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProjectCode", project.ProjectCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MoldCode", project.MoldCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Industry", project.Industry ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CustomerInfo", project.CustomerInfo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProductName", project.ProductName ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@InternalLeadTime", project.InternalLeadTime.HasValue ? (object)project.InternalLeadTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@CustomerLeadTime", project.CustomerLeadTime.HasValue ? (object)project.CustomerLeadTime.Value : DBNull.Value);
                command.Parameters.AddWithValue("@AccuracyLevel", project.AccuracyLevel ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Priority", project.Priority.HasValue ? (object)project.Priority.Value : DBNull.Value);
                command.Parameters.AddWithValue("@ProjectStatus", project.ProjectStatus ?? (object)DBNull.Value);
                
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddProjectAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 更新项目
        /// </summary>
        /// <param name="project"></param>
        /// <param name="originalProjectCode">原始项目代码，用于定位要更新的记录</param>
        /// <returns></returns>
        public async Task<int> UpdateProjectAsync(MESProjectManagement project, string originalProjectCode)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                Console.WriteLine("UpdateProjectAsync: Connection string is null or empty");
                return 0;
            }
            
            try
            {
                int result = 0; // 在外部声明result变量
                Console.WriteLine($"===== UpdateProjectAsync Start ====");
                Console.WriteLine($"Original ProjectCode: '{originalProjectCode}' (Length: {originalProjectCode?.Length})");
                Console.WriteLine($"New ProjectCode: '{project.ProjectCode}' (Length: {project.ProjectCode?.Length})");
                Console.WriteLine($"ProjectName: '{project.ProjectName}'");
                Console.WriteLine($"ProjectStatus: '{project.ProjectStatus}'");
                Console.WriteLine($"MoldCode: '{project.MoldCode}'");
                Console.WriteLine($"Industry: '{project.Industry}'");
                Console.WriteLine($"CustomerInfo: '{project.CustomerInfo}'");
                Console.WriteLine($"ProductName: '{project.ProductName}'");
                Console.WriteLine($"InternalLeadTime: {project.InternalLeadTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL"}");
                Console.WriteLine($"CustomerLeadTime: {project.CustomerLeadTime?.ToString("yyyy-MM-dd HH:mm:ss") ?? "NULL"}");
                Console.WriteLine($"AccuracyLevel: '{project.AccuracyLevel}'");
                Console.WriteLine($"Priority: {project.Priority?.ToString() ?? "NULL"}");
                
                using var connection = new SqlConnection(_connectionString);
                Console.WriteLine("Opening database connection...");
                await connection.OpenAsync();
                Console.WriteLine("Connection opened successfully");
                Console.WriteLine($"Connection State: {connection.State}");
                
                // 调试：获取表的实际列名
                var columnsSql = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MESProjectManagement'";
                using var columnsCommand = new SqlCommand(columnsSql, connection);
                using var columnsReader = await columnsCommand.ExecuteReaderAsync();
                Console.WriteLine("Actual column names in MESProjectManagement table:");
                while (columnsReader.Read())
                {
                    var columnName = columnsReader.GetString(0);
                    Console.WriteLine($"  - '{columnName}'");
                }
                columnsReader.Close();
                
                // 先检查数据库中是否存在原始ProjectCode
                // 使用实际列名（从建表语句确认）
                var checkSql = "SELECT COUNT(*) FROM MESProjectManagement WHERE [ProjectCode] = @OriginalProjectCode";
                using var checkCommand = new SqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@OriginalProjectCode", originalProjectCode ?? (object)DBNull.Value);
                var scalarResult = await checkCommand.ExecuteScalarAsync();
                var count = scalarResult != null ? (int)scalarResult : 0;
                Console.WriteLine($"Check for Original ProjectCode '{originalProjectCode}': {count} rows found");
                
                // 如果找到记录，执行UPDATE
                if (count > 0)
                {
                    // 使用ADO.NET SqlCommand，避免Dapper的自动映射问题
                    var sql = @"UPDATE MESProjectManagement 
                               SET [ProjectName] = @ProjectName, 
                                   [ProjectCode] = @ProjectCode, 
                                   [MoldCode] = @MoldCode, 
                                   [Industry] = @Industry, 
                                   [CustomerInfo] = @CustomerInfo, 
                                   [ProductName] = @ProductName, 
                                   [InternalLeadTime] = @InternalLeadTime, 
                                   [CustomerLeadTime] = @CustomerLeadTime, 
                                   [AccuracyLevel] = @AccuracyLevel, 
                                   [Priority] = @Priority, 
                                   [ProjectStatus] = @ProjectStatus 
                               WHERE [ProjectCode] = @OriginalProjectCode";
                    
                    Console.WriteLine($"Executing SQL: {sql}");
                    
                    using var command = new SqlCommand(sql, connection);
                    
                    // 手动添加参数，确保类型匹配
                    command.Parameters.AddWithValue("@ProjectName", project.ProjectName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProjectCode", project.ProjectCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MoldCode", project.MoldCode ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Industry", project.Industry ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerInfo", project.CustomerInfo ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ProductName", project.ProductName ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@InternalLeadTime", project.InternalLeadTime.HasValue ? (object)project.InternalLeadTime.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@CustomerLeadTime", project.CustomerLeadTime.HasValue ? (object)project.CustomerLeadTime.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@AccuracyLevel", project.AccuracyLevel ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Priority", project.Priority.HasValue ? (object)project.Priority.Value : DBNull.Value);
                    command.Parameters.AddWithValue("@ProjectStatus", project.ProjectStatus ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@OriginalProjectCode", originalProjectCode ?? (object)DBNull.Value);
                    
                    Console.WriteLine("Parameters added, executing query...");
                    
                    result = await command.ExecuteNonQueryAsync();
                    Console.WriteLine($"Update result: {result} rows affected");
                }
                else
                {
                    Console.WriteLine($"ERROR: No records found with Original ProjectCode '{originalProjectCode}'");
                    // 尝试查找所有项目代码，看看实际值是什么
                    var getAllCodesSql = "SELECT [ProjectCode] FROM MESProjectManagement";
                    using var getAllCodesCommand = new SqlCommand(getAllCodesSql, connection);
                    using var reader = await getAllCodesCommand.ExecuteReaderAsync();
                    Console.WriteLine("Available ProjectCodes in database:");
                    while (reader.Read())
                    {
                        var dbCode = reader.GetString(0);
                        Console.WriteLine($"  - '{dbCode}' (Length: {dbCode.Length})");
                    }
                }
                Console.WriteLine($"===== UpdateProjectAsync End =====");
                
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateProjectAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine($"===== UpdateProjectAsync Error =====");
                return 0;
            }
        }

        /// <summary>
        /// 删除项目
        /// </summary>
        /// <param name="projectCode"></param>
        /// <returns></returns>
        public async Task<int> DeleteProjectAsync(string projectCode)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return 0;
            }
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                var sql = "DELETE FROM MESProjectManagement WHERE [ProjectCode] = @ProjectCode";
                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ProjectCode", projectCode ?? (object)DBNull.Value);
                
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteProjectAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
            }
        }

        /// <summary>
        /// 批量导入项目数据
        /// </summary>
        /// <param name="projects"></param>
        /// <returns></returns>
        public async Task<int> BulkImportProjectsAsync(IEnumerable<MESProjectManagement> projects)
        {
            if (string.IsNullOrEmpty(_connectionString) || projects == null)
            {
                return 0;
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var transaction = connection.BeginTransaction();

                try
                {
                    var dataTable = new DataTable();

                    // 添加列到DataTable
                    dataTable.Columns.Add("ProjectName", typeof(string));
                    dataTable.Columns.Add("ProjectCode", typeof(string));
                    dataTable.Columns.Add("MoldCode", typeof(string));
                    dataTable.Columns.Add("Industry", typeof(string));
                    dataTable.Columns.Add("CustomerInfo", typeof(string));
                    dataTable.Columns.Add("ProductName", typeof(string));
                    dataTable.Columns.Add("InternalLeadTime", typeof(DateTime));
                    dataTable.Columns.Add("CustomerLeadTime", typeof(DateTime));
                    dataTable.Columns.Add("AccuracyLevel", typeof(string));
                    dataTable.Columns.Add("Priority", typeof(int));
                    dataTable.Columns.Add("ProjectStatus", typeof(string));

                    // 填充DataTable
                    foreach (var project in projects)
                    {
                        var row = dataTable.NewRow();
                        row["ProjectName"] = string.IsNullOrEmpty(project.ProjectName) ? DBNull.Value : (object)project.ProjectName;
                        row["ProjectCode"] = string.IsNullOrEmpty(project.ProjectCode) ? DBNull.Value : (object)project.ProjectCode;
                        row["MoldCode"] = string.IsNullOrEmpty(project.MoldCode) ? DBNull.Value : (object)project.MoldCode;
                        row["Industry"] = string.IsNullOrEmpty(project.Industry) ? DBNull.Value : (object)project.Industry;
                        row["CustomerInfo"] = string.IsNullOrEmpty(project.CustomerInfo) ? DBNull.Value : (object)project.CustomerInfo;
                        row["ProductName"] = string.IsNullOrEmpty(project.ProductName) ? DBNull.Value : (object)project.ProductName;
                        row["InternalLeadTime"] = project.InternalLeadTime.HasValue ? (object)project.InternalLeadTime.Value : DBNull.Value;
                        row["CustomerLeadTime"] = project.CustomerLeadTime.HasValue ? (object)project.CustomerLeadTime.Value : DBNull.Value;
                        row["AccuracyLevel"] = string.IsNullOrEmpty(project.AccuracyLevel) ? DBNull.Value : (object)project.AccuracyLevel;
                        row["Priority"] = project.Priority.HasValue ? (object)project.Priority.Value : DBNull.Value;
                        row["ProjectStatus"] = string.IsNullOrEmpty(project.ProjectStatus) ? DBNull.Value : (object)project.ProjectStatus;

                        dataTable.Rows.Add(row);
                    }

                    // 使用SqlBulkCopy批量插入数据
                    using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
                    bulkCopy.DestinationTableName = "MESProjectManagement";
                    bulkCopy.BatchSize = 1000;

                    // 映射列
                    bulkCopy.ColumnMappings.Add("ProjectName", "ProjectName");
                    bulkCopy.ColumnMappings.Add("ProjectCode", "ProjectCode");
                    bulkCopy.ColumnMappings.Add("MoldCode", "MoldCode");
                    bulkCopy.ColumnMappings.Add("Industry", "Industry");
                    bulkCopy.ColumnMappings.Add("CustomerInfo", "CustomerInfo");
                    bulkCopy.ColumnMappings.Add("ProductName", "ProductName");
                    bulkCopy.ColumnMappings.Add("InternalLeadTime", "InternalLeadTime");
                    bulkCopy.ColumnMappings.Add("CustomerLeadTime", "CustomerLeadTime");
                    bulkCopy.ColumnMappings.Add("AccuracyLevel", "AccuracyLevel");
                    bulkCopy.ColumnMappings.Add("Priority", "Priority");
                    bulkCopy.ColumnMappings.Add("ProjectStatus", "ProjectStatus");

                    await bulkCopy.WriteToServerAsync(dataTable);
                    await transaction.CommitAsync();

                    return dataTable.Rows.Count;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Console.WriteLine($"Error in BulkImportProjectsAsync: {ex.Message}");
                    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    return 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in BulkImportProjectsAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return 0;
            }
        }
    }
}