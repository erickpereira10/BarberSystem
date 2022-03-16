using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using BarberSystem.Janelas;
using BarberSystem.Dados;
using System.Data.Entity.Migrations;
using BarberSystem.Controle;
using BarberSystem.Utils;

namespace BarberSystem.Janelas {
    /// <summary>
    /// Lógica interna para ContasPagar.xaml
    /// </summary>
    public partial class ContasPagar {

        #region Atributos

        CONTAS_PAGAR cp = new CONTAS_PAGAR();
        BancoDados conexao = new BancoDados();
        private List<CONTAS_PAGAR> listaPagar = new List<CONTAS_PAGAR>();

        #endregion

        #region Construtor

        public ContasPagar() {
            InitializeComponent();
            dgPagar.RowBackground = null;
            carregaGrid();
            carregaPesquisa();
        }

        #endregion

        #region Metodos

        // limpar os campos(textBox)
        public void limpaCampos() {
            txtCodigo.Clear();
            txtDescricao.Clear();
            cbPesquisar.Text = string.Empty;
            txtUnitario.Clear();
            lblTotal.Content = "0";
            dpPagto.Text = "";
            dpVencto.Text = "";
            btnGravar.IsEnabled = true;
            txtFornecedor.Clear();
        }

        //carregar o dataGrid
        public void carregaGrid() {
            listaPagar = conexao.CONTAS_PAGAR.ToList();
            dgPagar.ItemsSource = null;
            dgPagar.ItemsSource = listaPagar.OrderBy(user => user.codigo);
        }

