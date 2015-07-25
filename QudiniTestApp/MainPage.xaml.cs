using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using System.Runtime.Serialization.Json;
using System.Reflection;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage.Streams;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using System.Net;

namespace QudiniTestApp
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            //Get initial list of customers
            HttpRequest();

            this.InitializeComponent();

            //Handle screen sizes
            Window.Current.SizeChanged += Current_SizeChanged;

            //Set initial visibility of UI components
            #region SetVisibility
            QudiniLogoGrid.Visibility = Visibility.Collapsed; 
            customerlabel_txtblk.Visibility = Visibility.Collapsed;
            customerName_txtblk.Visibility = Visibility.Collapsed;
            customerEAdd_txtblk.Visibility = Visibility.Collapsed;
            customerETime_txtblk.Visibility = Visibility.Collapsed;
            customerOTime_txtblk.Visibility = Visibility.Collapsed;
            customerProduct_txtblk.Visibility = Visibility.Collapsed;
            customerNotes_txtblk.Visibility = Visibility.Collapsed;
            customerName_txtbx.Visibility = Visibility.Collapsed;
            customerEAdd_txtbx.Visibility = Visibility.Collapsed;
            customerETime_txtbx.Visibility = Visibility.Collapsed;
            customerOTime_txtbx.Visibility = Visibility.Collapsed;
            customerProduct_txtbx.Visibility = Visibility.Collapsed;
            customerNotes_txtbx.Visibility = Visibility.Collapsed;
            myImage.Visibility = Visibility.Collapsed;
            CustomerAvatarElementGrid.Visibility = Visibility.Collapsed;
            #endregion

        }


        //Create list containers of customer details
        public List<string> CName = new List<string>();
        public List<string> CEmail = new List<string>();
        public List<string> ETime = new List<string>();
        public List<string> OTime = new List<string>();
        public List<string> Prod = new List<string>();
        public List<string> Notes = new List<string>();


        RootObject obj = new RootObject();

        DispatcherTimer dispatcherTimer;
        public int timesTicked;
        public int timesToTick;

        public int listViewIndex = 0;
        public int executionCount = 0;

        public void DispatcherTimerSetup(int ticked, int toTick)
        {
            timesTicked = ticked;
            timesToTick = toTick;

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        private void dispatcherTimer_Tick(object sender, object e)
        {
            if(timesTicked == 1)
            {
                customerNotes_txtbx.Text = "Refreshing...";
            }
            if (timesTicked == timesToTick)
            {
                dispatcherTimer.Stop();
                HttpRequest();
            }
            timesTicked--;
        }

        void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            if (Windows.UI.ViewManagement.ApplicationView.Value != Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape)
            {
                //Set visibility of UI components if app is snapped
                CustomerListGrid.Visibility = Visibility.Collapsed;
                CustomerDetailsGrid.Visibility = Visibility.Collapsed;
                SnappedGrid.Visibility = Visibility.Visible;

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/BlankBG.png", UriKind.Absolute));
                OuterGrid.Background = imageBrush;
            }
            else if (Windows.UI.ViewManagement.ApplicationView.Value == Windows.UI.ViewManagement.ApplicationViewState.FullScreenLandscape)
            {
                //Set visibility of UI components if app is not snapped
                CustomerListGrid.Visibility = Visibility.Visible;
                CustomerDetailsGrid.Visibility = Visibility.Visible;
                SnappedGrid.Visibility = Visibility.Collapsed;

                ImageBrush imageBrush = new ImageBrush();
                imageBrush.ImageSource = new BitmapImage(new Uri("ms-appx:///Assets/BG-main.png", UriKind.Absolute));
                OuterGrid.Background = imageBrush;
            }
        }

        #region ParseDataStorage
        public class Language
        {
            public string isoCode { get; set; }
            public string name { get; set; }
        }

        public class MerchantCustomer
        {
            public int id { get; set; }
        }

        public class Customer
        {
            public object createdBy { get; set; }
            public string emailAddress { get; set; }
            public int id { get; set; }
            public Language language { get; set; }
            public MerchantCustomer merchantCustomer { get; set; }
            public object mobileNetwork { get; set; }
            public string mobileNumber { get; set; }
            public string name { get; set; }
            public string notes { get; set; }
            public object pagerNumber { get; set; }
        }

        public class Product
        {
            public int id { get; set; }
            public string name { get; set; }
        }

        public class CustomersToday
        {
            public object bookingStartTime { get; set; }
            public object collectingServer { get; set; }
            public int currentPosition { get; set; }
            public Customer customer { get; set; }
            public string expectedTime { get; set; }
            public bool hasBeenSentReturnMessage { get; set; }
            public int id { get; set; }
            public bool inStore { get; set; }
            public bool isArrived { get; set; }
            public bool isFixed { get; set; }
            public string joinedTime { get; set; }
            public object lastSMSStatus { get; set; }
            public string originalExpectedTime { get; set; }
            public Product product { get; set; }
            public object productDescription { get; set; }
            public object timeArrivedInStore { get; set; }
            public string timeSentReturnMessage { get; set; }
            public int unreadMessages { get; set; }
            public object waitTime { get; set; }
        }

        public class Server
        {
            public string displayName { get; set; }
            public int id { get; set; }
        }

        public class Venue
        {
            public string name { get; set; }
        }

        public class Queue
        {
            public int averageServeTimeMinutes { get; set; }
            public List<CustomersToday> customersToday { get; set; }
            public object finishReminder { get; set; }
            public int id { get; set; }
            public string identifier { get; set; }
            public bool isBookingAllowed { get; set; }
            public bool isTabletCollectionEnabled { get; set; }
            public object maxQueueLength { get; set; }
            public string name { get; set; }
            public object requiredMpn { get; set; }
            public List<Server> servers { get; set; }
            public List<object> servingServers { get; set; }
            public object showAssignedCustomerPopup { get; set; }
            public string snsTopicArn { get; set; }
            public int unreadMessagesForQueue { get; set; }
            public Venue venue { get; set; }
        }

        public class Server2
        {
            public object currentBreakReason { get; set; }
            public string displayName { get; set; }
            public int id { get; set; }
        }

        public class ServersAvailable
        {
            public int minutesUntilNextAvailability { get; set; }
            public int nextAvailableMinutes { get; set; }
            public Server2 server { get; set; }
        }

        public class QueueData
        {
            public string currentUserRole { get; set; }
            public object customerServed { get; set; }
            public bool isActive { get; set; }
            public bool isMyLastCustomer { get; set; }
            public int minutesToNextFree { get; set; }
            public Queue queue { get; set; }
            public List<ServersAvailable> serversAvailable { get; set; }
        }

        public class RootObject
        {
            public QueueData queueData { get; set; }
            public string status { get; set; }
        }
        #endregion

        private async void HttpRequest()
        {
            //Set API authentication credentials
            string _auth = string.Format("{0}:{1}", "codetest1", "codetest100");
            string _enc = Convert.ToBase64String(Encoding.UTF8.GetBytes(_auth));
            string _basic = string.Format("{0} {1}", "Basic", _enc);    //Set HTTP authentiation type and embed credentials
                        
            int error = 0;
            try
            {
                //Set web request type and authorization
                HttpClient client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", _basic);

                //Send API request and get API response
                HttpResponseMessage response = await client.GetAsync("https://app.qudini.com/api/queue/gj9fs");
                response.EnsureSuccessStatusCode();

                //Read JSON data
                string responseBody = await response.Content.ReadAsStringAsync();

                //Deserialize JSON data
                obj = JsonConvert.DeserializeObject<RootObject>(responseBody);
                
                //Populate customer list containers
                for (int i = 0; i < obj.queueData.queue.customersToday.Count(); i++)
                {
                    if (executionCount == 0)
                    {
                        //Store data if app is initially executed for the first time
                        CName.Add(obj.queueData.queue.customersToday[i].customer.name);
                        CEmail.Add(obj.queueData.queue.customersToday[i].customer.emailAddress);
                        ETime.Add(obj.queueData.queue.customersToday[i].expectedTime);
                        OTime.Add(obj.queueData.queue.customersToday[i].originalExpectedTime);
                        Prod.Add(obj.queueData.queue.customersToday[i].product.name);
                        Notes.Add(obj.queueData.queue.customersToday[i].customer.notes);
                    }
                    else
                    {
                        //Store data if app is executed at least once
                        CName[i] = obj.queueData.queue.customersToday[i].customer.name;
                        CEmail[i] = obj.queueData.queue.customersToday[i].customer.emailAddress;
                        ETime[i] = obj.queueData.queue.customersToday[i].expectedTime;
                        OTime[i] = obj.queueData.queue.customersToday[i].originalExpectedTime;
                        Prod[i] = obj.queueData.queue.customersToday[i].product.name;
                        Notes[i] = obj.queueData.queue.customersToday[i].customer.notes;
                    }
                }
                //Keep track of the execution times
                executionCount++;

                //Display list of customers in listview container
                int indexCounter = 0;
                foreach (var LItem in CName)
                {
                    jsonDataListview.Items.Insert(indexCounter, LItem);
                    indexCounter++;
                }

                //Handle listview contents for duplicate records
                int listCounter = jsonDataListview.Items.Count();
                if (jsonDataListview.Items.Count() > obj.queueData.queue.customersToday.Count())
                {
                    int i1 = obj.queueData.queue.customersToday.Count();
                    int i2 = jsonDataListview.Items.Count();
                    while(i2 != i1)
                    {
                        jsonDataListview.Items.RemoveAt(i1);
                        i2 = jsonDataListview.Items.Count();
                    }
                    jsonDataListview.SelectedIndex = App.prevIndex;
                }
                customerNotes_txtbx.Text = "Done!";
            }
            catch (HttpRequestException e)
            {
                error = 1;
                customerNotes_txtbx.Text = "Error!";
            }
            if(error != 0)
            {
                error = 0;
                this.HttpRequest();
            }
            DispatcherTimerSetup(30, 0);
        }

        private void getGravatarImages(int SelectedIndex)
        {
            //Get email address of selected customer
            string customerEmail = CEmail[SelectedIndex];
            string imageUrl;
            var im = new BitmapImage();

            //Check if customer email address is null
            if(customerEmail != null && customerEmail != "No Record")
            {
                imageUrl = GravatarImage.GetURL(customerEmail);               
                im = new BitmapImage(new Uri(imageUrl));
            }
            else
            {
                imageUrl = GravatarImage.GetURL("MyEmailAddress@example.com", 80);  //Sample email from Gravatar
                im = new BitmapImage(new Uri(imageUrl));
            }

            customerNotes_txtbx.Text += " Image Loaded";
            myImage.Source = im;   
        }

        private void jsontDataListview_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Set visibility of UI components if the user has selected a record from the listview
            #region SetVisibility
            LogoGrid.Visibility = Visibility.Collapsed;
            QudiniLogoGrid.Visibility = Visibility.Visible;
            customerlabel_txtblk.Visibility = Visibility.Visible;
            customerName_txtblk.Visibility = Visibility.Visible;
            customerEAdd_txtblk.Visibility = Visibility.Visible;
            customerETime_txtblk.Visibility = Visibility.Visible;
            customerOTime_txtblk.Visibility = Visibility.Visible;
            customerProduct_txtblk.Visibility = Visibility.Visible;
            customerNotes_txtblk.Visibility = Visibility.Visible;
            customerName_txtbx.Visibility = Visibility.Visible;
            customerEAdd_txtbx.Visibility = Visibility.Visible;
            customerETime_txtbx.Visibility = Visibility.Visible;
            customerOTime_txtbx.Visibility = Visibility.Visible;
            customerProduct_txtbx.Visibility = Visibility.Visible;
            customerNotes_txtbx.Visibility = Visibility.Visible;
            myImage.Visibility = Visibility.Visible;
            CustomerAvatarElementGrid.Visibility = Visibility.Visible;
            #endregion

            //Create a duplicate data storage
            List<string> CNameCopy = new List<string>();
            List<string> CEmailCopy = new List<string>();
            List<string> ETimeCopy = new List<string>();
            List<string> OTimeCopy = new List<string>();
            List<string> ProdCopy = new List<string>();
            List<string> NotesCopy = new List<string>();

            //Populate duplicate data storage
            for (int i = 0; i < obj.queueData.queue.customersToday.Count(); i++)
            {
                CNameCopy.Insert(i, obj.queueData.queue.customersToday[i].customer.name);
                CEmailCopy.Insert(i, obj.queueData.queue.customersToday[i].customer.emailAddress);
                ETimeCopy.Insert(i, obj.queueData.queue.customersToday[i].expectedTime);
                OTimeCopy.Insert(i, obj.queueData.queue.customersToday[i].originalExpectedTime);
                ProdCopy.Insert(i, obj.queueData.queue.customersToday[i].product.name);
                NotesCopy.Insert(i, obj.queueData.queue.customersToday[i].customer.notes);  
            }
            
            //Remove duplicate records from the list
            if (CNameCopy.Count > obj.queueData.queue.customersToday.Count())
            {
                CNameCopy.RemoveRange(obj.queueData.queue.customersToday.Count() + 1, CNameCopy.Count);
            }

            //Insert "No Record" in the list if the API returns a null value of the specific customer field
            for (int i = 0; i < obj.queueData.queue.customersToday.Count(); i++)
            {
                #region NullRecordReplacement
                if (CNameCopy[i] == null)
                {
                    CNameCopy[i] = "No Record";
                }

                if (CEmailCopy[i] == null)
                {
                    CEmailCopy[i] = "No Record";
                }

                if (ETimeCopy[i] == null)
                {
                    ETimeCopy[i] = "No Record";
                }

                if (OTimeCopy[i] == null)
                {
                    OTimeCopy[i] = "No Record";
                }

                if (ProdCopy[i] == null)
                {
                    ProdCopy[i] = "No Record";
                }

                if (NotesCopy[i] == null)
                {
                    NotesCopy[i] = "No Record";
                }

                if (CName[i] == null)
                {
                    CName[i] = "No Record";
                }

                if (CEmail[i] == null)
                {
                    CEmail[i] = "No Record";
                }

                if (ETime[i] == null)
                {
                    ETime[i] = "No Record";
                }

                if (OTime[i] == null)
                {
                    OTime[i] = "No Record";
                }

                if (Prod[i] == null)
                {
                    Prod[i] = "No Record";
                }

                if (Notes[i] == null)
                {
                    Notes[i] = "No Record";
                }
                #endregion
            }
            
            //Store the current selected index of the listview element
            int listIndex = jsonDataListview.SelectedIndex;
            if(listIndex != -1)
            {
                //Store current selected index in a global variable
                App.prevIndex = listIndex;

                //Populate customer details textboxes based on current selected index
                customerName_txtbx.Text = CName[listIndex];
                customerEAdd_txtbx.Text = CEmail[listIndex];
                customerETime_txtbx.Text = ETime[listIndex];
                customerOTime_txtbx.Text = OTime[listIndex];
                customerProduct_txtbx.Text = Prod[listIndex];
                customerNotes_txtbx.Text = Notes[listIndex];
                getGravatarImages(listIndex);
            }
            else
            {
                //Populate customer details textboxes based on previously selected index
                customerName_txtbx.Text = CNameCopy[App.prevIndex];
                customerEAdd_txtbx.Text = CEmailCopy[App.prevIndex];
                customerETime_txtbx.Text = ETimeCopy[App.prevIndex];
                customerOTime_txtbx.Text = OTimeCopy[App.prevIndex];
                customerProduct_txtbx.Text = ProdCopy[App.prevIndex];
                customerNotes_txtbx.Text = NotesCopy[App.prevIndex];
                getGravatarImages(App.prevIndex);
            }
        }
    }
}
