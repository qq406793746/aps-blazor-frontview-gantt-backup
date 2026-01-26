using BlazorApp1.Pages;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Data;

namespace BlazorApp1.Services
{
    public class ProcessManagementService
    {
        private readonly string _connectionString;

        public ProcessManagementService(string connectionString)
        {
            _connectionString = connectionString ?? string.Empty;
        }

        /// <summary>
        /// 清空零件的工序信息
        /// </summary>
        public async Task<int> ClearProcessAsync(string projectCode, string moldCode, string partNumber)
        {
            if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(projectCode) || 
                string.IsNullOrEmpty(moldCode) || string.IsNullOrEmpty(partNumber))
            {
                return 0;
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"DELETE FROM MESProcessManagement 
                          WHERE ProjectCode = @ProjectCode 
                          AND MoldCode = @MoldCode 
                          AND PartNumber = @PartNumber";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ProjectCode", projectCode);
                command.Parameters.AddWithValue("@MoldCode", moldCode);
                command.Parameters.AddWithValue("@PartNumber", partNumber);

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ClearProcessAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 保存工序信息到数据库
        /// </summary>
        public async Task<int> SaveProcessAsync(ProcessManagement.BOMPartData part, List<ProcessManagement.ProcessStepData> processSteps)
        {
            if (string.IsNullOrEmpty(_connectionString) || part == null)
            {
                return 0;
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                // 检查是否已存在该零件的工艺信息
                var checkSql = @"SELECT COUNT(*) FROM MESProcessManagement 
                                WHERE ProjectCode = @ProjectCode 
                                AND MoldCode = @MoldCode 
                                AND PartNumber = @PartNumber";
                using var checkCommand = new SqlCommand(checkSql, connection);
                checkCommand.Parameters.AddWithValue("@ProjectCode", part.ProjectCode ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@MoldCode", part.MoldCode ?? (object)DBNull.Value);
                checkCommand.Parameters.AddWithValue("@PartNumber", part.PartNumber ?? (object)DBNull.Value);

                int exists = (int)await checkCommand.ExecuteScalarAsync();

                // 准备工艺数据，每道工序用"|"分隔工序名称、工序类型、预计工时、加工精度、状态
                var processes = new string[10];
                for (int i = 0; i < processSteps.Count && i < 10; i++)
                {
                    var step = processSteps[i];
                    processes[i] = $"{step.StepTitle ?? ""}|{step.ProcessType ?? ""}|{step.EstimatedTime}|{step.ProcessingPrecision ?? ""}|{step.Status ?? "待进行"}";
                }

                string sql;
                if (exists > 0)
                {
                    // 更新现有记录
                    sql = @"UPDATE MESProcessManagement 
                            SET Process1 = @Process1, Process2 = @Process2, Process3 = @Process3, 
                                Process4 = @Process4, Process5 = @Process5, Process6 = @Process6, 
                                Process7 = @Process7, Process8 = @Process8, Process9 = @Process9, 
                                Process10 = @Process10 
                            WHERE ProjectCode = @ProjectCode 
                            AND MoldCode = @MoldCode 
                            AND PartNumber = @PartNumber";
                }
                else
                {
                    // 插入新记录
                    sql = @"INSERT INTO MESProcessManagement 
                            (ProjectCode, MoldCode, PartNumber, 
                             Process1, Process2, Process3, Process4, Process5, 
                             Process6, Process7, Process8, Process9, Process10) 
                            VALUES (@ProjectCode, @MoldCode, @PartNumber, 
                                    @Process1, @Process2, @Process3, @Process4, @Process5, 
                                    @Process6, @Process7, @Process8, @Process9, @Process10)";
                }

                using var command = new SqlCommand(sql, connection);

                // 添加参数
                command.Parameters.AddWithValue("@ProjectCode", part.ProjectCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MoldCode", part.MoldCode ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@PartNumber", part.PartNumber ?? (object)DBNull.Value);

                for (int i = 0; i < 10; i++)
                {
                    command.Parameters.AddWithValue($"@Process{i + 1}", processes[i] ?? (object)DBNull.Value);
                }

                return await command.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in SaveProcessAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return 0;
            }
        }

        /// <summary>
        /// 获取零件的工序信息
        /// </summary>
        public async Task<List<ProcessManagement.ProcessStepData>> GetProcessStepsAsync(string projectCode, string moldCode, string partNumber)
        {
            if (string.IsNullOrEmpty(_connectionString) || string.IsNullOrEmpty(projectCode) || 
                string.IsNullOrEmpty(moldCode) || string.IsNullOrEmpty(partNumber))
            {
                return new List<ProcessManagement.ProcessStepData>();
            }

            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var sql = @"SELECT Process1, Process2, Process3, Process4, Process5, 
                                 Process6, Process7, Process8, Process9, Process10 
                          FROM MESProcessManagement 
                          WHERE ProjectCode = @ProjectCode 
                          AND MoldCode = @MoldCode 
                          AND PartNumber = @PartNumber";

                using var command = new SqlCommand(sql, connection);
                command.Parameters.AddWithValue("@ProjectCode", projectCode);
                command.Parameters.AddWithValue("@MoldCode", moldCode);
                command.Parameters.AddWithValue("@PartNumber", partNumber);

                var processSteps = new List<ProcessManagement.ProcessStepData>();

                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    for (int i = 0; i < 10; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            string processData = reader.GetString(i);
                            string[] parts = processData.Split('|');

                            if (parts.Length >= 5)
                            {
                                var step = new ProcessManagement.ProcessStepData
                                {
                                    Id = i + 1,
                                    StepCode = $"P{i + 1:D3}",
                                    StepTitle = parts[0],
                                    ProcessType = parts[1],
                                    EstimatedTime = decimal.TryParse(parts[2], out decimal time) ? time : 0,
                                    ProcessingPrecision = parts[3],
                                    Status = parts[4]
                                };
                                processSteps.Add(step);
                            }
                        }
                    }
                }

                return processSteps;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetProcessStepsAsync: {ex.Message}");
                Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");
                return new List<ProcessManagement.ProcessStepData>();
            }
        }
    }
}