        // verificar campos vazios
        public void verificaVazios() {
            if (txtDescricao.Text == "") {
                MessageBox.Show("O campo descrição não pode ser vazio!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                limpaCampos();
                return;
            }
            else {
                cp.descricao = txtDescricao.Text;
            }
            if (dpPagto.SelectedDate.ToString() == "") {
                cp.data_pagto = null;
            }
            else {
                cp.data_pagto = DateTime.Parse(dpPagto.SelectedDate.ToString());
            }
            if (dpVencto.SelectedDate.ToString() == "") {
                cp.data_vencto = null;
            }
            else {
                cp.data_vencto = DateTime.Parse(dpVencto.SelectedDate.ToString());
            }
            if (txtUnitario.Text == "") {
                cp.vl_unitario = null;
            }
            else {
                cp.vl_unitario = double.Parse(txtUnitario.Text);
            }
        }

        // calcular valor total e mostrar na Label
        public void calculaValorTotal() {
            cp.vl_total = 0.0;
            foreach (CONTAS_PAGAR item in listaPagar) {
                cp.vl_total += item.vl_unitario;
            }
            lblTotal.Content = cp.vl_total.ToString();
        }

        // carregar comboBox pesquisa
        private void carregaPesquisa() {
            //DateTime mes = DateTime.Now;
            //var sql = from cp in conexao.CONTAS_PAGAR
            //          where cp.codigo > 0 && cp.data_vencto.Value.Month == mes.Month
            //          select cp.codigo + "    - " + cp.descricao;
            var sql = conexao.CONTAS_PAGAR.ToList();
            cbPesquisar.ItemsSource = null;
            cbPesquisar.ItemsSource = sql.OrderBy(x => x.codigo);
            cbPesquisar.DisplayMemberPath = "codigo";
        }

        private void atualizaPesquisa() {
            var sql = from cp in conexao.CONTAS_PAGAR
                      where cp.data_vencto.Value.Month == dpCurrent.SelectedDate.Value.Month
                      select cp.codigo + "    - " + cp.descricao;
            cbPesquisar.ItemsSource = null;
            cbPesquisar.ItemsSource = sql.ToList();
        }

        // calcular valor total por data selecionada
        private void calcularPorData() {
            listaPagar = conexao.CONTAS_PAGAR.Where(x => x.data_vencto.Value.Month == dpCurrent.SelectedDate.Value.Month).ToList();
            cp.vl_total = 0.0;
            foreach (CONTAS_PAGAR item in listaPagar) {
                cp.vl_total += item.vl_unitario;
            }
            lblTotal.Content = cp.vl_total.ToString();
        }

        #endregion

        #region Eventos

        // botao novo
        private void btnNovo_Click(object sender, RoutedEventArgs e) {
            txtDescricao.Focus();
            limpaCampos();
        }

        // botao gravar
        private void btnGravar_Click(object sender, RoutedEventArgs e) {
            try {
                calculaValorTotal();
                verificaVazios();
                if (txtDescricao.Text == "") {
                    return;
                }
                cp.vl_total += cp.vl_unitario;

                conexao.CONTAS_PAGAR.Add(cp);
                conexao.SaveChanges();


                txtCodigo.Text = cp.codigo.ToString();
                carregaGrid();
                carregaPesquisa();

                MessageBox.Show("Dados salvo com sucesso!!!", "Salvando...", MessageBoxButton.OK, MessageBoxImage.Information);
                limpaCampos();
            }
            catch (Exception a) {
                MessageBox.Show("Erro ao gravar!" + "\n" + a.StackTrace, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // botao limpar
        private void btnLimpar_Click(object sender, RoutedEventArgs e) {
            limpaCampos();
        }

        // botao voltar
        private void btnVoltar_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        // botao pesquisar
        private void btnPesquisar_Click(object sender, RoutedEventArgs e) {
            btnGravar.IsEnabled = false;
            try {
                if (cbPesquisar.Text != null) {
                    if (cbPesquisar.Text != null && txtFornecedor.Text.Equals(string.Empty)) {
                        CONTAS_PAGAR rec = conexao.CONTAS_PAGAR.Find(int.Parse(cbPesquisar.Text));
                        List<CONTAS_PAGAR> listaPag = new List<CONTAS_PAGAR>();
                        listaPag.Add(rec);
                        dgPagar.ItemsSource = null;
                        dgPagar.ItemsSource = listaPag;
                    }
                }
                else {
                    MessageBox.Show("Registro não encontrado!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                }
            }
            catch (Exception a) {
                MessageBox.Show("Campo vazio ou código invalido!" + "\n" + a.StackTrace, "Erro", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);
                limpaCampos();
            }
        }

        // botao excluir
        private void btnExcluir_Click(object sender, RoutedEventArgs e) {
            try {
                MessageBoxResult resultado = MessageBox.Show("Tem certeza que deseja excluir o registro?", "Excluir",
                                                             MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultado == MessageBoxResult.Yes) {
                    cp = conexao.CONTAS_PAGAR.Remove(cp);
                    cp.descricao = null;
                    cp.data_pagto = null;
                    cp.data_vencto = null;
                    cp.vl_unitario = null;
                    cp.vl_total = null;
                    limpaCampos();
                    conexao.SaveChanges();
                    int? codigo = conexao.CONTAS_PAGAR.Max(a => (int?)a.codigo);
                    Util.redefinirPK_AutoIncremento("CONTAS_PAGAR", codigo);
                    MessageBox.Show("Registro excluido com sucesso!", "Excluir", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    carregaGrid();
                    limpaCampos();
                    carregaPesquisa();
                }
                else {
                    limpaCampos();
                    return;
                }
                btnGravar.IsEnabled = true;
            }
            catch (Exception) {
                MessageBox.Show("Erro imprevisto ou campos vazios", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // botao calcular valor total
        private void btnCalcularValorTotal_Click(object sender, RoutedEventArgs e) {
            calculaValorTotal();
        }

        // exportar para o excel
        private void btnExportar_Click(object sender, RoutedEventArgs e) {
            Utils.Util.exportarExcel(dgPagar);
        }

        // mostrar a calculadora do windows
        private void btnCalcWindows_Click(object sender, RoutedEventArgs e) {
            try {
                System.Diagnostics.Process.Start("calc.exe");
            }
            catch (Exception) {
                MessageBox.Show("Sistema não encontrou a calculadora!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // botao alterar
        private void btnAlterar_Click(object sender, RoutedEventArgs e) {
            try {
                if (txtCodigo.Text != "") {
                    verificaVazios();
                    if (txtDescricao.Text == "") {
                        return;
                    }
                    cp.vl_total = cp.vl_unitario;
                    double? temp = 0.0;
                    foreach (CONTAS_PAGAR item in listaPagar) {
                        item.vl_total = temp;
                        item.vl_total += item.vl_unitario;
                        temp = item.vl_total;
                    }
                    conexao.CONTAS_PAGAR.AddOrUpdate(cp);
                    conexao.SaveChanges();
                    MessageBox.Show("Dados alterados com sucesso!", "Alterar", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                    carregaGrid();
                    carregaPesquisa();
                }
                else {
                    MessageBox.Show("Insira um código ou pesquise para alterar", "Alterar", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                    return;
                }
                btnGravar.IsEnabled = true;
            }
            catch (Exception) {
                MessageBox.Show("Erro ao tentar alterar!", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // pesquisar por data
        private void dpCurrent_SelectedDateChanged(object sender, SelectionChangedEventArgs e) {
            if (dpCurrent.SelectedDate != null) {
                atualizaPesquisa();
                calcularPorData();
            }
            if (dgPagar.SelectedItems.Count > 0) {
                cp = dgPagar.SelectedItem as CONTAS_PAGAR;
                txtCodigo.Text = cp.codigo.ToString();
                txtDescricao.Text = cp.descricao;
                txtFornecedor.Text = cp.fornecedor.nome;
                dpVencto.Text = cp.data_vencto.ToString();
                dpPagto.Text = cp.data_pagto.ToString();
                txtUnitario.Text = cp.vl_unitario.ToString();
                lblTotal.Content = cp.vl_total.ToString();
            }
        }

        private void BtnReceber_Click(object sender, RoutedEventArgs e) {
            try {

                if (txtCodigo.Text != "") {
                    if (dgPagar.SelectedItems.Count > 0) {
                        if (dpPagto.DisplayDate != null) {
                            CONTAS_PAGAR pag = conexao.CONTAS_PAGAR.Find(int.Parse(txtCodigo.Text));
                            pag.data_pagto = dpPagto.DisplayDate;
                            conexao.CONTAS_PAGAR.Attach(pag);
                            conexao.Entry(pag).Property(x => x.data_pagto).IsModified = true;
                            conexao.SaveChanges();
                            MessageBox.Show("Conta paga com sucesso!", "Sucesso",
                                            MessageBoxButton.OK, MessageBoxImage.Information);
                            limpaCampos();
                            carregaGrid();
                        }
                        else {
                            MessageBox.Show("Insira a data de pagamento!", "Atenção",
                                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
                    }
                    else {
                        MessageBox.Show("Escolha um registro na grid!", "Atenção",
                                             MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                else {
                    MessageBox.Show("Código da conta vazio!", "Atenção",
                                              MessageBoxButton.OK, MessageBoxImage.Warning);
                }

            }
            catch (Exception) {

                MessageBox.Show("Erro ao receber conta!", "Erro",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // mostrar dados
        private void DgPagar_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (dgPagar.SelectedItems.Count > 0) {
                cp = dgPagar.SelectedItem as CONTAS_PAGAR;
                txtCodigo.Text = cp.codigo.ToString();
                txtDescricao.Text = cp.descricao;
                txtFornecedor.Text = cp.fornecedor.nome;
                dpVencto.Text = cp.data_vencto.ToString();
                dpPagto.Text = cp.data_pagto.ToString();
                txtUnitario.Text = cp.vl_unitario.ToString();
                lblTotal.Content = cp.vl_total.ToString();
            }
        }

        #endregion

    }
}
