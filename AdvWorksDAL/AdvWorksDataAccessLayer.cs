﻿using AdvWorksDTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvWorksDAL
{
    public class AdvWorksDataAccessLayer
    {
        SqlConnection conObj;
        SqlCommand cmdObj;
        public AdvWorksDataAccessLayer()
        {
            conObj = new SqlConnection(ConfigurationManager.ConnectionStrings["AdvWorksConnectionStr"].ConnectionString);
        }

        public int ConnectionToDB()
        {
            try
            {
                string sqlConStr = ConfigurationManager.ConnectionStrings["AdvWorksConnectionStr"].ConnectionString;
                SqlConnection conObj = new SqlConnection(sqlConStr);
                conObj.Open();
                if (conObj.State.ToString() == "Open")
                    return 1;
                else
                    return -1;
            }
            catch (Exception ex)
            {
                return -99;
            }
        }
        public List<DeptDetailsDTO> FetchAllDept()
        {
            try
            {
                cmdObj = new SqlCommand(@"SELECT Name,GroupName FROM HumanResources.Department",conObj);
                conObj.Open();
                SqlDataReader drDept = cmdObj.ExecuteReader();
                //while (drDept.Read())
                //{
                //    Console.WriteLine(drDept["Name"]+" | "+drDept["GroupName"]);
                //}
                List<DeptDetailsDTO> lstDept = new List<DeptDetailsDTO>();
                while (drDept.Read())
                {
                    DeptDetailsDTO deptFromReader = new DeptDetailsDTO();
                    deptFromReader.DeptName = drDept["Name"].ToString();
                    deptFromReader.DeptGroupName = drDept["GroupName"].ToString();
                    lstDept.Add(deptFromReader);
                    //lstDept.Add(new DeptDetailsDTO()
                    //{
                    //    DeptName = drDept["Name"].ToString(),
                    //    DeptGroupName = drDept["GroupName"].ToString()
                    //});
                }
                return lstDept;
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                conObj.Close();
            }
        }

        public int AddNewDepartment(DeptDetailsDTO newDeptObj)
        {
            try
            {
                cmdObj = new SqlCommand();
                cmdObj.CommandText = "uspAddNewDept";
                cmdObj.CommandType = System.Data.CommandType.StoredProcedure;
                cmdObj.Connection = conObj;
                cmdObj.Parameters.AddWithValue("@deptName", newDeptObj.DeptName);
                cmdObj.Parameters.AddWithValue("@deptGroupName", newDeptObj.DeptGroupName);
                cmdObj.Parameters.AddWithValue("@deptDate", System.DateTime.Now);

                SqlParameter prcReturnValue = new SqlParameter();
                prcReturnValue.Direction = ParameterDirection.ReturnValue;
                prcReturnValue.SqlDbType = SqlDbType.Int;
                cmdObj.Parameters.Add(prcReturnValue);

                conObj.Open();
                cmdObj.ExecuteNonQuery();
                return Convert.ToInt32(prcReturnValue);
            }
            catch (Exception)
            {
                return -99;
            }
            finally { conObj.Close(); }
        }

        public List<ProductsDTO> ProductList()
        {
            cmdObj = new SqlCommand(@"SELECT ProductID,Name,ProductNumber,ListPrice FROM Production.Product", conObj);
            SqlDataAdapter daProducts = new SqlDataAdapter(cmdObj);
            DataTable dtProductsFromDB = new DataTable();
            daProducts.Fill(dtProductsFromDB);
            List<ProductsDTO> lstProducts = new List<ProductsDTO>();
            foreach (DataRow prod in dtProductsFromDB.Rows)
            {
                ProductsDTO newObj = new ProductsDTO();
                newObj.ProdId = Convert.ToInt32(prod["ProductId"]);
                newObj.ProdName = Convert.ToString(prod["Name"]);
                newObj.ProdNum = Convert.ToString(prod["ProductNumber"]);
                newObj.ProdListPrice = Convert.ToInt32(prod["ListPrice"]);
                lstProducts.Add(newObj);
            }
            return lstProducts;
        }
        public List<ProductsDTO> ProdSearchList(string input)
        {
            try
            {
                cmdObj = new SqlCommand($@"SELECT ProductID,Name,ProductNumber,ListPrice FROM Production.Product WHERE Name LIKE '%{input}%' ORDER BY Name", conObj);
                SqlDataAdapter daProducts = new SqlDataAdapter(cmdObj);
                DataTable dtProductsFromDB = new DataTable();
                daProducts.Fill(dtProductsFromDB);
                List<ProductsDTO> lstProducts = new List<ProductsDTO>();
                foreach (DataRow prod in dtProductsFromDB.Rows)
                {
                    ProductsDTO newObj = new ProductsDTO();
                    newObj.ProdId = Convert.ToInt32(prod["ProductId"]);
                    newObj.ProdName = Convert.ToString(prod["Name"]);
                    newObj.ProdNum = Convert.ToString(prod["ProductNumber"]);
                    newObj.ProdListPrice = Convert.ToInt32(prod["ListPrice"]);
                    lstProducts.Add(newObj);
                }
                return lstProducts;
            }
            catch (Exception ex)
            {
                Exception exp = new Exception("Invalid user input");
                throw exp;
            }
        }
    }
}
