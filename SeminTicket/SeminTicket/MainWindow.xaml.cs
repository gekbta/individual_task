using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;
using Newtonsoft.Json;

namespace SeminTicket
{
    public partial class MainWindow : Window
    {
        // Определение необходимых списков и переменных
        private List<Client> clients = new List<Client>();
        private string jsonFilePath = "data.json";

        public MainWindow()
        {
            InitializeComponent();

            // Загрузка данных из JSON-файлов и инициализация интерфейса
            apartments = LoadApartmentsFromJson();
            apartmentListView.ItemsSource = apartments;

            clients = LoadDataFromJson();
            requests = new ObservableCollection<Request>(LoadRequestsFromJson());

            requestListView.ItemsSource = requests;
            InitializeComboBoxes();
        }
        // Обработчик события нажатия на кнопку добавления клиента
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = fullNameTextBox.Text;
            DateTime birthDate = birthDateDatePicker.SelectedDate ?? DateTime.MinValue;
            string phoneNumber = phoneNumberTextBox.Text;
            bool isBlocked = blockComboBox.SelectedIndex == 1; // 1 соответствует "Да"

            if (!string.IsNullOrWhiteSpace(fullName) && birthDate != DateTime.MinValue && !string.IsNullOrWhiteSpace(phoneNumber))
            {
                Client Client = new Client
                {
                    FullName = fullName,
                    BirthDate = birthDate,
                    PhoneNumber = phoneNumber,
                    IsBlocked = isBlocked
                };

                clients.Add(Client);
                clientListView.Items.Add(Client);
                SaveDataToJson();

                fullNameTextBox.Clear();
                birthDateDatePicker.SelectedDate = null;
                phoneNumberTextBox.Clear();
                blockComboBox.SelectedIndex = 0; // Устанавливаем "Нет" по умолчанию
                UpdateComboBoxesForRequests();
            }
            else
            {
                MessageBox.Show("Заполните все поля.");
            }
        }

        // Загрузка списка клиентов из JSON-файла
        private List<Client> LoadDataFromJson()
        {
            List<Client> loadedClients = new List<Client>();

            if (File.Exists(jsonFilePath))
            {
                string jsonData = File.ReadAllText(jsonFilePath);
                loadedClients = JsonConvert.DeserializeObject<List<Client>>(jsonData);

                foreach (var client in loadedClients)
                {
                    clientListView.Items.Add(client);
                }
            }

            return loadedClients;
        }
        // Сохранение списка клиентов в JSON-файл
        private void SaveDataToJson()
        {
            string jsonData = JsonConvert.SerializeObject(clients, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(jsonFilePath, jsonData);
        }

        private Client selectedClient;

        // Обработчик события изменения выбора клиента в ListView
        private void ClientListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (clientListView.SelectedItem != null)
            {
                selectedClient = (Client)clientListView.SelectedItem;
                fullNameTextBox.Text = selectedClient.FullName;
                birthDateDatePicker.SelectedDate = selectedClient.BirthDate;
                phoneNumberTextBox.Text = selectedClient.PhoneNumber;

                editButton.IsEnabled = true; // Включение кнопки "Редактировать"
                addButton.IsEnabled = false; // Выключение кнопки "Добавить"
            }
            else
            {
                selectedClient = null;
                fullNameTextBox.Clear();
                birthDateDatePicker.SelectedDate = null;
                phoneNumberTextBox.Clear();

                editButton.IsEnabled = false; // Выключение кнопки "Редактировать"
                addButton.IsEnabled = true; // Включение кнопки "Добавить"
            }
        }

