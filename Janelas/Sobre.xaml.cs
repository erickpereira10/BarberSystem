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

namespace BarberSystem.Janelas
{
    /// <summary>
    /// Lógica interna para Sobre.xaml
    /// </summary>
    public partial class Sobre : Window
    {
        public Sobre()
        {
            InitializeComponent();
        }

        // mudando a cor da faixa
        private void Rectangle_MouseMove(object sender, MouseEventArgs e) {
            retangulo.Fill = Brushes.DarkBlue;
        }
        private void retangulo_MouseLeave(object sender, MouseEventArgs e) {
            retangulo.Fill = Brushes.DarkGreen;
        }
    }
}