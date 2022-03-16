﻿using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;
using BarberSystem.Dados;
using BarberSystem.Controle;
using System.Data.Entity;
using BespokeFusion;
using System.Windows.Media;

namespace BarberSystem.Utils {
    public class Util {

        #region Validar Campos Vazios

        // validar campos vazios
        public static bool vazio;
        public static string VerificarCamposVazios(string txt) {
            vazio = false;
            if (txt.Equals(string.Empty)) {
                MessageBox.Show("Campo não pode ficar vazio!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                vazio = true;
            }
            return txt;
        }

        #endregion

        #region Encriptar e Descriptografar

        // crypt senha
        public static string encrypt(string valor) {
            MD5 hash = MD5.Create();
            byte[] valorCriptografado = hash.ComputeHash(Encoding.Default.GetBytes(valor));
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < valorCriptografado.Length; i++) {
                strBuilder.Append(valorCriptografado[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        // descrypt senha
        public static bool descrypt(string valor, string valorCriptografado) {
            string novoValorCriptografado = encrypt(valor);
            StringComparer compararSenha = StringComparer.OrdinalIgnoreCase;
            if (compararSenha.Compare(novoValorCriptografado, valorCriptografado) == 0) {
                return true;
            }
            else {
                return false;
            }
        }

        #endregion

        #region Exportar para Excel

        public static void exportarExcel(DataGrid dg) {
            Excel.Application excel = new Excel.Application();
            excel.Visible = true;
            Workbook workbook = excel.Workbooks.Add(System.Reflection.Missing.Value);
            Worksheet sheet1 = (Worksheet)workbook.Sheets[1];

            for (int j = 0; j < dg.Columns.Count; j++) {
                Range myRange = (Range)sheet1.Cells[1, j + 1];
                sheet1.Cells[1, j + 1].Font.Bold = true;
                sheet1.Columns[j + 1].ColumnWidth = 15;
                myRange.Value2 = dg.Columns[j].Header;
            }
            for (int i = 0; i < dg.Columns.Count; i++) {
                for (int j = 0; j < dg.Items.Count; j++) {
                    TextBlock b = dg.Columns[i].GetCellContent(dg.Items[j]) as TextBlock;
                    Microsoft.Office.Interop.Excel.Range myRange = (Microsoft.Office.Interop.Excel.Range)sheet1.Cells[j + 2, i + 1];
                    myRange.Value2 = b.Text;
                }
            }
        }

        #endregion

        #region PK Auto incremento ao excluir

        // redefinir valor da chave primaria auto incremento
        public static void redefinirPK_AutoIncremento(string tabela, int? valor) {
            try {
                BancoDados conexao = new BancoDados();
                string sql = string.Format(@"DBCC CHECKIDENT ({0}, RESEED, {1})", tabela, valor);
                conexao.Database.ExecuteSqlCommand(sql);
            }
            catch (Exception) {
                throw;
            }
        }

        #endregion
    }
}
