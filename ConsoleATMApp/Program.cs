using System;
using System.Collections.Generic;
using BankLibrary;

namespace ConsoleATMApp
{
    class Program
    {
        static void Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            List<Account> accounts = new List<Account>
            {
                new Account("1234567890", "John Doe", 1000, "1000"),
                new Account("0987654321", "Alice Smith", 1500, "2000") 
            };

            foreach (var account in accounts)
            {
                account.OnAuthentication += ConsoleMessage;
                account.OnBalanceCheck += ConsoleMessage;
                account.OnWithdraw += ConsoleMessage;
                account.OnTransfer += ConsoleMessage;
            }

            bool isAuthenticated = false;
            Account authenticatedAccount = null; 

            while (!isAuthenticated)
            {
                Console.Clear();
                Console.WriteLine("=== Вхід в систему ===");
                Console.WriteLine("Введіть номер картки:");
                Console.Write("=>");
                string cardNumber = Console.ReadLine();
                Console.WriteLine("Введіть PIN:");
                Console.Write("=>");
                string pin = Console.ReadLine();

                foreach (var account in accounts)
                {
                    if (account.Authenticate(cardNumber, pin))
                    {
                        isAuthenticated = true;
                        authenticatedAccount = account;
                        Console.Clear();
                        Console.WriteLine("Аутентифікація успішна!\n");
                        break;
                    }
                }

                if (!isAuthenticated)
                {
                    Console.Clear();
                    Console.WriteLine("Невдала спроба аутентифікації. Спробуйте ще раз! Натисніть Enter");
                    Console.ReadLine();
                }
            }

            bool continueUsing = true;

            while (continueUsing)
            {
                Console.WriteLine("==== Головне меню ====");
                Console.WriteLine("1. Перевірка балансу");
                Console.WriteLine("2. Зняття коштів");
                Console.WriteLine("3. Перерахування коштів");
                Console.WriteLine("4. Реєстрація нового користувача");
                Console.WriteLine("5. Вихід");
                Console.WriteLine("========================");
                Console.WriteLine("Оберіть опцію:");
                Console.Write("=>");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        authenticatedAccount.CheckBalance();
                        break;
                    case "2":
                        Console.WriteLine("Введіть суму для зняття:");
                        decimal withdrawAmount = decimal.Parse(Console.ReadLine());
                        authenticatedAccount.Withdraw(withdrawAmount);
                        break;
                    case "3":
                        Console.WriteLine("Введіть номер картки одержувача:");
                        string recipientCard = Console.ReadLine();
                        Console.WriteLine("Введіть суму для переказу:");
                        decimal transferAmount = decimal.Parse(Console.ReadLine());
                        Account recipient = accounts.Find(acc => acc.CardNumber == recipientCard); 
                        if (recipient != null)
                        {
                            authenticatedAccount.Transfer(recipient, transferAmount);
                        }
                        else
                        {
                            Console.WriteLine("Одержувач не знайдений.");
                        }
                        break;
                    case "4":
                        Console.WriteLine("Введіть номер картки нового користувача:");
                        string newCardNumber = Console.ReadLine();
                        Console.WriteLine("Введіть ім'я нового користувача:");
                        string newOwnerName = Console.ReadLine();
                        Console.WriteLine("Введіть початковий баланс:");
                        decimal newBalance = decimal.Parse(Console.ReadLine());
                        Console.WriteLine("Введіть PIN-код:");
                        string newPin = Console.ReadLine();

                        Account newAccount = new Account(newCardNumber, newOwnerName, newBalance, newPin);
                        accounts.Add(newAccount); 

                        newAccount.OnAuthentication += ConsoleMessage;
                        newAccount.OnBalanceCheck += ConsoleMessage;
                        newAccount.OnWithdraw += ConsoleMessage;
                        newAccount.OnTransfer += ConsoleMessage;

                        Console.WriteLine("Новий користувач успішно зареєстрований!");
                        break;
                    case "5":
                        continueUsing = false;
                        break;
                    default:
                        Console.WriteLine("Неправильний вибір.");
                        break;
                }

                if (continueUsing)
                {
                    Console.WriteLine("\nНатисніть Enter для повернення до меню...");
                    Console.ReadLine();
                    Console.Clear();
                }
            }

            Console.WriteLine("Дякуємо за використання сервісу! Натисніть Enter, щоб завершити...");
            Console.ReadLine();
        }

        static void ConsoleMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}

namespace BankLibrary
{
    public delegate void AccountActionHandler(string message);

    public class Account
    {
        public string CardNumber { get; set; }
        public string OwnerName { get; set; }
        public decimal Balance { get; private set; }
        public string Pin { get; private set; }

        public event AccountActionHandler OnAuthentication;
        public event AccountActionHandler OnBalanceCheck;
        public event AccountActionHandler OnWithdraw;
        public event AccountActionHandler OnTransfer;

        public Account(string cardNumber, string ownerName, decimal initialBalance, string pin)
        {
            CardNumber = cardNumber;
            OwnerName = ownerName;
            Balance = initialBalance;
            Pin = pin;
        }

        public bool Authenticate(string cardNumber, string pin)
        {
            if (this.CardNumber == cardNumber && this.Pin == pin)
            {
                OnAuthentication?.Invoke("Аутентифікація пройшла успішно");
                return true;
            }
            else
            {
                OnAuthentication?.Invoke("Аутентифікація не вдалася");
                return false;
            }
        }

        public decimal CheckBalance()
        {
            OnBalanceCheck?.Invoke($"Ваш поточний баланс: {Balance}");
            return Balance;
        }

        public bool Withdraw(decimal amount)
        {
            if (amount <= Balance)
            {
                Balance -= amount;
                OnWithdraw?.Invoke($"Зняття коштів успішне. Новий баланс: {Balance}");
                return true;
            }
            OnWithdraw?.Invoke("Недостатньо коштів для зняття.");
            return false;
        }

        public bool Transfer(Account recipient, decimal amount)
        {
            if (Withdraw(amount))
            {
                recipient.Balance += amount;
                OnTransfer?.Invoke($"Перерахування успішне. {amount} перераховано на рахунок {recipient.OwnerName}.");
                return true;
            }
            OnTransfer?.Invoke("Перерахування не вдалося.");
            return false;
        }
    }
}
