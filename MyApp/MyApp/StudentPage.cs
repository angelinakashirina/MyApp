using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MyApp
{
        public partial class StudentPage : Xamarin.Forms.ContentPage
    {
            bool edited = true;  // условие редактирования

            public Students Student { get; set; }


            public StudentPage(Students student)
            {
                Title = "Информация о ученике";

                Entry nameEntry = new Entry
                {
                    Placeholder = "Введите имя ученика",
                    PlaceholderColor = Color.FromHex("#66666")
                };
                nameEntry.SetBinding(Entry.TextProperty, "Name");

                Entry priceEntry = new Entry
                {
                    Placeholder = "Введите оценку",
                    PlaceholderColor = Color.FromHex("#66666")
                };
                priceEntry.Keyboard = Keyboard.Numeric;
                priceEntry.SetBinding(Entry.TextProperty, "");



                Button button = new Button
                {
                    Text = "Добавить"
                };
                button.Clicked += SaveStudent;

                this.Content = new StackLayout { Children = { nameEntry, priceEntry, button } };





                Student = student;
                if (student == null)
                {
                    Student = new Students();
                    edited = false;
                }
                this.BindingContext = Student;
            }

            async void SaveStudent(object sender, EventArgs e)
            {
                await Navigation.PopAsync();

                // Условие при добавлении
                if (edited == false)
                {
                   NavigationPage navPage =
                        (NavigationPage)Application.Current.MainPage;
                    IReadOnlyList<Page> navStack = navPage.Navigation.NavigationStack;
                    MainPage homePage =
                        navStack[navPage.Navigation.NavigationStack.Count
                        - 1] as MainPage;

                    if (homePage != null)
                    {
                        homePage.AddStudent(Student);
                    }
                }
            }
        }
    }