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
    /// Lógica interna para Estoque.xaml
    /// </summary>
    ///

    public partial class Estoque {

        #region Atributos

        ESTOQUE estoque = new ESTOQUE();
        BancoDados conexao = new BancoDados();
        private List<ESTOQUE> listaEstoque = new List<ESTOQUE>();

        #endregion

        #region Construtor

        public Estoque() {
            InitializeComponent();
            dgEstoque.RowBackground = null;
            carregaGrid();
            BloquearCampos();
        }

        #endregion

        #region Metodos

        // limpar os campos
        private void limpaCampos(){
            txtPesquisar.Text = string.Empty;
            txtCodigo.Clear();
            txtProduto.Clear();
            txtUnitario.Clear();
            txtTotal.Clear();
            txtQuantidade.Clear();
        }

        // carregar o dataGrid
        private void carregaGrid(){
            listaEstoque = conexao.ESTOQUE.ToList();
            dgEstoque.ItemsSource = null;
            dgEstoque.ItemsSource = listaEstoque.OrderBy(user => user.codigo);
        }

        // bloquear campos
        private void BloquearCampos() {
            txtCodigo.IsReadOnly = true;
            txtProduto.IsReadOnly = true;
            txtUnitario.IsReadOnly = true;
            txtTotal.IsReadOnly = true;
            txtQuantidade.IsReadOnly = true;
        }

        #endregion

        #region Eventos

        // botao pesquisar
        private void btnPesquisar_Click(object sender, RoutedEventArgs e) {
            try {
                if (txtPesquisar.Text != string.Empty) {
                    var sql = conexao.PRODUTOS.Where(x => x.descricao.StartsWith(txtPesquisar.Text));
                    dgEstoque.ItemsSource = null;
                    dgEstoque.ItemsSource = sql.ToList();
                }
                else {
                    MessageBox.Show("Produto no estoque não encontrado!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                }
            }
            catch (Exception a) {
                MessageBox.Show("Campo vazio ou código invalido!" + "\n" + a.StackTrace, "Erro", MessageBoxButton.OK,
                                 MessageBoxImage.Exclamation);
                limpaCampos();
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
            Util.exportarExcel(dgEstoque);
        }

        // mostrar dados
        private void DgEstoque_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try{

              if(dgEstoque.SelectedItems.Count > 0){
                    estoque = dgEstoque.SelectedItem as ESTOQUE;
                    if(estoque != null){
                        txtProduto.Text = estoque.produto;
                        txtCodigo.Text = estoque.codigo.ToString();
                        txtUnitario.Text = estoque.vl_produto.ToString();
                        txtTotal.Text = estoque.vl_total.ToString();
                        txtQuantidade.Text = estoque.quantidade.ToString();
                    }
                }

            }catch(Exception ex){
                MessageBox.Show("Erro ao selecionar produto" + ex.Message, "Erro",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
