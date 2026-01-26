using BlazorApp1.Pages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data;
using OfficeOpenXml;
using System.IO;

namespace BlazorApp1.Services
{
    public class BOMManagementService
    {
        private readonly string _connectionString;

        public BOMManagementService(string connectionString)
        {
            _connectionString = connectionString ?? string.Empty;
        }

        /// <summary>
        /// 获取所有BOM数据
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<BomManagement.BOMData>> GetAllBOMsAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return Array.Empty<BomManagement.BOMData>();
            }
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                // 先获取表的列信息，以便正确映射
                var columnsSql = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'MESBOMManagement'";
                var columnsCommand = new SqlCommand(columnsSql, connection);
                var columns = new List<string>();
                using (var columnsReader = await columnsCommand.ExecuteReaderAsync())
                {
                    while (columnsReader.Read())
                    {
                        columns.Add(columnsReader.GetString(0));
                    }
                }
                
                Console.WriteLine("MESBOMManagement表的列名：");
                foreach (var column in columns)
                {
                    Console.WriteLine($"  - {column}");
                }
                
                var sql = "SELECT * FROM MESBOMManagement";
                var command = new SqlCommand(sql, connection);
                
                var boms = new List<BomManagement.BOMData>();
                using var reader = await command.ExecuteReaderAsync();
                
                while (reader.Read())
                {
                    var bom = new BomManagement.BOMData
                    {
                        Id = 0, // 数据库表中没有Id字段，使用0占位
                        Drawing = "", // 数据库表中没有Drawing字段
                        ImageData = "", // 数据库表中没有ImageData字段
                        ReportStatus = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim(),
                        ReportStatusClass = GetReportStatusClass(reader.IsDBNull(0) ? "" : reader.GetString(0).Trim()),
                        Project = reader.IsDBNull(1) ? "" : reader.GetString(1).Trim(), // ProjectCode列
                        PartNumber = reader.IsDBNull(2) ? "" : reader.GetString(2).Trim(), // PartNumber现在是nvarchar类型
                        WorkOrderType = reader.IsDBNull(3) ? "" : reader.GetString(3).Trim(), // PartType列
                        WorkOrderNumber = reader.IsDBNull(4) ? "" : reader.GetString(4).Trim(),
                        Material = reader.IsDBNull(5) ? "" : reader.GetString(5).Trim(),
                        BarcodeInfo = reader.IsDBNull(6) ? "" : reader.GetString(6).Trim(), // Barcode列
                        ProcessPersonnel = reader.IsDBNull(7) ? "" : reader.GetString(7).Trim(), // Worker列
                        IsSelected = false,
                        // 设置原始值字段，用于后续的删除和更新操作
                        OriginalProject = reader.IsDBNull(1) ? "" : reader.GetString(1).Trim(),
                        OriginalPartNumber = reader.IsDBNull(2) ? "" : reader.GetString(2).Trim(),
                        OriginalWorkOrderNumber = reader.IsDBNull(4) ? "" : reader.GetString(4).Trim()
                    };
                    boms.Add(bom);
                }
                
