using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using System.Data;

namespace BlazorApp1.Services
{
    public class EquipmentManagementService
    {
        private readonly string _connectionString;

        public EquipmentManagementService(string connectionString)
        {
            _connectionString = connectionString ?? string.Empty;
        }

        // 获取所有设备信息
        public async Task<List<EquipmentData>> GetAllEquipmentsAsync()
        {
            var equipments = new List<EquipmentData>();

            if (string.IsNullOrEmpty(_connectionString))
            {
                return equipments;
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                string sql = "SELECT EquipmentSerialNumber, EquipmentName, EquipmentCategory, EquipmentType, EquipmentModel, EquipmentLocation FROM MESEquipmentManagement";
                using var cmd = new SqlCommand(sql, conn);
                using var reader = await cmd.ExecuteReaderAsync();
                
                while (await reader.ReadAsync())
                {
                    equipments.Add(new EquipmentData
                    {
                        EquipmentSerialNumber = reader["EquipmentSerialNumber"]?.ToString() ?? string.Empty,
                        EquipmentName = reader["EquipmentName"]?.ToString() ?? string.Empty,
                        EquipmentCategory = reader["EquipmentCategory"]?.ToString() ?? string.Empty,
                        EquipmentType = reader["EquipmentType"]?.ToString() ?? string.Empty,
                        EquipmentModel = reader["EquipmentModel"]?.ToString() ?? string.Empty,
                        EquipmentLocation = reader["EquipmentLocation"]?.ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return equipments;
        }

        // 添加新设备
        public async Task<bool> AddEquipmentAsync(EquipmentData equipment)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return false;
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                string sql = @"INSERT INTO MESEquipmentManagement (EquipmentSerialNumber, EquipmentName, EquipmentCategory, EquipmentType, EquipmentModel, EquipmentLocation) 
                                VALUES (@EquipmentSerialNumber, @EquipmentName, @EquipmentCategory, @EquipmentType, @EquipmentModel, @EquipmentLocation)";
                using var cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@EquipmentSerialNumber", equipment.EquipmentSerialNumber);
                cmd.Parameters.AddWithValue("@EquipmentName", equipment.EquipmentName);
                cmd.Parameters.AddWithValue("@EquipmentCategory", equipment.EquipmentCategory);
                cmd.Parameters.AddWithValue("@EquipmentType", equipment.EquipmentType);
                cmd.Parameters.AddWithValue("@EquipmentModel", equipment.EquipmentModel);
                cmd.Parameters.AddWithValue("@EquipmentLocation", equipment.EquipmentLocation);

                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
        
        // 获取当前最大的设备序号
        public async Task<int> GetMaxEquipmentSerialNumberAsync()
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return 0;
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                // 查询当前最大的设备序号，如果没有则返回0
                string sql = "SELECT ISNULL(MAX(CAST(EquipmentSerialNumber AS INT)), 0) FROM MESEquipmentManagement";
                using var cmd = new SqlCommand(sql, conn);
                
                var result = await cmd.ExecuteScalarAsync();
                return result is DBNull ? 0 : Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
        
        // 删除设备
        public async Task<bool> DeleteEquipmentAsync(EquipmentData equipment)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return false;
            }

            try
            {
                using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync();
                string sql = "DELETE FROM MESEquipmentManagement WHERE EquipmentSerialNumber = @EquipmentSerialNumber";
                using var cmd = new SqlCommand(sql, conn);
                
                cmd.Parameters.AddWithValue("@EquipmentSerialNumber", equipment.EquipmentSerialNumber);
                
                int result = await cmd.ExecuteNonQueryAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // 设备数据模型
        public class EquipmentData
        {
            public string EquipmentSerialNumber { get; set; } = string.Empty;
            public string EquipmentName { get; set; } = string.Empty;
            public string EquipmentCategory { get; set; } = string.Empty;
            public string EquipmentType { get; set; } = string.Empty;
            public string EquipmentModel { get; set; } = string.Empty;
            public string EquipmentLocation { get; set; } = string.Empty;
            public bool IsSelected { get; set; } = false;
        }
    }
}