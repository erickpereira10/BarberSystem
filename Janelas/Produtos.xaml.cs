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
using BarberSystem.Utils;
using BarberSystem.Dados;
using System.Data.Entity.Migrations;
using BarberSystem.Controle;

namespace BarberSystem.Janelas {
    /// <summary>
    /// Lógica interna para Produtos.xaml
    /// </summary>
    public partial class Produtos {

        #region Atributos

        PRODUTOS produto = new PRODUTOS();
        BancoDados conexao = new BancoDados();
        private List<PRODUTOS> listaProdutos = new List<PRODUTOS>();

        #endregion

        #region Construtor

        public Produtos() {
            InitializeComponent();
            dgProdutos.RowBackground = null;
            carregaGrid();
            carregaComboFornecedor();
            BloquearCampos();
        }

        #endregion

        #region Metodos

        // metodo para limpar os campos
        private void limpaCampos() {
            txtCodigo.Clear();
            txtPesquisar.Clear();
            txtDescricao.Clear();
            txtQuantidade.Clear();
            txtUnitario.Clear();
            cbCodFornecedor.Text = "";
            txtCodigoFornecedor.Clear();
            btnGravar.IsEnabled = true;
            dpVencimento.Text = string.Empty;
        }

        // bloquear campos
        private void BloquearCampos(){
            txtCodigo.IsReadOnly = true;
            txtCodigoFornecedor.IsReadOnly = true;
        }

        // metodo para carregar o dataGrid
        public void carregaGrid() {
            listaProdutos = conexao.PRODUTOS.ToList();
            dgProdutos.ItemsSource = null;
            dgProdutos.ItemsSource = listaProdutos.OrderBy(user => user.codigo);
        }

        // carregar comboBox fornecedor
        private void carregaComboFornecedor() {
            var sql = from f in conexao.FORNECEDORES
                      where f.codigo > 0
                      select f.nome;

            cbCodFornecedor.ItemsSource = null;
            cbCodFornecedor.ItemsSource = sql.ToList();
        }

        // gerar contas a pagar
        private CONTAS_PAGAR GerarContasPagar() {
            try {

                FORNECEDORES forn = conexao.FORNECEDORES.Find(int.Parse(txtCodigoFornecedor.Text));
                CONTAS_PAGAR c = new CONTAS_PAGAR();
                c.descricao = txtDescricao.Text;
                c.fornecedor = forn;
                c.data_vencto = dpVencimento.DisplayDate;
                c.vl_total = (double.Parse(txtUnitario.Text) * int.Parse(txtQuantidade.Text));
                c.vl_unitario = double.Parse(txtUnitario.Text);
                return c;

            }
            catch (Exception) {
                throw;
            }
        }

        // gerar estoque
        private ESTOQUE GerarEstoque() {
            try {
                ESTOQUE est = new ESTOQUE();
                est.produto = txtDescricao.Text;
                est.quantidade = int.Parse(txtQuantidade.Text);
                est.vl_produto = double.Parse(txtUnitario.Text);
                est.vl_total = (double.Parse(txtUnitario.Text) * int.Parse(txtQuantidade.Text));
                est.PRODUTOS = produto;
                return est;

            }
            catch (Exception) {
                throw;
            }
        }

        #endregion

        #region Eventos

        // botao novo
        private void btnNovo_Click(object sender, RoutedEventArgs e) {
            txtDescricao.Focus();
            limpaCampos();
        }

