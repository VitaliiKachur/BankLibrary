﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BankLibrary;

namespace Bank
{
    public partial class Form1 : Form
    {
        private List<Account> _accounts;  
        private Account _currentAccount;
        public Form1()
        {
            InitializeComponent();
            InitializeEvents();

            _accounts = new List<Account>
            {
                new Account("12345678", "Ivan Ivanov", 5000, "1234"),
                new Account("4141252547892585", "Іван Франчук", 7000, "7777"),
                new Account("98765432", "Ольга Петренко", 8000, "1111")
            };

            foreach (var account in _accounts)
            {
                account.OnAuthentication += DisplayMessage;
                account.OnBalanceCheck += DisplayMessage;
                account.OnWithdraw += DisplayMessage;
                account.OnTransfer += DisplayMessage;
            }

            ToggleAuthenticatedControls(false);
            logoutButton.Visible = false;
        }
        private void InitializeEvents()
        {
            authenticateButton.Click -= authenticateButton_Click;
            authenticateButton.Click += authenticateButton_Click;

            checkBalanceButton.Click -= checkBalanceButton_Click;
            checkBalanceButton.Click += checkBalanceButton_Click;

            withdrawButton.Click -= withdrawButton_Click;
            withdrawButton.Click += withdrawButton_Click;

            transferButton.Click -= transferButton_Click;
            transferButton.Click += transferButton_Click;

            logoutButton.Click -= logoutButton_Click;
            logoutButton.Click += logoutButton_Click;
        }
        private void ToggleAuthenticatedControls(bool isAuthenticated)
        {
            checkBalanceButton.Visible = isAuthenticated;
            withdrawButton.Visible = isAuthenticated;
            withdrawAmountTextBox.Visible = isAuthenticated;
            transferButton.Visible = isAuthenticated;
            recipientCardTextBox.Visible = isAuthenticated;
            transferAmountTextBox.Visible = isAuthenticated;
            logoutButton.Visible = isAuthenticated;
            label3.Visible = isAuthenticated;
            label4.Visible = isAuthenticated;
            label5.Visible = isAuthenticated;
            label6.Visible = isAuthenticated;
            label7.Visible = isAuthenticated;
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void DisplayMessage(string message)
        {
            messagesListBox.Items.Add(message);
        }


        private void authenticateButton_Click(object sender, EventArgs e)
        {
            string cardNumber = cardNumberTextBox.Text;
            string pin = pinTextBox.Text;

            _currentAccount = _accounts.FirstOrDefault(a => a.CardNumber == cardNumber && a.Authenticate(cardNumber, pin));

            if (_currentAccount != null)
            {
                DisplayMessage("Вхід успішний.");
                ToggleAuthenticatedControls(true);
            }
            else
            {
                DisplayMessage("Невірний номер картки або PIN.");
                ToggleAuthenticatedControls(false);
            }
        }

        private void checkBalanceButton_Click(object sender, EventArgs e)
        {
            _currentAccount?.CheckBalance();
        }

        private void withdrawButton_Click(object sender, EventArgs e)
        {
            if (_currentAccount == null) return;

            if (decimal.TryParse(withdrawAmountTextBox.Text, out decimal amount))
            {
                if (_currentAccount.Withdraw(amount))
                {
                    DisplayMessage($"Зняття {amount} успішно виконано.");
                }
                else
                {
                    DisplayMessage("Недостатньо коштів.");
                }
            }
            else
            {
                DisplayMessage("Введіть коректну суму для зняття.");
            }
        }

        private void transferButton_Click(object sender, EventArgs e)
        {
            if (_currentAccount == null) return;

            if (decimal.TryParse(transferAmountTextBox.Text, out decimal amount))
            {
                string recipientCard = recipientCardTextBox.Text;
                Account recipient = _accounts.FirstOrDefault(a => a.CardNumber == recipientCard);

                if (recipient != null && _currentAccount.Transfer(recipient, amount))
                {
                    DisplayMessage($"Переказ {amount} на картку {recipientCard} успішно виконано.");
                }
                else
                {
                    DisplayMessage("Переказ не виконано. Перевірте наявність коштів або номер картки одержувача.");
                }
            }
            else
            {
                DisplayMessage("Введіть коректну суму для переказу.");
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            DisplayMessage("Ви вийшли з акаунту.");
            ToggleAuthenticatedControls(false);
            cardNumberTextBox.Clear();
            pinTextBox.Clear();
            withdrawAmountTextBox.Clear();
            transferAmountTextBox.Clear();
            recipientCardTextBox.Clear();
            messagesListBox.Items.Clear();

            _currentAccount = null;
        }
    }
}