        // Обработчик события нажатия на кнопку редактирования клиента
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedClient != null)
            {
                string fullName = fullNameTextBox.Text;
                DateTime birthDate = birthDateDatePicker.SelectedDate ?? DateTime.MinValue;
                string phoneNumber = phoneNumberTextBox.Text;
                bool isBlocked = blockComboBox.SelectedIndex == 1; // 1 соответствует "Да"

                if (!string.IsNullOrWhiteSpace(fullName) && birthDate != DateTime.MinValue && !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    selectedClient.FullName = fullName;
                    selectedClient.BirthDate = birthDate;
                    selectedClient.PhoneNumber = phoneNumber;
                    selectedClient.IsBlocked = isBlocked;

                    clientListView.Items.Refresh();
                    SaveDataToJson();

                    fullNameTextBox.Clear();
                    birthDateDatePicker.SelectedDate = null;
                    phoneNumberTextBox.Clear();
                    blockComboBox.SelectedIndex = 0; // Устанавливаем "Нет" по умолчанию
                    selectedClient = null;
                    editButton.IsEnabled = false; // Выключение кнопки "Редактировать"
                    // Снимаем выбор с элемента в ListView
                    clientListView.SelectedItem = null;
                    // После редактирования клиента метод обновления ComboBox и списка заявок
                    UpdateComboBoxesForRequests();
                }
                else
                {
                    MessageBox.Show("Заполните все поля.");
                }
            }
        }
        // Обработчик события нажатия на кнопку сброса выбора клиента
        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            selectedClient = null;
            fullNameTextBox.Clear();
            birthDateDatePicker.SelectedDate = null;
            phoneNumberTextBox.Clear();
            clientListView.SelectedItem = null; // Сброс выбранного элемента в списке
        }


        //Кваритиры--------------------------------------------
        private List<Apartment> apartments = new List<Apartment>();
        private Apartment selectedApartment;
        private List<Apartment> LoadApartmentsFromJson()
        {
            string filePath = "apartments.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                List<Apartment> loadedApartments = JsonConvert.DeserializeObject<List<Apartment>>(json);
                return loadedApartments;
            }
            else
            {
                return new List<Apartment>();
            }
        }

        private void AddApartmentButton_Click(object sender, RoutedEventArgs e)
        {
            // Получите данные из элементов интерфейса и создайте новую квартиру
            string address = addressTextBox.Text;
            double area = double.Parse(areaTextBox.Text);
            int numberOfRooms = int.Parse(numberOfRoomsTextBox.Text);
            decimal rentPerDay = decimal.Parse(rentPerDayTextBox.Text);
            string description = descriptionTextBox.Text;
            bool isAvailable = availabilityComboBox.SelectedIndex == 0;

            Apartment apartment = new Apartment
            {
                Address = address,
                Area = area,
                NumberOfRooms = numberOfRooms,
                RentPerDay = rentPerDay,
                Description = description,
                IsAvailable = isAvailable
            };

            // Добавить квартиру в список и обновить отображение
            apartments.Add(apartment);
            apartmentListView.ItemsSource = null;
            apartmentListView.ItemsSource = apartments;
            SaveApartmentsToJson();

            // Очистить поля ввода
            ClearApartmentFields();
            UpdateComboBoxesForRequests();
        }

        private void SaveApartmentsToJson()
        {
            string json = JsonConvert.SerializeObject(apartments, Newtonsoft.Json.Formatting.Indented);

            string filePath = "apartments.json";

            File.WriteAllText(filePath, json);
        }


        private void EditApartmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedApartment != null)
            {
                // Получить данные из элементов интерфейса и обновить выбранную квартиру
                string address = addressTextBox.Text;
                double area = double.Parse(areaTextBox.Text);
                int numberOfRooms = int.Parse(numberOfRoomsTextBox.Text);
                decimal rentPerDay = decimal.Parse(rentPerDayTextBox.Text);
                string description = descriptionTextBox.Text;
                bool isAvailable = availabilityComboBox.SelectedIndex == 0;

                selectedApartment.Address = address;
                selectedApartment.Area = area;
                selectedApartment.NumberOfRooms = numberOfRooms;
                selectedApartment.RentPerDay = rentPerDay;
                selectedApartment.Description = description;
                selectedApartment.IsAvailable = isAvailable;

                // Обновить отображение и сохранить изменения
                apartmentListView.Items.Refresh();
                SaveApartmentsToJson();

                // Очистить поля ввода и снять выбор с квартиры
                ClearApartmentFields();
                apartmentListView.SelectedItem = null;
                UpdateComboBoxesForRequests();
            }
            else
            {
                MessageBox.Show("Выберите квартиру для редактирования.");
            }
        }

        private void ResetSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбросить выбор квартиры и поля ввода
            selectedApartment = null;
            ClearApartmentFields();
            apartmentListView.SelectedItem = null;
        }

        private void ApartmentListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (apartmentListView.SelectedItem != null)
            {
                selectedApartment = (Apartment)apartmentListView.SelectedItem;
                // Заполнить поля ввода данными выбранной квартиры
                addressTextBox.Text = selectedApartment.Address;
                areaTextBox.Text = selectedApartment.Area.ToString();
                numberOfRoomsTextBox.Text = selectedApartment.NumberOfRooms.ToString();
                rentPerDayTextBox.Text = selectedApartment.RentPerDay.ToString();
                descriptionTextBox.Text = selectedApartment.Description;
                availabilityComboBox.SelectedIndex = selectedApartment.IsAvailable ? 0 : 1;

                editApartmentButton.IsEnabled = true;
                addApartmentButton.IsEnabled = false;
            }
            else
            {
                selectedApartment = null;
                ClearApartmentFields();

                editApartmentButton.IsEnabled = false;
                addApartmentButton.IsEnabled = true;
            }
        }

        private void ClearApartmentFields()
        {
            addressTextBox.Clear();
            areaTextBox.Clear();
            numberOfRoomsTextBox.Clear();
            rentPerDayTextBox.Clear();
            descriptionTextBox.Clear();
            availabilityComboBox.SelectedIndex = 0; // Свободна по умолчанию
        }
        private void UpdateComboBoxesForRequests()
        {
            // Обновить источник данных для ComboBox с клиентами
            clientComboBox.ItemsSource = clients.Where(client => !client.IsBlocked).Select(client => client.FullName).ToList();
            // Обновить источник данных для ComboBox с клиентами на вкладке "Заявки"
            clientComboBox.ItemsSource = clients.Where(client => !client.IsBlocked).Select(client => client.FullName).ToList();

            // Обновить источник данных для ComboBox с квартирами на вкладке "Заявки"
            apartmentComboBox.ItemsSource = apartments.Where(apartment => apartment.IsAvailable).Select(apartment => apartment.Address).ToList();
        }



        //Заявки-------------------------------------
        private ObservableCollection<Request> requests = new ObservableCollection<Request>();

        private Request selectedRequest;

        private void InitializeComboBoxes()
        {
            // Заполнить ComboBox с клиентами
            clientComboBox.ItemsSource = clients.Where(client => !client.IsBlocked).Select(client => client.FullName).ToList();

            // Заполнить ComboBox с квартирами
            apartmentComboBox.ItemsSource = apartments.Where(apartment => apartment.IsAvailable).Select(apartment => apartment.Address).ToList();
        }

        private void ClearRequestFields()
        {
            clientComboBox.SelectedIndex = -1;
            apartmentComboBox.SelectedIndex = -1;
            startDatePicker.SelectedDate = null;
            endDatePicker.SelectedDate = null;
            requestStatusComboBox.SelectedIndex = -1;
        }

        private void AddRequestButton_Click(object sender, RoutedEventArgs e)
        {
            string clientName = (string)clientComboBox.SelectedItem;
            string apartmentAddress = (string)apartmentComboBox.SelectedItem;
            DateTime startDate = startDatePicker.SelectedDate ?? DateTime.MinValue;
            DateTime endDate = endDatePicker.SelectedDate ?? DateTime.MinValue;
            ComboBoxItem selectedComboBoxItem = requestStatusComboBox.SelectedItem as ComboBoxItem;
            string status = selectedComboBoxItem?.Content as string;


            if (!string.IsNullOrWhiteSpace(clientName) && !string.IsNullOrWhiteSpace(apartmentAddress)
                && startDate != DateTime.MinValue && endDate != DateTime.MinValue && !string.IsNullOrWhiteSpace(status))
            {
                Request request = new Request
                {
                    ClientName = clientName,
                    ApartmentAddress = apartmentAddress,
                    StartDate = startDate,
                    EndDate = endDate,
                    Status = status
                };

                requests.Add(request);
                SaveRequestsToJson();

                ClearRequestFields();
            }
            else
            {
                MessageBox.Show("Заполните все поля заявки.");
            }
        }

        private void RequestListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (requestListView.SelectedItem != null)
            {
                selectedRequest = (Request)requestListView.SelectedItem;

                clientComboBox.SelectedItem = selectedRequest.ClientName;
                apartmentComboBox.SelectedItem = selectedRequest.ApartmentAddress;
                startDatePicker.SelectedDate = selectedRequest.StartDate;
                endDatePicker.SelectedDate = selectedRequest.EndDate;
                requestStatusComboBox.SelectedItem = selectedRequest.Status;

                editRequestButton.IsEnabled = true;
                addRequestButton.IsEnabled = false;
            }
            else
            {
                selectedRequest = null;
                ClearRequestFields();
                editRequestButton.IsEnabled = false;
                addRequestButton.IsEnabled = true;
            }
        }

        private void EditRequestButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedRequest != null)
            {
                // Получить данные из элементов интерфейса и обновить выбранную заявку
                string clientName = (string)clientComboBox.SelectedItem;
                string apartmentAddress = (string)apartmentComboBox.SelectedItem;
                DateTime startDate = startDatePicker.SelectedDate ?? DateTime.MinValue;
                DateTime endDate = endDatePicker.SelectedDate ?? DateTime.MinValue;
                string status = requestStatusComboBox.SelectedItem as string;

                selectedRequest.ClientName = clientName;
                selectedRequest.ApartmentAddress = apartmentAddress;
                selectedRequest.StartDate = startDate;
                selectedRequest.EndDate = endDate;
                selectedRequest.Status = status;

                // Если заявка была одобрена, изменить статус квартиры на "Занята"
                if (status == "Одобрена")
                {
                    Apartment apartment = apartments.FirstOrDefault(a => a.Address == apartmentAddress);
                    if (apartment != null)
                    {
                        apartment.IsAvailable = false;
                    }
                }

                // Обновить отображение и сохранить изменения
                requestListView.Items.Refresh();
                SaveRequestsToJson();
                SaveApartmentsToJson();

                // Очистить поля ввода и снять выбор с заявки
                ClearRequestFields();
                requestListView.SelectedItem = null;
            }
            else
            {
                MessageBox.Show("Выберите заявку для редактирования.");
            }
        }
        private void ResetRequestSelectionButton_Click(object sender, RoutedEventArgs e)
        {
            // Сбросить выбор заявки и поля ввода
            selectedRequest = null;
            ClearRequestFields();
            requestListView.SelectedItem = null;
        }

        private void SaveRequestsToJson()
        {
            string json = JsonConvert.SerializeObject(requests, Newtonsoft.Json.Formatting.Indented);

            string filePath = "requests.json";

            File.WriteAllText(filePath, json);
        }

        private List<Request> LoadRequestsFromJson()
        {
            string filePath = "requests.json";

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                return JsonConvert.DeserializeObject<List<Request>>(json);
            }

            return new List<Request>();
        }

    }

    public class BooleanToYesNoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? "Да" : "Нет";
            }
            return "Нет";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string strValue && strValue.Equals("Да", StringComparison.OrdinalIgnoreCase);
        }
    }
}
