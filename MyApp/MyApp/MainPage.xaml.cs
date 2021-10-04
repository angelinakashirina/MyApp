using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using Xamarin.Forms;

namespace MyApp
{
    public partial class MainPage : ContentPage
    {
        protected internal ObservableCollection<Students> StudentList { get; set; }
        ListView listView;
        

        public MainPage()
        {

            Title = "Список учеников";

            // Создаем базовую сетку
            Grid titleGrid = new Grid
            {
                ColumnDefinitions =
                {
                    new ColumnDefinition
                    {Width = new GridLength(1, GridUnitType.Star)},
                    new ColumnDefinition
                    {Width = new GridLength(1, GridUnitType.Star)}
                },
                RowDefinitions =
                {
                    new RowDefinition {Height = 35},
                }
            };

            Label studentName = new Label
            {
                Text = "Ученики",
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                TextColor = Color.Black
            };
            Label markValue = new Label
            {
                Text = "Оценка",
                FontSize = Device.GetNamedSize(NamedSize.Title, typeof(Label)),
                TextColor = Color.Black
            };

            titleGrid.Children.Add(studentName, 0, 0);
            titleGrid.Children.Add(markValue, 1, 0);



            // Создание базовой таблицы\листа
            StudentList = new ObservableCollection<Students>();

            listView = new ListView
            {
                // Задание источника данных
                ItemsSource = StudentList,

                ItemTemplate = new DataTemplate(() =>
                {
                    Grid table = new Grid
                    {
                        ColumnDefinitions =
                        {
                            new ColumnDefinition
                            { Width = new GridLength(1, GridUnitType.Star) },
                            new ColumnDefinition
                            { Width = new GridLength(1, GridUnitType.Star) }
                        },

                        RowDefinitions =
                        {
                            new RowDefinition { Height = GridLength.Auto },
                            new RowDefinition { Height = GridLength.Auto }
                        }
                    };

                    Label nameLabel = new Label
                    {
                        FontSize =
                        Device.GetNamedSize(NamedSize.Large, typeof(Label))
                    };
                    nameLabel.SetBinding(Label.TextProperty, "Name");

                    Label priceLabel = new Label
                    {
                        FontSize =
                        Device.GetNamedSize(NamedSize.Large, typeof(Label))
                    };
                    priceLabel.SetBinding(Label.TextProperty, "Price");

                    table.Children.Add(nameLabel, 0, 0);
                    table.Children.Add(priceLabel, 1, 0);


                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Children = {table}
                        }
                    };
                })
            };

            // Кнопка на добавление студентов
            Button buttonAdd = new Button { Text = "Добавить" };
            buttonAdd.Clicked += AddButton_Click;

            // Кнопка для удаления учеников
            Button buttonRemove = new Button { Text = "Удалить" };
            buttonRemove.Clicked += RemoveButton_Click;

            // Кнопка для сохранения изменений(необходимая нам опция)
            Button buttonSave = new Button { Text = "Сохранить изменения" };
            buttonSave.Clicked += SaveButton_Click;

            Button buttonave = new Button { Text = " изменения" };
            buttonave.Clicked += SaveButton_Click;



            // Подгружаем список
            var loadPeople = LoadPeople();
            foreach(var p in loadPeople)
            {
                StudentList.Add(p);
            }



            // Пример API, соединение по HTTP
            string url = "http://api.openweathermap.org/data/2.5/weather?q=Astrakhan" +
                         "&units=metric" +
                         "&appid=6d72fff1fe9f40ae9b27530eddbc8296";

            HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);

            HttpWebResponse httpWebResponse =
            (HttpWebResponse)httpWebRequest.GetResponse();

            string response;

            using (StreamReader streamReader =
            new StreamReader(httpWebResponse.GetResponseStream()))
            {
                response = streamReader.ReadToEnd();
            }

            //Пример использования JSON, десереализация жсона с помощью библиотеки Newtontojson
            ApiTestInfo apiResponse =
            JsonConvert.DeserializeObject<ApiTestInfo>(response);

            Label weather = new Label
            {
                Text = "Погода в Астрахани: " + apiResponse.Main.Temp + " °C",
                FontSize = 20,
                HorizontalOptions = LayoutOptions.Center
            };




            //Определение размещения элементов
            this.Content = new StackLayout { Children = 
                { titleGrid, listView, weather, buttonAdd, buttonRemove, buttonSave } };
        }


        // Папка для сохранения нашего JSON
        string folderPath = 
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        // Метод загрузки учеников
        private List<Students> LoadPeople()
        {
            if (File.Exists(Path.Combine(folderPath, "kohais.json")))
            {
                string json = File.ReadAllText(Path.Combine(folderPath, "kohais.json"));
                return JsonConvert.DeserializeObject<List<Students>>(json);
            }
            else
            {
                return new List<Students>();
            }
        }
        // Метод Сохранения учеников + сериализация JSON 
        public void SaveStudents(List<Students> list)
        {
            string json = JsonConvert.SerializeObject(list);
            File.WriteAllText(Path.Combine(folderPath, "kohais.json"), json);

        }


        // Кнопка сохранения учеников
        private void SaveButton_Click(object sender, EventArgs e)
        {
            var listToSave = new List<Students>();
            listToSave.AddRange(StudentList);
            SaveStudents(listToSave);
        }

        // Добавление нового элемента на странице StudentPage
        private async void AddButton_Click(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new StudentPage(null));
        }

        // Вспомогательный метод для добавления элемента в список
        protected internal void AddStudent(Students student)
        {
            StudentList.Add(student);
        }

        // Удаление выделенного объекта
        private void RemoveButton_Click(object sender, EventArgs e)
        {
            Students student = listView.SelectedItem as Students;
            if (student != null)
            {
                StudentList.Remove(student);
                listView.SelectedItem = null;
            }
        }

    }
}
