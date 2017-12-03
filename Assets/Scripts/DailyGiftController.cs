/*на основе скрипта команды инди-разработчиков Montana Games*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using UnityEngine;

public sealed class DailyGiftController : MonoBehaviour
{
    [Header("Settings")]
    [Header("Others")]
    private int DaysCount;
    private const int MaxDayCount = 7;

    private DateTime LastGiftDateTime;

    private bool CanUserGetGift;

    public Action<int> OnUserWantGift;

    private List<DailyGiftItem> items;

    public GameObject NoInternetMessage;

    private string OnlineTimeUrl = "http://www.microsoft.com";

    private string[] dayNames;
    private string[] giftInfos;
    private bool prVideo;//признак просмотра рекламы
    private int purseOfPlayer;//аналог кошелька игрока
    private void Start()
    {

    }
    void Awake()
    {
        items = new List<DailyGiftItem>(transform.GetComponentsInChildren<DailyGiftItem>());

        Start((days) =>
        {
            days++;  //первый день - 1        
        },
        MaxDayCount, // Максимальное количество дней подряд, которые видит пользователь
        new[]
        {
            "1 День",
            "2 День",
            "3 День",
            "4 День",
            "5 День",
            "6 День",
            "7 День"
        },
        new[]
        {
            "10 GOlD",
            "20 GOlD",
            "30 GOlD",
            "40 GOlD",
            "50 GOlD",
            "60 GOlD",
            "Seif" //в котором 70 gold =)
        }
        );
    }

    void Save()
    {
        PlayerPrefs.SetInt("DaysCount", DaysCount);
        PlayerPrefs.SetString("LastGiftDateTime", LastGiftDateTime.ToLongDateString());
    }
    void Load()
    {
        DaysCount = PlayerPrefs.GetInt("DaysCount");

        if (PlayerPrefs.HasKey("LastGiftDateTime"))
            LastGiftDateTime = DateTime.Parse(PlayerPrefs.GetString("LastGiftDateTime"));
    }


    void SetInfos()
    {
        for (int i = 0; i < items.Count; i++)
        {
            short status = 0;

            if (DaysCount == i && CanUserGetGift)
                status = 1;
            else if (DaysCount > i)
            {
                status = 2;
            }

            var _dayName = string.Empty;

            if (dayNames != null && dayNames.Length > i)
            {
                _dayName = dayNames[i];
            }

            var _giftInfo = string.Empty;

            if (giftInfos != null && giftInfos.Length > i)
            {
                _giftInfo = giftInfos[i];
            }

            items[i].SetInfo(this, _dayName, _giftInfo, status);
        }
    }

    void ShowNoInternetPanel()
    {
        if (NoInternetMessage != null)
            NoInternetMessage.SetActive(true);
    }

    void HideNoInternetPanel()
    {
        if (NoInternetMessage != null)
            NoInternetMessage.SetActive(false);
    }
    /// <summary>
    /// Метод для запуска скрипта
    /// </summary>
    /// <param name="callback"> Функция в которую вернётся день (начиная с 0) за который пользовтелю нужно вручить подарок
    /// По нажатию пользователем кнопки "Забрать подарок".
    /// </param>
    /// <param name="_MaxDaysCount">Максимальное количество дней подряд за которое пользователь получит подарок, и счёт сбросится на новый день</param>
    /// <param name="dayNames">Массив для изменений названий дней</param>
    /// <param name="giftInfos">Массив для изменения описания подарка</param>
    public void Start(Action<int> callback, int _MaxDaysCount = MaxDayCount, string[] dayNames = null, string[] giftInfos = null)
    {
        Load();
        this.dayNames = dayNames;
        this.giftInfos = giftInfos;

        if (callback != null)
            OnUserWantGift = callback;

        DateTime now = DateTime.Now;
        now = GetNistTime();// Связь с сайтом microsoft
        if (now == DateTime.MinValue)
        {
            ShowNoInternetPanel();
            DaysCount = 0;
            CanUserGetGift = false;
            SetInfos();
            return;
        }
        else
            HideNoInternetPanel();

        if (now.AddDays(-1).Day == LastGiftDateTime.Day &&
            now.Hour > 3 &&//начало суток в 3.00
            now.AddDays(-1).Month == LastGiftDateTime.Month &&
            now.AddDays(-1).Year == LastGiftDateTime.Year)
        {
            CanUserGetGift = true;
        }
        else if (now.Day == LastGiftDateTime.Day &&
             now.Month == LastGiftDateTime.Month && now.Year == LastGiftDateTime.Year)
        {
            CanUserGetGift = false;
        }
        else
        if (now.Hour > 3)//начало суток в 3.00
        {//сброс цикла дней на начало (если не заходили один день)
            DaysCount = 0;
            CanUserGetGift = true;
        }
        SetInfos();
    }

    internal void GetGift()
    {
        if (CanUserGetGift)
        {
            OnUserWantGift(DaysCount);
            DaysCount++;

            if (DaysCount == MaxDayCount)
                DaysCount = 0;

            LastGiftDateTime = DateTime.Now;
            CanUserGetGift = false;
        }
        Save();
        SetInfos();
        GetGiftInProject(); //получение подарка
    }
    private DateTime GetNistTime()
    {
        var myHttpWebRequest = (HttpWebRequest)WebRequest.Create(OnlineTimeUrl);
        var response = myHttpWebRequest.GetResponse();
        string todaysDates = response.Headers["date"];
        return System.DateTime.ParseExact(todaysDates, "ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture.DateTimeFormat, DateTimeStyles.AssumeUniversal);
    }
    public void ResetInfo()
    {
        PlayerPrefs.DeleteAll();	// очистка всей информации для этого приложения
    }
    public void SeeVideo()
    {
        prVideo = true;//просмотрена реклама
    }
    public void GetGiftInProject() //метод выдачи подарка. Так же его содержимое можно вызывать в любом месте проекта
    {
        int _gift = 10; //стартовая сумма вознаграждения
        if (prVideo)//если был просмотрен видео-ролик
            _gift *= 2;//подарок удваиваем
        prVideo = false;
        FindObjectOfType<DailyGiftController>().Start((day) =>
        {
            switch (day)
            {
                case 1://Первый день
                    purseOfPlayer += _gift * 1;//Выдаём подарок пользователю.
                    break;
                case 2://Второй день
                    purseOfPlayer += _gift * 2;
                    break;
                case 3://Третий день 
                    purseOfPlayer += _gift * 3;
                    break;
                case 4://Четвертый день
                    purseOfPlayer += _gift * 4;
                    break;
                case 5://Пятый день
                    purseOfPlayer += _gift * 5;
                    break;
                case 6://Шестой день 
                    purseOfPlayer += _gift * 6;
                    break;
                case 7://Седьмой день 
                    purseOfPlayer += _gift * 7;
                    break;
            }
        },
        7 //Максимальное количество дней подряд
        );
        PlayerPrefs.SetInt("purseOfPlayer", purseOfPlayer);//сохраняем сумму в кошельке
    }
}