        // botao alterar
        private void btnAlterar_Click(object sender, RoutedEventArgs e) {
            try {
                if (txtCodigo.Text != "") {
                    produto.codigo = int.Parse(txtCodigo.Text);
                    produto.vl_unitario = double.Parse(txtUnitario.Text);
                    produto.codfornecedor = int.Parse(Util.VerificarCamposVazios(txtCodigoFornecedor.Text));
                    produto.nome_fornecedor = Util.VerificarCamposVazios(cbCodFornecedor.Text);
                    FORNECEDORES f = conexao.FORNECEDORES.Find(int.Parse(txtCodigoFornecedor.Text));
                    produto.FORNECEDORES = f;
                    produto.quantidade = int.Parse(txtQuantidade.Text);

                    if (Util.vazio == true) {
                        return;
                    }

                    conexao.PRODUTOS.AddOrUpdate(produto);
                    conexao.SaveChanges();
                    MessageBox.Show("Dados alterados com sucesso!", "Alterar", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                    carregaGrid();
                }
                else {
                    MessageBox.Show("Insira um código ou pesquise para alterar", "Alterar", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                    return;
                }
            }
            catch (Exception a) {
                MessageBox.Show("Alguns campos não podem ficar vazios" + "\n" + a.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                limpaCampos();
            }
            btnGravar.IsEnabled = true;
        }

        // botao pesquisar
        private void btnPesquisar_Click(object sender, RoutedEventArgs e) {
            btnGravar.IsEnabled = false;
            try {
                if (txtPesquisar.Text != string.Empty) {
                    var sql = conexao.PRODUTOS.Where(x => x.descricao.StartsWith(txtPesquisar.Text));
                    dgProdutos.ItemsSource = null;
                    dgProdutos.ItemsSource = sql.ToList();
                }
                else {
                    MessageBox.Show("Produto não encontrado!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                }
            }
            catch (Exception a) {
                MessageBox.Show("Campo vazio ou código invalido!" + "\n" + a.Message, "Erro", MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
                limpaCampos();
            }
        }

        // botao excluir
        private void btnExcluir_Click(object sender, RoutedEventArgs e) {
            try {
                MessageBoxResult resultado = MessageBox.Show("Tem certeza que deseja excluir o registro?", "Excluir", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultado == MessageBoxResult.Yes) {
                    produto = conexao.PRODUTOS.Remove(produto);
                    limpaCampos();
                    produto.descricao = null;
                    produto.vl_unitario = null;
                    produto.codfornecedor = null;
                    produto.nome_fornecedor = null;
                    conexao.SaveChanges();
                    int? codigo = conexao.PRODUTOS.Max(a => (int?)a.codigo);
                    Util.redefinirPK_AutoIncremento("PRODUTOS", codigo);
                    MessageBox.Show("Registro excluido com sucesso!", "Excluir", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    carregaGrid();
                    limpaCampos();
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

        // botao gravar
        private void btnGravar_Click(object sender, RoutedEventArgs e) {
            try {
                produto.descricao = Util.VerificarCamposVazios(txtDescricao.Text);
                produto.vl_unitario = double.Parse(txtUnitario.Text);
                produto.codfornecedor = int.Parse(Util.VerificarCamposVazios(txtCodigoFornecedor.Text));
                produto.nome_fornecedor = Util.VerificarCamposVazios(cbCodFornecedor.Text);
                FORNECEDORES f = conexao.FORNECEDORES.Find(int.Parse(txtCodigoFornecedor.Text));
                produto.FORNECEDORES = f;
                produto.quantidade = int.Parse(txtQuantidade.Text);


                if (Util.vazio == true) {
                    return;
                }

                conexao.PRODUTOS.Add(produto);
                conexao.CONTAS_PAGAR.Add(GerarContasPagar());
                conexao.ESTOQUE.Add(GerarEstoque());
                conexao.SaveChanges();

                txtCodigo.Text = produto.codigo.ToString();
                carregaGrid();

                MessageBox.Show("Dados salvo com sucesso!!!", "Salvando...", MessageBoxButton.OK, MessageBoxImage.Information);
                limpaCampos();
            }
            catch (Exception a) {
                MessageBox.Show(a.Message, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
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

        // botao excel
        private void btnExportar_Click(object sender, RoutedEventArgs e) {
            Util.exportarExcel(dgProdutos);
        }

        // carregar codigo do fornecedor em tela
        private void cbCodFornecedor_DropDownClosed(object sender, EventArgs e) {
            try {
                if(cbCodFornecedor.Text != ""){
                    FORNECEDORES f = conexao.FORNECEDORES.First(x => x.nome.Equals(cbCodFornecedor.Text));
                    txtCodigoFornecedor.Text = f.codigo.ToString();
                }
            }
            catch (Exception) {
               
            }
        }

        // mostrar dados
        private void DgProdutos_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                if (dgProdutos.SelectedItems.Count > 0) {
                    produto = dgProdutos.SelectedItem as PRODUTOS;
                    if (produto != null) {
                        txtDescricao.Text = produto.descricao;
                        txtCodigo.Text = produto.codigo.ToString();
                        txtUnitario.Text = produto.vl_unitario.ToString();
                        cbCodFornecedor.Text = produto.FORNECEDORES.nome;
                        txtCodigoFornecedor.Text = produto.FORNECEDORES.codigo.ToString();
                        txtQuantidade.Text = produto.quantidade.ToString();
                        dpVencimento.Text = produto.vencimento.ToString();
                    }
                }
            }
            catch (Exception ex) {
                MessageBox.Show("Selecione um registro!" + "\n" + ex.Message, "Aviso",
                               MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        #endregion

    }
}