                return boms;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAllBOMsAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return Array.Empty<BomManagement.BOMData>();
            }
        }

        /// <summary>
        /// 更新BOM数据
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public async Task<int> UpdateBOMAsync(BomManagement.BOMData bom)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return 0;
            }
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                // 根据项目代码、零件号和工单号来定位记录（假设这三个字段组合是唯一的）
                var sql = @"UPDATE MESBOMManagement 
                           SET [ReportStatus] = @ReportStatus, 
                               [ProjectCode] = @ProjectCode, 
                               [PartNumber] = @PartNumber, 
                               [PartType] = @PartType, 
                               [WorkOrderNumber] = @WorkOrderNumber, 
                               [Material] = @Material, 
                               [Barcode] = @Barcode, 
                               [Worker] = @Worker 
                           WHERE [ProjectCode] = @OriginalProjectCode 
                           AND [PartNumber] = @OriginalPartNumber 
                           AND [WorkOrderNumber] = @OriginalWorkOrderNumber";
                
                using var command = new SqlCommand(sql, connection);
                
                // 手动添加参数，确保类型匹配
                command.Parameters.AddWithValue("@ReportStatus", bom.ReportStatus ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@ProjectCode", bom.Project ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PartNumber", bom.PartNumber ?? (object)DBNull.Value); // PartNumber现在是nvarchar类型，直接使用字符串
                command.Parameters.AddWithValue("@PartType", bom.WorkOrderType ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@WorkOrderNumber", bom.WorkOrderNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Material", bom.Material ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Barcode", bom.BarcodeInfo ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Worker", bom.ProcessPersonnel ?? (object)DBNull.Value);
                
                // 原始值，用于定位要更新的记录
                command.Parameters.AddWithValue("@OriginalProjectCode", bom.OriginalProject ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OriginalPartNumber", bom.OriginalPartNumber ?? (object)DBNull.Value); // 使用原始的PartNumber值
                command.Parameters.AddWithValue("@OriginalWorkOrderNumber", bom.OriginalWorkOrderNumber ?? (object)DBNull.Value);
                
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in UpdateBOMAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 删除BOM数据
        /// </summary>
        /// <param name="bom"></param>
        /// <returns></returns>
        public async Task<int> DeleteBOMAsync(BomManagement.BOMData bom)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return 0;
            }
            
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                
                // 根据项目代码、零件号和工单号来定位记录（假设这三个字段组合是唯一的）
                var sql = @"DELETE FROM MESBOMManagement 
                           WHERE [ProjectCode] = @OriginalProjectCode 
                           AND [PartNumber] = @OriginalPartNumber 
                           AND [WorkOrderNumber] = @OriginalWorkOrderNumber";
                
                using var command = new SqlCommand(sql, connection);
                
                // 使用原始值定位要删除的记录
                command.Parameters.AddWithValue("@OriginalProjectCode", bom.OriginalProject ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OriginalPartNumber", bom.OriginalPartNumber ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@OriginalWorkOrderNumber", bom.OriginalWorkOrderNumber ?? (object)DBNull.Value);
                
                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in DeleteBOMAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 获取报工状态的样式类
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private string GetReportStatusClass(string status)
        {
            switch (status)
            {
                case "已报工":
                    return "badge-success";
                case "未报工":
                    return "badge-secondary";
                default:
                    return "badge-secondary";
            }
        }

        /// <summary>
        /// 从Excel文件导入BOM数据
        /// </summary>
        /// <param name="fileStream">Excel文件流</param>
        /// <returns>导入成功的记录数</returns>
        public async Task<int> ImportBOMsFromExcelAsync(Stream fileStream)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return 0;
            }

            try
            {
                // 设置EPPlus使用非商业许可证
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                // 将文件流异步复制到内存流
                using var memoryStream = new MemoryStream();
                await fileStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0; // 重置流位置到开始
                
                using var package = new ExcelPackage(memoryStream);
                var worksheet = package.Workbook.Worksheets[0]; // 获取第一个工作表
                var rowCount = worksheet.Dimension.Rows;
                var importedCount = 0;

                // 使用SqlBulkCopy进行批量导入
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();
                using var bulkCopy = new SqlBulkCopy(connection)
                {
                    DestinationTableName = "MESBOMManagement"
                };

                // 创建DataTable用于批量导入
                var dataTable = new DataTable();
                dataTable.Columns.Add("ReportStatus", typeof(string));
                dataTable.Columns.Add("ProjectCode", typeof(string));
                dataTable.Columns.Add("PartNumber", typeof(string));
                dataTable.Columns.Add("PartType", typeof(string));
                dataTable.Columns.Add("WorkOrderNumber", typeof(string));
                dataTable.Columns.Add("Material", typeof(string));
                dataTable.Columns.Add("Barcode", typeof(string));
                dataTable.Columns.Add("Worker", typeof(string));

                // 从第二行开始读取数据（第一行是标题）
                for (int row = 2; row <= rowCount; row++)
                {
                    var dataRow = dataTable.NewRow();
                    
                    // 映射Excel列到DataTable
                    // Excel列：项目代码, 报工状态, 零件号, 工单类型, 工单号, 材质, BAR-条形码, 工艺人员
                    string cellValue;
                    
                    cellValue = worksheet.Cells[row, 1].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["ProjectCode"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 2].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["ReportStatus"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 3].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["PartNumber"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 4].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["PartType"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 5].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["WorkOrderNumber"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 6].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["Material"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 7].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["Barcode"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;
                    
                    cellValue = worksheet.Cells[row, 8].Value?.ToString()?.Trim() ?? string.Empty;
                    dataRow["Worker"] = string.IsNullOrEmpty(cellValue) ? DBNull.Value : cellValue;

                    dataTable.Rows.Add(dataRow);
                    importedCount++;
                }

                // 执行批量导入
                await bulkCopy.WriteToServerAsync(dataTable);
                return importedCount;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ImportBOMsFromExcelAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                throw;
            }
        }
    }
}