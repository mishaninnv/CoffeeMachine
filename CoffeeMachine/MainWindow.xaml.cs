using CoffeeMachine.Db;
using CoffeeMachine.Enums;
using CoffeeMachine.Managers;
using CoffeeMachine.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace CoffeeMachine
{
    public partial class MainWindow : Window
    {
        private MainContext _db;
        private RequestModel _request;
        private List<int> denomination = new List<int>() { 1, 2, 5, 10, 50, 100, 200 };
        private int _money;
        private SugarManager _sugarManager;
        private WaterManager _waterManager;
        private CupManager _cupManager;
        private CoffeeManager _coffeeManager;
        private MilkManager _milkManager;

        public MainWindow()
        {
            _db = new MainContext();
            _request = new RequestModel();
            _waterManager = new WaterManager();
            _cupManager = new CupManager();
            _sugarManager = new SugarManager();
            _milkManager = new MilkManager();
            _coffeeManager = new CoffeeManager();

            InitializeDb();
            InitializeComponent();

            UpdateLinks();
        }

        /// <summary>Показать панель администратора.</summary>
        private void AdminPanelB_Click(object sender, RoutedEventArgs e)
        {
            var adminPanel = new AdminPanelWindow();
            adminPanel.Closed += (sender, args) => UpdateLinks();
            adminPanel.Show();
        }

        /// <summary>Инициализация базы данных с заполнением первичными данными.</summary>
        private void InitializeDb()
        {
            _db.Database.EnsureCreated();

            // добавляем строку ресурсов (при ее отсутствии)
            var resources = _db.Resources.FirstOrDefault();
            if (resources == null)
            {
                _db.Resources.Add(new ResourceModel());
            }

            // добавляем отсуствующие единицы в базу данных
            foreach (var unit in Enum.GetValues(typeof(UnitsEnum)))
            {
                var existUnit = _db.Units.FirstOrDefault(x => x.Name == unit.ToString());
                if (existUnit == null && unit != null)
                {
                    _db.Units.Add(new UnitModel() { Name = unit.ToString() });
                }
            }

            // добавляем отсутствующие типы оплаты
            foreach (var paymentType in Enum.GetValues(typeof(PaymentTypesEmun)))
            {
                var existPaymentType = _db.PaymentTypes.FirstOrDefault(x => x.Name == paymentType.ToString());
                if (existPaymentType == null && paymentType != null)
                {
                    _db.PaymentTypes.Add(new PaymentTypeModel() { Name = paymentType.ToString() });
                }
            }

            _db.SaveChanges();
        }

        /// <summary>Добавление связей с данными из базы данных.</summary>
        private void UpdateLinks()
        {
            var test = _db.Coffee.AsNoTracking().ToList();
            selectedCoffee.ItemsSource = _db.Coffee.ToList();
            selectedCoffee.DisplayMemberPath = "Name";
            selectedCoffee.SelectedIndex = 0;

            selectedPaymentType.ItemsSource = _db.PaymentTypes.ToList();
            selectedPaymentType.DisplayMemberPath = "Name";
            selectedPaymentType.SelectedIndex = 0;

            denominationMoney.ItemsSource = denomination;
            denominationMoney.SelectedIndex = 0;
        }

        /// <summary>Обновление данных о заказе.</summary>
        private void UpdateSelectedPosition()
        {
            var selectedItem = (CoffeeModel)selectedCoffee.SelectedItem;
            if (selectedItem == null)
                return;

            var coffee = _db.Coffee.FirstOrDefault(x => x.Name == selectedItem.Name);
            if (coffee != null)
            {
                selectedPosition.Text = string.Format("{0}, молоко - {1}, сахар - {2}, цена {3}.", coffee.Name, _request.AddMilk + coffee.MilkAmount, _request.AddSugar, coffee.Price);
            }
            changeMoney.Text = "0";
        }

        /// <summary>Добавление молока в заказ.</summary>
        private void AddMilk_Click(object sender, RoutedEventArgs e)
        {
            var addUnitMilk = _db.Units.AsNoTracking().FirstOrDefault(x => x.Name == UnitsEnum.MilkUnit.ToString())?.ConvertValue ?? 0;
            if (_db.Resources.AsNoTracking().FirstOrDefault().MilkCurrent > addUnitMilk)
            {
                _request.AddMilk += addUnitMilk;
                UpdateSelectedPosition();
                description.Text = string.Empty;
            }
            else
            {
                description.Text = "Недостаточно молока";
            }
        }

        /// <summary>Добавление сахара в заказ.</summary>
        private void AddSugar_Click(object sender, RoutedEventArgs e)
        {
            var addUnitSugar = _db.Units.AsNoTracking().FirstOrDefault(x => x.Name == UnitsEnum.SugarUnit.ToString())?.ConvertValue ?? 0;
            if (_db.Resources.AsNoTracking().FirstOrDefault().SugarCurrent > addUnitSugar)
            {
                _request.AddSugar += addUnitSugar;
                UpdateSelectedPosition();
                description.Text = string.Empty;
            }
            else
            {
                description.Text = "Недостаточно сахара";
            }
        }

        /// <summary>Обновить данные о заказе при выборе напитка.</summary>
        private void selectedCoffee_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) => UpdateSelectedPosition();

        /// <summary>Добавление денег.</summary>
        private void AddMoney_Click(object sender, RoutedEventArgs e)
        {
            _money += (int)denominationMoney.SelectedItem;
            money.Text = _money.ToString();
        }

        /// <summary>Кнопка оформления заказа.</summary>
        private void Success_Click(object sender, RoutedEventArgs e)
        {
            var error = string.Empty;

            var choosedCoffee = (CoffeeModel)selectedCoffee.SelectedItem;
            var coffee = _db.Coffee.FirstOrDefault(x => x.Name == choosedCoffee.Name);
            var choosedPaymentType = (PaymentTypeModel)selectedPaymentType.SelectedItem;
            var paymentType = _db.PaymentTypes.FirstOrDefault(x => x.Name == choosedPaymentType.Name);
            var resources = _db.Resources.AsNoTracking().FirstOrDefault();

            if (coffee == null || paymentType == null || resources == null)
                return;

            _request.Price = coffee.Price;
            _request.Name = coffee.Name;
            _request.PaymentType = paymentType.Id;
            _request.AddMilk += coffee.MilkAmount;

            error += _money < _request.Price ? "Недостаточно денег." : string.Empty;
            error += resources.CupsCurrent < 1 ? "Отсутствуют стаканчики." : string.Empty;
            error += resources.MilkCurrent < _request.AddMilk ? "Недостаточно молока." : string.Empty;
            error += resources.SugarCurrent < _request.AddSugar ? "Недостаточно сахара." : string.Empty;
            error += resources.WaterCurrent < coffee.WaterAmount ? "Недостаточно воды." : string.Empty;
            error += resources.CoffeeCurrent < coffee.CoffeeAmount ? "Недостаточно кофе." : string.Empty;

            if (!string.IsNullOrWhiteSpace(error))
            {
                description.Text = error;
            }
            else
            {
                changeMoney.Text = (_money - _request.Price).ToString();
                money.Text = "0";
                _money = 0;
                _milkManager.GetMilk(_request.AddMilk);
                _sugarManager.GetSugar(_request.AddSugar);
                _cupManager.GetCup(1);
                _waterManager.GetWater(coffee.WaterAmount);
                _coffeeManager.GetCoffee(coffee.CoffeeAmount);

                _db.Requests.Add(_request);
                _db.SaveChanges();

                _request = new RequestModel();
                description.Text = "Спасибо за заказ.";
            }
        }

        /// <summary>Загрузка тестовых данных (рецепты, ресурсы, единицы).</summary>
        private void LoadTestData_Click(object sender, RoutedEventArgs e)
        {
            var allCoffee = _db.Coffee.ToList();
            foreach (var coffee in allCoffee)
            {
                _db.Coffee.Remove(coffee);
            }

            _db.Coffee.Add(new CoffeeModel() { Name = "Каппучино", CoffeeAmount = 50, WaterAmount = 100, MilkAmount = 20, Price = 150 });
            _db.Coffee.Add(new CoffeeModel() { Name = "Латте", CoffeeAmount = 50, WaterAmount = 100, MilkAmount = 50, Price = 200 });
            _db.Coffee.Add(new CoffeeModel() { Name = "Американо", CoffeeAmount = 50, WaterAmount = 70, MilkAmount = 0, Price = 120 });
            _db.Coffee.Add(new CoffeeModel() { Name = "Эспрессо", CoffeeAmount = 70, WaterAmount = 50, MilkAmount = 0, Price = 100 });

            var resource = _db.Resources.FirstOrDefault();
            resource.CoffeeMax = 5;
            resource.CoffeeCurrent = 2000;
            resource.WaterMax = 5;
            resource.WaterCurrent = 5000;
            resource.MilkMax = 2;
            resource.MilkCurrent = 1000;
            resource.CupsMax = 100;
            resource.CupsCurrent = 100;
            resource.SugarMax = 5;
            resource.SugarCurrent = 1000;
            _db.Resources.Update(resource);
            _db.SaveChanges();

            var milkUnitDb = _db.Units.FirstOrDefault(x => x.Name == UnitsEnum.MilkUnit.ToString());
            milkUnitDb.ConvertValue = 10;
            _db.Units.Update(milkUnitDb);

            var sugarUnitDb = _db.Units.FirstOrDefault(x => x.Name == UnitsEnum.SugarUnit.ToString());
            sugarUnitDb.ConvertValue = 5;
            _db.Units.Update(sugarUnitDb);

            _db.SaveChanges();

            UpdateLinks();
        }
    }
}