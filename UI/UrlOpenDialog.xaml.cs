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

namespace UI
{
    /// <summary>
    /// Логика взаимодействия для UrlOpenDialog.xaml
    /// </summary>
    public partial class UrlOpenDialog : Window
    {
        public UrlOpenDialog(string title, string question, string defaultAnswer = "")
        {
            InitializeComponent();
            Title = title;
            QuestionLbl.Content = question;
            UrlBox.Text = defaultAnswer;
        }
        private void Window_ContentRendered(object sender, EventArgs e)
        {
            UrlBox.SelectAll();
            UrlBox.Focus();
        }
        public string Answer => UrlBox.Text;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
