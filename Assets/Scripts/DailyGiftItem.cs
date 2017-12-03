/*на основе скрипта команды инди-разработчиков Montana Games*/
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class DailyGiftItem : MonoBehaviour
{
    private DailyGiftController controller;
    void OnEnable() //получаем ссылки на элементы Item для связи
    {
        if (day == null)
        {
            var o1 = transform.Find("day");
            if (o1 != null) day = o1.GetComponent<Text>();
        }

        if (giftInfo == null)
        {
            var gameObject1 = transform.Find("GiftInfo");
            if (gameObject1 != null)
                giftInfo = gameObject1.GetComponent<Text>();
        }

        if (GiftedPanel == null)
        {
            var panel = transform.Find("GiftedPanel");
            if (panel != null)
                GiftedPanel = panel.gameObject;
        }

        if (GiftedText == null)
        {
            var text = transform.Find("GiftedText");
            if (text != null) GiftedText = text.gameObject;
        }

        if (GetGiftButton == null)
        {
            var giftButton = transform.Find("GetGiftButton");
            if (giftButton != null)
                GetGiftButton = giftButton.gameObject;
        }
        if (GetVideoButton == null)
        {
            var VideoButton = transform.Find("GetVideoButton");
            if (VideoButton != null)
                GetVideoButton = VideoButton.gameObject;
        }
        if (LockedPanel == null)
        {
            var o = transform.Find("LockedPanel");
            if (o != null)
                LockedPanel = o.gameObject;
        }
    }
    private void Start()
    {//если не доступна одна из кнопок получения подарка - отключаем анимацию окна
        GetComponent<Animation>().enabled = GetVideoButton.activeInHierarchy;
    }
    public Text day;
    public Text giftInfo;
    public GameObject GiftedPanel;
    public GameObject GiftedText;
    public GameObject GetGiftButton;
    public GameObject GetVideoButton;
    public GameObject LockedPanel;

    public void SetInfo(DailyGiftController controller, string dayText, string giftInfo, int status = 0)
    {
        this.controller = controller;
        this.day.text = dayText;
        this.giftInfo.text = giftInfo;

        switch (status)
        {
            case 0:
                StatusWindow(false, false, false, false, true);//default status (статус окна "награда не доступна"(кнопки скрыты, затемнения нет))
                break;
            case 1:
                StatusWindow(false, false, true, true, false);// Gift can givet (статус окна "награда доступна, ожидает получения")
                break;
            case 2:
                StatusWindow(true, true, false, false, false);// Gift collected (статус окна "награда получена"(кнопка скрыта, затемнение есть))
                break;
        }
    }

    public void OnClickGetGift()
    {
        GetComponent<Animation>().enabled = false;//отключаем анимацию окна
        controller.GetGift();
    }

    void StatusWindow(bool _GiftedPanel, bool _text, bool _buttonGift, bool _buttonVideo, bool _LockedPanel) //default status 
    {
        if (GiftedPanel != null) GiftedPanel.SetActive(_GiftedPanel);
        if (GiftedText != null) GiftedText.SetActive(_text);
        if (GetGiftButton != null) GetGiftButton.SetActive(_buttonGift);
        if (GetVideoButton != null) GetVideoButton.SetActive(_buttonVideo);
        if (LockedPanel != null) LockedPanel.SetActive(_LockedPanel);

    }
}