﻿using System;
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
using BarberSystem.Utils;
using BarberSystem.Dados;
using System.Data.Entity.Migrations;
using BarberSystem.Controle;

namespace BarberSystem.Janelas {
    /// <summary>
    /// Lógica interna para Barbeiros.xaml
    /// </summary>
    public partial class Barbeiros {

        private BARBEIROS barbeiro = new BARBEIROS();
        private List<BARBEIROS> listaBarber;
        private BancoDados conexao = new BancoDados();

        public Barbeiros() {
            InitializeComponent();
            dgBarbeiro.RowBackground = null;
            carregaGrid();
            carregaCombopesquisa();
        }

        public void limpaCampos(){
            txtCodigo.Clear();
            txtNome.Clear();
            txtEndereco.Clear();
            txtNumero.Clear();
            txtCidade.Clear();
            cbEstado.Text = "";
            MtxtCep.Clear();
            cbSexo.Text = "";
            MtxtCelular.Clear();
            cbPesquisar.Text = string.Empty;
            txtBairro.Clear();
            btnGravar.IsEnabled = true;
        }

        // botao novo
        private void btnNovo_Click(object sender, RoutedEventArgs e) {
            txtNome.Focus();
            limpaCampos();
        }

        // botao voltar
        private void btnVoltar_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        // botao gravar
        private void btnGravar_Click(object sender, RoutedEventArgs e) {
            try {
                barbeiro.nome = Util.VerificarCamposVazios(txtNome.Text);
                barbeiro.endereco = Util.VerificarCamposVazios(txtEndereco.Text);
                barbeiro.numero = int.Parse(txtNumero.Text);
                barbeiro.bairro = Util.VerificarCamposVazios(txtBairro.Text);
                barbeiro.cidade = Util.VerificarCamposVazios(txtCidade.Text);
                barbeiro.estado = Util.VerificarCamposVazios(cbEstado.Text);
                barbeiro.cep = MtxtCep.Text;
                barbeiro.sexo = Util.VerificarCamposVazios(cbSexo.Text);
                barbeiro.celular = MtxtCelular.Text;

                if (Util.vazio == true) {
                    return;
                }
 
                conexao.BARBEIROS.Add(barbeiro);
                conexao.SaveChanges();

                txtCodigo.Text = barbeiro.codigo.ToString();
                MessageBox.Show("Dados salvo com sucesso!!!", "Salvando...", MessageBoxButton.OK, MessageBoxImage.Information);
                limpaCampos();
                carregaGrid();
                carregaCombopesquisa();

            }
            catch(Exception a){
                MessageBox.Show("Erro ao gravar" + "\n" + a.StackTrace, "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // carregar a grid
        public void carregaGrid(){
            listaBarber = conexao.BARBEIROS.ToList();
            dgBarbeiro.ItemsSource = null;
            dgBarbeiro.ItemsSource = listaBarber.OrderBy(user => user.nome);
        }

        // botao limpar
        private void btnLimpar_Click(object sender, RoutedEventArgs e) {
            limpaCampos();
        }

        // botao excluir
        private void btnExcluir_Click(object sender, RoutedEventArgs e) {
            try {
                MessageBoxResult resultado = MessageBox.Show("Tem certeza que deseja excluir o registro?", "Excluir", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (resultado == MessageBoxResult.Yes) {
                    barbeiro = conexao.BARBEIROS.Remove(barbeiro);
                    barbeiro.nome = null;
                    barbeiro.endereco = null;
                    barbeiro.numero = null;
                    barbeiro.bairro = null;
                    barbeiro.cidade = null;
                    barbeiro.estado = null;
                    barbeiro.cep = null;
                    barbeiro.sexo = null;
                    barbeiro.celular = null;
                    conexao.SaveChanges();
                    int? codigo = conexao.BARBEIROS.Max(a => (int?)a.codigo);
                    Util.redefinirPK_AutoIncremento("BARBEIROS", codigo);
                    MessageBox.Show("Registro excluido com sucesso!", "Excluir", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    carregaGrid();
                    limpaCampos();
                    carregaCombopesquisa();
                }
                else {
                    limpaCampos();
                    return;
                }
            } catch(Exception){
                MessageBox.Show("Erro imprevisto ou campos vazios", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // botao pesquisar
        private void btnPesquisar_Click(object sender, RoutedEventArgs e) {
            try {
                if (cbPesquisar.Text != null) {
                    int codigo = int.Parse(cbPesquisar.Text.Substring(0, 4).Trim());
                    barbeiro = conexao.BARBEIROS.Find(codigo);
                    txtCodigo.Text = barbeiro.codigo.ToString();
                    txtNome.Text = barbeiro.nome;
                    txtEndereco.Text = barbeiro.endereco;
                    txtNumero.Text = barbeiro.numero.ToString();
                    txtBairro.Text = barbeiro.bairro;
                    txtCidade.Text = barbeiro.cidade;
                    cbEstado.Text = barbeiro.estado;
                    MtxtCep.Text = barbeiro.cep;
                    cbSexo.Text = barbeiro.sexo;
                    MtxtCelular.Text = barbeiro.celular;
                }
                else {
                    MessageBox.Show("Barbeiro não encontrado!", "Informação", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                }
            }catch(Exception a){
                MessageBox.Show("Campo vazio ou código invalido!" + "\n" + a.StackTrace, "Erro", MessageBoxButton.OK,
                                MessageBoxImage.Exclamation);

                limpaCampos();
            }
        }

        // exportar para o excel
        private void btnExportar_Click(object sender, RoutedEventArgs e) {
            Util.exportarExcel(dgBarbeiro);
        }


        // botao alterar
        private void btnAlterar_Click(object sender, RoutedEventArgs e) {
            try {
                if (txtCodigo.Text != "") {
                    barbeiro.nome = txtNome.Text;
                    barbeiro.endereco = txtEndereco.Text;
                    barbeiro.numero = int.Parse(txtNumero.Text);
                    barbeiro.bairro = txtBairro.Text;
                    barbeiro.cidade = txtCidade.Text;
                    barbeiro.estado = cbEstado.Text;
                    barbeiro.cep = MtxtCep.Text;
                    barbeiro.sexo = cbSexo.Text;
                    barbeiro.celular = MtxtCelular.Text;
                    conexao.BARBEIROS.AddOrUpdate(barbeiro);
                    conexao.SaveChanges();
                    MessageBox.Show("Dados alterados com sucesso!", "Alterar", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                    carregaGrid();
                    carregaCombopesquisa();
                }
                else {
                    MessageBox.Show("Insira um código ou pesquise para alterar", "Alterar", MessageBoxButton.OK, MessageBoxImage.Information);
                    limpaCampos();
                    return;
                }
            }
            catch (Exception a) {
                MessageBox.Show("Alguns campos não podem ficar vazios" + "\n" + a.StackTrace, "Erro", MessageBoxButton.OK, MessageBoxImage.Warning);
                limpaCampos();
            }
        }

        // carregar combo de pesquisa
        private void carregaCombopesquisa(){
            var sql = from b in conexao.BARBEIROS
                      where b.codigo > 0 
                      select b.codigo + "    - " + b.nome;
            cbPesquisar.ItemsSource = null;
            cbPesquisar.ItemsSource = sql.ToList();
        }
    }
}
